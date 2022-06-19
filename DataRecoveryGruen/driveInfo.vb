Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports System.Management
Imports System.Management.Instrumentation

Public Class driveInfo

    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub driveInfo(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Visible = True
        ProgressPanel1.Visible = True
        run()

    End Sub

    Dim dataSourceGUI As BindingList(Of driveModel)

    Private Function run()

        Dim result As New BindingList(Of driveModel)()
        dataSourceGUI = result

        Dim dicDrives = New Dictionary(Of Integer, HDD)()
        dicDrives = getAdvancedInformation()


        GridControl1.DataSource = dataSourceGUI
        Try
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_DiskDrive")

            For Each queryObj As ManagementObject In searcher.Get()

                Dim id As String = queryObj("DeviceID")
                Dim laufwerke As String = Nothing
                For Each drive As IO.DriveInfo In IO.DriveInfo.GetDrives
                    If id = getPhysicalDriveID(drive.RootDirectory.ToString.Replace("\", "")) Then
                        laufwerke = laufwerke + drive.RootDirectory.ToString + " "
                    End If
                Next

                If laufwerke = Nothing Then
                    laufwerke = "none"
                End If

                Dim bytes As Double = Double.Parse(queryObj("BytesPerSector")) * Double.Parse(queryObj("TotalSectors"))
                Dim gb As Double = Math.Round(bytes / (1024 * 1024 * 1024), 2)

                Dim Temperature As Double = -1
                Dim PowerOnHours As Double = -1
                Dim PowerOnCount As Double = -1

                For Each drive In dicDrives
                    If (id = drive.Value.Name) Then
                        For Each attr In drive.Value.Attributes
                            If attr.Value.HasData Then
                                If (attr.Value.Attribute.Contains("emperature")) Then
                                    'MessageBox.Show(attr.Value.Data.ToString)
                                    Temperature = attr.Value.Data
                                End If
                                If (attr.Value.Attribute.Equals("Power-on hours count")) Then
                                    'MessageBox.Show(attr.Value.Data.ToString)
                                    PowerOnHours = attr.Value.Data
                                End If
                                If (attr.Value.Attribute.Equals("Power cycle count")) Then
                                    'MessageBox.Show(attr.Value.Data.ToString)
                                    PowerOnCount = attr.Value.Data
                                End If
                            End If
                        Next

                    End If


                Next




                result.Add(New driveModel() With {.Model = queryObj("Model"), .SerialNumber = queryObj("SerialNumber"), .Partitions = Int32.Parse(queryObj("Partitions")), .DeviceID = queryObj("DeviceID"), .FirmewareVersion = queryObj("FirmwareRevision"), .Laufwerke = laufwerke, .TotalSectors = queryObj("TotalSectors"), .BytesPerSector = queryObj("BytesPerSector"), .TotalBytes = bytes, .TotalGB = gb, .InterfaceType = queryObj("InterfaceType"), .Temperature = Temperature, .PowerOnCount = PowerOnCount, .PowerOnHours = PowerOnHours})
                'https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-diskdrive

            Next
        Catch err As ManagementException
            Console.WriteLine("An error occurred while querying for WMI data: " & err.Message)
        End Try

        ProgressPanel1.Visible = False
        'https://social.msdn.microsoft.com/Forums/vstudio/en-US/b3577b7a-ea4b-4c90-a3e9-31a9b621469b/accessing-hard-drive-smart-data-from-vb?forum=vbgeneral

    End Function

    Public Function getPhysicalDriveID(ByVal strLogDisk As String) As String
        Dim logicalDiskId As String = strLogDisk
        Dim deviceId As String = String.Empty
        Dim query As String = ("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" _
                     + (logicalDiskId + "'} WHERE AssocClass = Win32_LogicalDiskToPartition"))
        Dim queryResults = New ManagementObjectSearcher(query)
        Dim partitions = queryResults.Get
        For Each part In partitions
            query = ("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" _
                        + (part("DeviceID") + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
            queryResults = New ManagementObjectSearcher(query)
            Dim drives = queryResults.Get
            For Each drive In drives
                deviceId = drive("DeviceID").ToString
            Next
        Next
        Return deviceId

    End Function

    Public Function getAdvancedInformation() As Dictionary(Of Integer, HDD)
        Dim dicDrives = New Dictionary(Of Integer, HDD)()



        Dim wdSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive")

        ' extract model and interface information
        Dim iDriveIndex As Integer = 0
        For Each drive As ManagementObject In wdSearcher.Get()
            Dim hdd = New HDD()
            hdd.Model = drive("Model").ToString().Trim()
            hdd.Type = drive("InterfaceType").ToString().Trim()
            hdd.InstanceName = drive("PNPDeviceID").ToString().Trim() + "_0"
            hdd.Name = drive("Name").ToString().Trim()
            dicDrives.Add(iDriveIndex, hdd)
            iDriveIndex += 1
        Next drive

        Dim pmsearcher = New ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia")

        ' retrieve hdd serial number
        iDriveIndex = 0
        For Each drive As ManagementObject In pmsearcher.Get()
            ' because all physical media will be returned we need to exit
            ' after the hard drives serial info is extracted
            If iDriveIndex >= dicDrives.Count Then
                Exit For
            End If
            If (drive("SerialNumber")) Is Nothing Then
                dicDrives(iDriveIndex).Serial = "None"
            Else
                dicDrives(iDriveIndex).Serial = drive("SerialNumber").ToString().Trim()
            End If

            'dicDrives(iDriveIndex).Serial = If(drive("SerialNumber") Is Nothing, "None", drive("SerialNumber").ToString().Trim())
            iDriveIndex += 1
        Next drive

        ' get wmi access to hdd 
        Dim searcher = New ManagementObjectSearcher("Select * from Win32_DiskDrive")
        searcher.Scope = New ManagementScope("\root\wmi")

        ' check if SMART reports the drive is failing
        searcher.Query = New ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus")
        iDriveIndex = 0
        For Each drive As ManagementObject In searcher.Get()

            For Each kvp As KeyValuePair(Of Integer, HDD) In dicDrives
                If (dicDrives(kvp.Key).InstanceName.Equals(UCase(drive.Properties("InstanceName").Value))) Then
                    dicDrives(kvp.Key).IsOK = DirectCast(drive.Properties("PredictFailure").Value, Boolean) = False
                End If
            Next




            iDriveIndex += 1
        Next drive

        ' retrive attribute flags, value worste and vendor data information
        searcher.Query = New ObjectQuery("Select * from MSStorageDriver_FailurePredictData")
        iDriveIndex = 0
        For Each data As ManagementObject In searcher.Get()

            For Each kvp As KeyValuePair(Of Integer, HDD) In dicDrives
                If (dicDrives(kvp.Key).InstanceName.Equals(UCase(data.Properties("InstanceName").Value))) Then




                    Dim bytes() As Byte = DirectCast(data.Properties("VendorSpecific").Value, Byte())
                    For i As Integer = 0 To 29
                        Try
                            Dim id As Integer = bytes(i * 12 + 2)

                            Dim flags As Integer = bytes(i * 12 + 4) ' least significant status byte, +3 most significant byte, but not used so ignored.
                            'bool advisory = (flags & 0x1) == 0x0;
                            Dim failureImminent As Boolean = (flags And &H1) = &H1
                            'bool onlineDataCollection = (flags & 0x2) == 0x2;

                            Dim value As Integer = bytes(i * 12 + 5)
                            Dim worst As Integer = bytes(i * 12 + 6)
                            Dim vendordata As Integer = BitConverter.ToInt32(bytes, i * 12 + 7)
                            If id = 0 Then
                                Continue For
                            End If

                            Dim attr = dicDrives(kvp.Key).Attributes(id)
                            attr.Current = value
                            attr.Worst = worst
                            attr.Data = vendordata
                            attr.IsOK = failureImminent = False
                        Catch
                            ' given key does not exist in attribute collection (attribute not in the dictionary of attributes)
                        End Try
                    Next i
                    iDriveIndex += 1





                End If
            Next


        Next data

        ' retreive threshold values foreach attribute
        searcher.Query = New ObjectQuery("Select * from MSStorageDriver_FailurePredictThresholds")
        iDriveIndex = 0
        For Each data As ManagementObject In searcher.Get()


            For Each kvp As KeyValuePair(Of Integer, HDD) In dicDrives
                If (dicDrives(kvp.Key).InstanceName.Equals(UCase(data.Properties("InstanceName").Value))) Then



                    Dim bytes() As Byte = DirectCast(data.Properties("VendorSpecific").Value, Byte())
                    For i As Integer = 0 To 29
                        Try

                            Dim id As Integer = bytes(i * 12 + 2)
                            Dim thresh As Integer = bytes(i * 12 + 3)
                            If id = 0 Then
                                Continue For
                            End If

                            Dim attr = dicDrives(kvp.Key).Attributes(id)
                            attr.Threshold = thresh
                        Catch
                            ' given key does not exist in attribute collection (attribute not in the dictionary of attributes)
                        End Try
                    Next i

                    iDriveIndex += 1




                End If
            Next



        Next data



        Return dicDrives


    End Function


End Class


Public Class driveModel
    <Display(Name:="Model")>
    Public Property Model() As String

    <Display(Name:="SerialNumber")>
    Public Property SerialNumber() As String


    <Display(Name:="Partitions")>
    Public Property Partitions() As Int32

    <Display(Name:="DeviceID")>
    Public Property DeviceID() As String

    <Display(Name:="FirmewareVersion")>
    Public Property FirmewareVersion() As String

    <Display(Name:="Laufwerke")>
    Public Property Laufwerke() As String

    <Display(Name:="TotalSectors")>
    Public Property TotalSectors() As Double

    <Display(Name:="BytesPerSector")>
    Public Property BytesPerSector() As Double

    <Display(Name:="TotalBytes")>
    Public Property TotalBytes() As Double

    <Display(Name:="TotalGB")>
    Public Property TotalGB() As Double

    <Display(Name:="InterfaceType")>
    Public Property InterfaceType() As String

    <Display(Name:="Temperature")>
    Public Property Temperature() As Double

    <Display(Name:="Power On Hours")>
    Public Property PowerOnHours() As Double

    <Display(Name:="Power On Count")>
    Public Property PowerOnCount() As Double



End Class


'https://social.msdn.microsoft.com/Forums/vstudio/en-US/76842db3-0746-4769-bf51-c6d129684b72/get-all-information-from-hard-disk-using-vbnet?forum=vbgeneral
Public Class HDD

    Public Property Index() As Integer
    Public Property IsOK() As Boolean
    Public Property Model() As String
    Public Property Type() As String
    Public Property InstanceName() As String
    Public Property Name() As String
    Public Property Serial() As String
    Public Attributes As New Dictionary(Of Integer, Smart)() From {
        {&H0, New Smart("Invalid")},
        {&H1, New Smart("Raw read error rate")},
        {&H2, New Smart("Throughput performance")},
        {&H3, New Smart("Spinup time")},
        {&H4, New Smart("Start/Stop count")},
        {&H5, New Smart("Reallocated sector count")},
        {&H6, New Smart("Read channel margin")},
        {&H7, New Smart("Seek error rate")},
        {&H8, New Smart("Seek timer performance")},
        {&H9, New Smart("Power-on hours count")},
        {&HA, New Smart("Spinup retry count")},
        {&HB, New Smart("Calibration retry count")},
        {&HC, New Smart("Power cycle count")},
        {&HD, New Smart("Soft read error rate")},
        {&HB8, New Smart("End-to-End error")},
        {&HBE, New Smart("Airflow Temperature")},
        {&HBF, New Smart("G-sense error rate")},
        {&HC0, New Smart("Power-off retract count")},
        {&HC1, New Smart("Load/Unload cycle count")},
        {&HC2, New Smart("HDD temperature")},
        {&HC3, New Smart("Hardware ECC recovered")},
        {&HC4, New Smart("Reallocation count")},
        {&HC5, New Smart("Current pending sector count")},
        {&HC6, New Smart("Offline scan uncorrectable count")},
        {&HC7, New Smart("UDMA CRC error rate")},
        {&HC8, New Smart("Write error rate")},
        {&HC9, New Smart("Soft read error rate")},
        {&HCA, New Smart("Data Address Mark errors")},
        {&HCB, New Smart("Run out cancel")},
        {&HCC, New Smart("Soft ECC correction")},
        {&HCD, New Smart("Thermal asperity rate (TAR)")},
        {&HCE, New Smart("Flying height")},
        {&HCF, New Smart("Spin high current")},
        {&HD0, New Smart("Spin buzz")},
        {&HD1, New Smart("Offline seek performance")},
        {&HDC, New Smart("Disk shift")},
        {&HDD, New Smart("G-sense error rate")},
        {&HDE, New Smart("Loaded hours")},
        {&HDF, New Smart("Load/unload retry count")},
        {&HE0, New Smart("Load friction")},
        {&HE1, New Smart("Load/Unload cycle count")},
        {&HE2, New Smart("Load-in time")},
        {&HE3, New Smart("Torque amplification count")},
        {&HE4, New Smart("Power-off retract count")},
        {&HE6, New Smart("GMR head amplitude")},
        {&HE7, New Smart("Temperature")},
        {&HF0, New Smart("Head flying hours")},
        {&HFA, New Smart("Read error retry rate")}
    }

End Class



Public Class Smart
    Public ReadOnly Property HasData() As Boolean
        Get
            If Current = 0 AndAlso Worst = 0 AndAlso Threshold = 0 AndAlso Data = 0 Then
                Return False
            End If
            Return True
        End Get
    End Property
    Public Property Attribute() As String
    Public Property Current() As Integer
    Public Property Worst() As Integer
    Public Property Threshold() As Integer
    Public Property Data() As Integer
    Public Property IsOK() As Boolean

    Public Sub New()

    End Sub

    Public Sub New(ByVal attributeName As String)
        Me.Attribute = attributeName
    End Sub
End Class