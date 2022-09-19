Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports System.Management
Imports System.Management.Instrumentation

Public Class getHelp



    Private Async Sub get_Help(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown

        Me.Visible = True
        ProgressPanel1.Visible = True
        Try
            Await getAsyncInfo()
            Me.Close()
        Catch ex As Exception
            run()
            Me.Visible = False
            ProgressPanel1.Visible = False
        End Try


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

        dicDrives = driveInfo.getAdvancedInformation()


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


        Try
            Process.Start(webAddress)
        Catch ex As Exception
            MessageBox.Show(Form1.getValue("no_default_browser"))
        End Try




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






End Class

