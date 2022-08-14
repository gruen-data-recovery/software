Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports System.Management
Imports System.Management.Instrumentation

Public Class getHelp



    Private Async Sub get_Help(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown

        Me.Visible = True
        ProgressPanel1.Visible = True
        Await getAsyncInfo()
        Me.Close()

    End Sub


    Private Async Function getAsyncInfo() As Threading.Tasks.Task
        Await Threading.Tasks.Task.Run(Sub()
                                           run()

                                       End Sub).ConfigureAwait(False)
    End Function

    Private Function run()

        Dim webAddress As String
        webAddress = "https://www.data-recovery.de/datenrettung-anfragen/?redirect=freeware" + "&os=" + My.Computer.Info.OSPlatform
        Dim dn As Integer
        dn = 1


        Dim dicDrives = New Dictionary(Of Integer, HDD)()
        dicDrives = getAdvancedInformation()


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

                Dim bps As Double = 0
                Dim ts As Double = 0
                Dim bytes As Double = 0


                If (Double.TryParse(queryObj("BytesPerSector"), bps) And Double.TryParse(queryObj("TotalSectors"), ts)) Then
                    bytes = bps * ts
                End If
                Dim gb As Double = Math.Round(bytes / (1024 * 1024 * 1024), 2)

                Dim Temperature As Double = -1
                Dim PowerOnHours As Double = -1
                Dim PowerOnCount As Double = -1

                For Each drive In dicDrives
                    If (id = drive.Value.Name) Then
                        For Each attr In drive.Value.Attributes
                            If attr.Value.HasData Then
                                If (attr.Value.Attribute.Contains("emperature")) Then
                                    'MessageBox.Show(CInt(“&H” & Strings.Right(Hex(attr.Value.Data), 2)).ToString)
                                    Temperature = CInt(“&H” & Strings.Right(Hex(attr.Value.Data), 2))
                                    'convert value into hex, get last 2 values and convert back to decimal
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

                webAddress = webAddress + "&model" + CStr(dn) + "=" + queryObj("Model") + "&serialNumber" + CStr(dn) + "=" + queryObj("SerialNumber") + "&partitions" + CStr(dn) + "=" + CStr(queryObj("Partitions")) + "&deviceID" + CStr(dn) + "=" + queryObj("DeviceID") + "&firmwareVersion" + CStr(dn) + "=" + queryObj("FirmwareRevision") + "&laufwerke" + CStr(dn) + "=" + laufwerke + "&totalSectors" + CStr(dn) + "=" + CStr(queryObj("TotalSectors")) + "&bytesPerSector" + CStr(dn) + "=" + CStr(queryObj("BytesPerSector")) + "&totalBytes" + CStr(dn) + "=" + CStr(bytes) + "&totalGB" + CStr(dn) + "=" + CStr(gb) + "&interfaceType" + CStr(dn) + "=" + queryObj("InterfaceType") + "&temperature" + CStr(dn) + "=" + CStr(Temperature) + "&powerOnCount" + CStr(dn) + "=" + CStr(PowerOnCount) + "&PowerOnHours" + CStr(dn) + "=" + CStr(PowerOnHours) + ""


                dn = dn + 1


                'https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-diskdrive

            Next
        Catch err As ManagementException
            Console.WriteLine("An error occurred while querying for WMI data: " & err.Message)
        End Try

        'https://social.msdn.microsoft.com/Forums/vstudio/en-US/b3577b7a-ea4b-4c90-a3e9-31a9b621469b/accessing-hard-drive-smart-data-from-vb?forum=vbgeneral


        Process.Start(webAddress)


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
            Try
                hdd.Model = drive("Model").ToString().Trim()
            Catch ex As Exception
                hdd.Model = "unknown"
            End Try
            Try
                If (drive("InterfaceType") = Nothing) Then
                    hdd.Type = "unknown"
                Else
                    hdd.Type = drive("InterfaceType").ToString().Trim()
                End If
            Catch ex As Exception
                hdd.Type = "unknown"
            End Try
            Try
                hdd.InstanceName = drive("PNPDeviceID").ToString().Trim() + "_0"
            Catch ex As Exception
                hdd.InstanceName = "unknown"
            End Try
            Try
                hdd.Name = drive("Name").ToString().Trim()
            Catch ex As Exception
                hdd.Name = "unknown"
            End Try
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

