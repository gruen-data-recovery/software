Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports DevExpress.XtraBars

Imports System.Runtime.InteropServices.Marshal
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.SpreadsheetSource.Implementation
Imports System.Globalization
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraCharts
Imports System.Text
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors
Imports System.Device.Location


Public Class Form1
#Region "NATIVE FUNCTIONS"
#Region "API"
    Private Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, ByVal lpSecurityAttributes As Integer, ByVal dwCreationDisposition As Integer, ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As Integer) As Integer
    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Integer) As Integer
    Private Declare Function DeviceIoControlNTFS Lib "kernel32" Alias "DeviceIoControl" (ByVal hDevice As Int32, ByVal dwIoControlCode As Int32, ByRef lpInBuffer As Object, ByVal nInBufferSize As Int32, ByRef lpOutBuffer As NTFS_VOLUME_DATA_BUFFER, ByVal nOutBufferSize As Int32, ByRef lpBytesReturned As Int32, ByVal lpOverlapped As Int32) As Int32
    Private Structure NTFS_VOLUME_DATA_BUFFER
        Dim VolumeSerialNumber As Int64
        Dim NumberSectors As Int64
        Dim TotalClusters As Int64
        Dim FreeClusters As Int64
        Dim TotalReserved As Int64
        Dim BytesPerSector As Int32 'UInt32
        Dim BytesPerCluster As Int32 'UInt32
        Dim BytesPerFileRecordSegment As Int32 'UInt32
        Dim ClustersPerFileRecordSegment As Int32 'UInt32
        Dim MftValidDataLength As Int64
        Dim MftStartLcn As Int64
        Dim Mft2StartLcn As Int64
        Dim MftZoneStart As Int64
        Dim MftZoneEnd As Int64
    End Structure
    Private Structure STANDARD_MFT_ENTRY
        Dim FileNumber As Int32 '0x00-0x03  FILE: For file clusters    BAAD: For bad clusters
        Dim UpdateSequenceOffset As Int16 '0x04-0x05
        Dim FixupArrayEntryCount As Int16 '0x06-0x07
        Dim LogfileSequenceNumber As Int64 '0x08-0x0F   $Logfile (LSN)
        Dim SequenceNumber As Int16 '0x10-0x11
        Dim HardLinkCount As Int16 '0x12-0x13
        Dim OffsetToFirstAttribute As Int16 '0x14-0x15
        Dim Flags As Int16 '0x16-0x17   0x01:Record in use  0x02:Directory (ACTUALLY 0x0000:DeletedFile 0x0100:File 0x0200:DeletedFolder 0x0300:Folder)
        Dim UsedEntrySize As Int32 '0x18-0x1B   How much of this mft entry is used
        Dim AllocatedEntrySize As Int32 '0x1C-0x1F   How much space this mft entry takes
        Dim FileReference As Int64 '0x20-0x27   File reference to the base of $FILE record
        Dim NextAttributeID As Int16 '0x28-0x29
        Dim AlignTo4BBoundary As Int16 '0x2A-0x2B  (XP and above)
        Dim MFTRecordNumber As Int32 '0x2C-0x2F  (XP and above)
        Dim Unused1 As Int64 '0x30-0x37
    End Structure
    Private Enum MFT_ENTRY_FILE_TYPE_FLAGS
        DeletedFile = 0
        File = 1
        DeletedDirectory = 2
        Directory = 3
        Stream = 9
        Metadata = 13
    End Enum
    Private Enum EFileAccess As System.Int32
        DELETE = &H10000
        READ_CONTROL = &H20000
        WRITE_DAC = &H40000
        WRITE_OWNER = &H80000
        SYNCHRONIZE = &H100000
        STANDARD_RIGHTS_REQUIRED = &HF0000
        STANDARD_RIGHTS_READ = READ_CONTROL
        STANDARD_RIGHTS_WRITE = READ_CONTROL
        STANDARD_RIGHTS_EXECUTE = READ_CONTROL
        STANDARD_RIGHTS_ALL = &H1F0000
        SPECIFIC_RIGHTS_ALL = &HFFFF
        ACCESS_SYSTEM_SECURITY = &H1000000
        MAXIMUM_ALLOWED = &H2000000
        GENERIC_READ = &H80000000
        GENERIC_WRITE = &H40000000
        GENERIC_EXECUTE = &H20000000
        GENERIC_ALL = &H10000000
    End Enum
    Private Enum EFileShare
        FILE_SHARE_NONE = &H0
        FILE_SHARE_READ = &H1
        FILE_SHARE_WRITE = &H2
        FILE_SHARE_DELETE = &H4
    End Enum
    Private Enum ECreationDisposition
        CREATE_NEW = 1
        CREATE_ALWAYS = 2
        OPEN_EXISTING = 3
        OPEN_ALWAYS = 4
        TRUNCATE_EXISTING = 5
    End Enum
    Private Enum EFileAttributes
        FILE_ATTRIBUTE_READONLY = &H1
        FILE_ATTRIBUTE_HIDDEN = &H2
        FILE_ATTRIBUTE_SYSTEM = &H4
        FILE_ATTRIBUTE_DIRECTORY = &H10
        FILE_ATTRIBUTE_ARCHIVE = &H20
        FILE_ATTRIBUTE_DEVICE = &H40
        FILE_ATTRIBUTE_NORMAL = &H80
        FILE_ATTRIBUTE_TEMPORARY = &H100
        FILE_ATTRIBUTE_SPARSE_FILE = &H200
        FILE_ATTRIBUTE_REPARSE_POINT = &H400
        FILE_ATTRIBUTE_COMPRESSED = &H800
        FILE_ATTRIBUTE_OFFLINE = &H1000
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = &H2000
        FILE_ATTRIBUTE_ENCRYPTED = &H4000
        FILE_ATTRIBUTE_VIRTUAL = &H10000
        FILE_FLAG_BACKUP_SEMANTICS = &H2000000
        FILE_FLAG_DELETE_ON_CLOSE = &H4000000
        FILE_FLAG_NO_BUFFERING = &H2000000
        FILE_FLAG_OPEN_NO_RECALL = &H100000
        FILE_FLAG_OPEN_REPARSE_POINT = &H200000
        FILE_FLAG_OVERLAPPED = &H40000000
        FILE_FLAG_POSIX_SEMANTICS = &H100000
        FILE_FLAG_RANDOM_ACCESS = &H10000000
        FILE_FLAG_SEQUENTIAL_SCAN = &H8000000
        FILE_FLAG_WRITE_THROUGH = &H80000000
    End Enum
    Private Enum FILE_DEVICE
        FILE_DEVICE_BEEP = 1
        FILE_DEVICE_CD_ROM
        FILE_DEVICE_CD_ROM_FILE_SYSTEM
        FILE_DEVICE_CONTROLLER
        FILE_DEVICE_DATALINK
        FILE_DEVICE_DFS
        FILE_DEVICE_DISK
        FILE_DEVICE_DISK_FILE_SYSTEM
        FILE_DEVICE_FILE_SYSTEM
        FILE_DEVICE_INPORT_PORT
        FILE_DEVICE_KEYBOARD
        FILE_DEVICE_MAILSLOT
        FILE_DEVICE_MIDI_IN
        FILE_DEVICE_MIDI_OUT
        FILE_DEVICE_MOUSE
        FILE_DEVICE_MULTI_UNC_PROVIDER
        FILE_DEVICE_NAMED_PIPE
        FILE_DEVICE_NETWORK
        FILE_DEVICE_NETWORK_BROWSER
        FILE_DEVICE_NETWORK_FILE_SYSTEM
        FILE_DEVICE_NULL
        FILE_DEVICE_PARALLEL_PORT
        FILE_DEVICE_PHYSICAL_NETCARD
        FILE_DEVICE_PRINTER
        FILE_DEVICE_SCANNER
        FILE_DEVICE_SERIAL_MOUSE_PORT
        FILE_DEVICE_SERIAL_PORT
        FILE_DEVICE_SCREEN
        FILE_DEVICE_SOUND
        FILE_DEVICE_DEVICE_STREAMS
        FILE_DEVICE_TAPE
        FILE_DEVICE_TAPE_FILE_SYSTEM
        FILE_DEVICE_TRANSPORT
        FILE_DEVICE_UNKNOWN
        FILE_DEVICE_VIDEO
        FILE_DEVICE_VIRTUAL_DISK
        FILE_DEVICE_WAVE_IN
        FILE_DEVICE_WAVE_OUT
        FILE_DEVICE_8042_PORT
        FILE_DEVICE_NETWORK_REDIRECTOR
        FILE_DEVICE_BATTERY
        FILE_DEVICE_BUS_EXTENDER
        FILE_DEVICE_MODEM
        FILE_DEVICE_VDM
        FILE_DEVICE_MASS_STORAGE
        FILE_DEVICE_SMB
        FILE_DEVICE_KS
        FILE_DEVICE_CHANGER
        FILE_DEVICE_SMARTCARD
        FILE_DEVICE_ACPI
        FILE_DEVICE_DVD
        FILE_DEVICE_FULLSCREEN_VIDEO
        FILE_DEVICE_DFS_FILE_SYSTEM
        FILE_DEVICE_DFS_VOLUME
    End Enum
    Private Const FILE_ANY_ACCESS = &H0
    Private Const FILE_READ_ACCESS = &H1
    Private Const FILE_WRITE_ACCESS = &H2
    Private Const METHOD_BUFFERED = &H0
    Private Const METHOD_IN_DIRECT = &H1
    Private Const METHOD_OUT_DIRECT = &H2
    Private Const METHOD_NEITHER = &H3
#End Region
#Region "FUNCTIONS"
    Private Function CTL_CODE(ByVal DeviceType As Int32, ByVal FunctionNumber As Int32, ByVal Method As Int32, ByVal Access As Int32) As Int32
        Return (DeviceType << 16) Or (Access << 14) Or (FunctionNumber << 2) Or Method
    End Function
    Private Function ByteArrayPart(ByVal Arrays() As Byte, ByVal LBound As Integer, ByVal UBound As Integer) As Byte()
        Dim temp(UBound - LBound + 1) As Byte
        Array.Copy(Arrays, LBound, temp, 0, UBound - LBound + 1)
        Return temp
    End Function
    Private Function GetFullPath2(ByVal ParentID As Integer, ByVal MFTBaseAddress As Long, ByVal MFTEntrySize As Integer, ByVal BytesPerCluster As Integer) As String
        If ParentID = 5 Then Return Strings.Left(dDive.Disk, 2)
        Dim OrgMFTBA = MFTBaseAddress
        Dim Length As ULong = 0
        Dim LenLen As Byte = 0
        Dim Offset As ULong = 0
        Dim OffLen As Byte = 0
        Dim MFT() As Byte = dDive.ReadSectors(MFTBaseAddress, MFTEntrySize)
        Dim baseaddrM = MergeToInt(MFT, &H14, &H15) 'The offset the the first attribute
        While MFT(baseaddrM) <> &H80
            baseaddrM = baseaddrM + MergeToInt(MFT, baseaddrM + &H4, baseaddrM + &H7) 'Add the length of the attribute to the base address to find the next attribute
        End While
        baseaddrM = baseaddrM + &H40
        While MFT(baseaddrM) > 0
            LenLen = MFT(baseaddrM) And &HF
            OffLen = (MFT(baseaddrM) And &HF0) / &H10
            Length = MergeToInt(MFT, baseaddrM + 1, baseaddrM + LenLen)
            Offset = Offset + MergeToInt(MFT, baseaddrM + 1 + LenLen, baseaddrM + LenLen + OffLen)
            If ((CLng(ParentID) * CLng(MFTEntrySize) * CLng(dDive.BytesPerSector)) / CLng(BytesPerCluster)) > Length Then
                ParentID = ParentID - ((Length * CLng(BytesPerCluster)) \ (CLng(MFTEntrySize) * CLng(dDive.BytesPerSector)))
            Else
                MFTBaseAddress = (Offset * CLng(BytesPerCluster)) \ CLng(dDive.BytesPerSector)
                Exit While
            End If
            baseaddrM = baseaddrM + (1 + LenLen + OffLen)
        End While

        Dim Bytes() = dDive.ReadSectors(MFTBaseAddress + CLng(ParentID * MFTEntrySize), 2)
        Dim baseaddr = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
        baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        Dim Parent = MergeToInt(Bytes, baseaddr + &H18, baseaddr + &H1D)
        Dim Name = System.Text.UnicodeEncoding.Unicode.GetString(ByteArrayPart(Bytes, baseaddr + &H5A, (baseaddr + &H5A) + ((2 * Bytes(baseaddr + &H58)) - 2)))
        baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        If Name.Contains("~") Then Name = System.Text.UnicodeEncoding.Unicode.GetString(ByteArrayPart(Bytes, baseaddr + &H5A, (baseaddr + &H5A) + ((2 * Bytes(baseaddr + &H58)) - 2)))
        If Name.Length >= 75 Then Name = "SKIPTHISFILE" 'Name = Mid(Name, 1, 74) & Mid(Name, 76, Name.Length - 75)
        'Name.Replace(Chr(0), "+")
        'Name = Mid(Name, 1, Len(Name) - 1)
        Name = Name.Trim(Chr(0))
        Return GetFullPath2(Parent, OrgMFTBA, MFTEntrySize, BytesPerCluster) & "\" & Name
    End Function
    Private Function MergeToInt(ByVal Array() As Byte, ByVal LBound As Integer, ByVal Ubound As Integer) As Int64
        Dim result As UInt64 = 0
        For action = 0 To Ubound - LBound
            'result = result + ((Array(Ubound - action)) << (action * 2))
            'result = result + ((Array(LBound + action)) << (action * 2))
            Try
                result = result + ((Array(LBound + action)) * (2 ^ (action * 8)))
            Catch
            End Try
            'result = result + ((Array(Ubound - action)) * (2 ^ (action * 8)))
        Next
        Try
            Return Convert.ToInt64(result)
        Catch
            Try
                Return Convert.ToInt64((result - Int64.MaxValue) - Int64.MaxValue)
            Catch
                Return 0
            End Try
        End Try
    End Function
    Private Function GetStringFromByteArray(ByVal Bytes() As Byte) As String
        Dim str As String = ""
        For Each part In Bytes
            str = str + ChrW(part)
        Next
        Return str
    End Function
    Public Function GetSizeStr(ByVal size As Long, Optional ByVal decimals As Integer = 0, Optional ByVal base As Long = 1024, Optional ByVal nodecimals As Boolean = False) As String
        Dim sz
        If decimals > 0 Then
            Dim sz2 As Double = CDbl(size)
            sz = sz2
        Else
            Dim sz2 As Long = size
            sz = sz2
        End If
        Dim units() As String = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "XB", "WB", "VB"}
        Dim unit As Integer = 0
redo:
        If sz >= base Then
            sz = sz / base
            unit = unit + 1
            If decimals > 0 Then sz = round(sz, decimals)
            GoTo redo
        End If
        If unit > UBound(units) Then Throw New Exception("Size too large.")
        If Not nodecimals Then
            Return sz.ToString & " " & units(unit)
        Else
            Return CInt(sz).ToString & " " & units(unit)
        End If
    End Function
    Public Function round(ByVal number As Double, ByVal decimals As Integer) As Double
        Return CDbl(CInt(number * (10 ^ decimals))) / CDbl(10 ^ decimals)
    End Function
    Dim DoECounter As ULong = 0 'Event Counter as long
    Public Function GetFileIntegrity(ByVal MFTSector As ULong, ByVal BytesPerSector As Integer, ByVal BytesPerFileRecordSegment As Integer) As String
        Dim Bytes = dDive.ReadSectors(MFTSector, (BytesPerFileRecordSegment / BytesPerSector))
        Dim baseaddr = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
        While Bytes(baseaddr) <> &H80
            baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        End While
        Dim Length As ULong = 0
        Dim LenLen As Byte = 0
        Dim Offset As ULong = 0
        Dim OffLen As Byte = 0
        Dim FileSize As ULong = 0
        Dim GoodClusters As ULong = 0
        DoECounter = 0
        If MergeToInt(Bytes, baseaddr + &HE, baseaddr + &HF) = 1 Then
            Return getValue("excellent")
        End If
        baseaddr = baseaddr + &H40
        While Bytes(baseaddr) > 0
            LenLen = Bytes(baseaddr) And &HF
            OffLen = (Bytes(baseaddr) And &HF0) / &H10
            Length = MergeToInt(Bytes, baseaddr + 1, baseaddr + LenLen)
            Offset = Offset + MergeToInt(Bytes, baseaddr + 1 + LenLen, baseaddr + LenLen + OffLen)
            FileSize = FileSize + Length
            If Length > 2 ^ 20 Then Length = 1
            For Check = Offset To Offset + Length - 1
                If Not GetBitmapClusterAllocation(Check) Then GoodClusters = GoodClusters + 1
                DoECounter = DoECounter + 1
                If DoECounter >= 100000 Then
                    Application.DoEvents()
                    DoECounter = 0
                End If
            Next
            baseaddr = baseaddr + (1 + LenLen + OffLen)
        End While
        Dim Percent = (GoodClusters * 100) \ FileSize
        If Percent = 100 Then Return getValue("excellent")
        If Percent >= 80 And Percent <= 99 Then Return getValue("good")
        If Percent >= 60 And Percent <= 79 Then Return getValue("ok")
        If Percent >= 40 And Percent <= 59 Then Return getValue("bad")
        If Percent >= 1 And Percent >= 39 Then Return getValue("horrible")
        If Percent = 0 Then Return getValue("overwritten")
        Return getValue("unknown")
    End Function
    Private Function GetBitmapClusterAllocation(ByVal Cluster As ULong) As Boolean
        Return ((Bitmap(Cluster \ 8) And (2 ^ (Cluster Mod 8))) / (2 ^ (Cluster Mod 8))) = 1
    End Function
    Private Function ReadBitmap(ByVal Drive As String) As Byte()
        Dim RBM As DirectDriveIO 'New DirectDriveIO(Drive)
        Drive = Drive.TrimEnd("\")
        Try
            RBM = New DirectDriveIO(Drive & "\")
        Catch
            MsgBox("Could not open drive.", MsgBoxStyle.Critical, "ERROR")
            Exit Function
        End Try
        Dim diskhandle = CreateFile("\\?\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
        If diskhandle = 0 Then
            diskhandle = CreateFile("\\.\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            If diskhandle = 0 Then
                MsgBox("Could not access drive.", MsgBoxStyle.Critical, "ERROR")
                Exit Function
            End If
        End If
        Dim FSCTL_GET_NFTS_VOLUME_DATA = CTL_CODE(FILE_DEVICE.FILE_DEVICE_FILE_SYSTEM, 25, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim buffer As NTFS_VOLUME_DATA_BUFFER
        DeviceIoControlNTFS(diskhandle, FSCTL_GET_NFTS_VOLUME_DATA, 0, 0, buffer, SizeOf(buffer), 0, 0)
        CloseHandle(diskhandle)
        Dim MFTAddress As Long = buffer.MftStartLcn * CLng(buffer.BytesPerCluster / buffer.BytesPerSector)
        Dim MFTEntrySize As Integer = buffer.BytesPerFileRecordSegment / buffer.BytesPerSector
        Dim BitmapBase As Long = MFTAddress + (MFTEntrySize * 6)
        Dim Bytes = RBM.ReadSectors(BitmapBase, MFTEntrySize)
        Dim baseaddr = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
        baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        'baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
        'Read the data runs 0x40
        baseaddr = baseaddr + &H40
        Dim Length As ULong = 0
        Dim LenLen As Byte = 0
        Dim Offset As ULong = 0
        Dim OffLen As Byte = 0
        Dim TempBytes() As Byte
        Dim BitmapBytes() As Byte
        LenLen = Bytes(baseaddr) And &HF
        OffLen = (Bytes(baseaddr) And &HF0) / &H10
        Length = MergeToInt(Bytes, baseaddr + 1, baseaddr + LenLen)
        Offset = MergeToInt(Bytes, baseaddr + 1 + LenLen, baseaddr + LenLen + OffLen)
        BitmapBytes = RBM.ReadSectors(Offset * (buffer.BytesPerCluster / buffer.BytesPerSector), Length * (buffer.BytesPerCluster / buffer.BytesPerSector))
        baseaddr = baseaddr + (1 + LenLen + OffLen)
        While Bytes(baseaddr) > 0
            LenLen = Bytes(baseaddr) And &HF
            OffLen = (Bytes(baseaddr) And &HF0) / &H10
            Length = MergeToInt(Bytes, baseaddr + 1, baseaddr + LenLen)
            Offset = Offset + MergeToInt(Bytes, baseaddr + 1 + LenLen, baseaddr + LenLen + OffLen)
            TempBytes = RBM.ReadSectors(Offset * (buffer.BytesPerCluster / buffer.BytesPerSector), Length * (buffer.BytesPerCluster / buffer.BytesPerSector))
            BitmapBytes = MergeByteArrays(BitmapBytes, TempBytes)
            baseaddr = baseaddr + (1 + LenLen + OffLen)
        End While
        Return BitmapBytes
    End Function
    Private Function MergeByteArrays(ByVal a As Byte(), ByVal b As Byte()) As Byte()
        Dim c(a.Count + b.Count - 1) As Byte
        For aa = 0 To UBound(a)
            c(aa) = a(aa)
        Next
        For ab = UBound(a) + 1 To UBound(c)
            c(ab) = b(ab - (UBound(a) + 1))
        Next
        Return c
    End Function
    'Private Function ByteArrayToBitArray(ByVal ByteArray As Byte()) As BitArray
    '    Dim BitA As New BitArray(ByteArray.Count * 8)
    '    Dim Counter As ULong = 0
    '    For Each b In ByteArray
    '        For c = 0 To 7
    '            BitA(Counter + c) = (b And (2 ^ c)) / (2 ^ c)
    '        Next c
    '        Counter = Counter + 8
    '    Next
    '    Return BitA
    'End Function
    Public Function GetImageFormat(ByVal Img As Image) As String
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp) Then Return "BMP"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf) Then Return "EMF"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif) Then Return "EXIF"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif) Then Return "GIF"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon) Then Return "ICON"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) Then Return "JPEG"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp) Then Return "MEMORYBMP"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png) Then Return "PNG"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff) Then Return "TIFF"
        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Wmf) Then Return "WMF"
        Return "UNKNOWN"
    End Function
    Public Function RemoveBadChars(ByVal Str As String) As String
        For rep = 0 To 31
            'Str.Replace(Chr(rep), "")
            If rep <> 9 And rep <> 10 And rep <> 13 Then Str = Str.Replace(Chr(rep), "")
        Next
        Str = Str.Replace(Chr(127), "")
        'Str.Replace(Chr(129), "")
        'Str.Replace(Chr(141), "")
        'Str.Replace(Chr(143), "")
        'Str.Replace(Chr(144), "")
        'Str.Replace(Chr(157), "")
        Return Str
    End Function
#End Region
#End Region
    Public Structure NonResidentData
        Dim AttributeType As Int32 '0x00            Attribute Type Identifier
        Dim AttributeLength As Int32 '0x04          Size of the attribute structure
        Dim NonResidentFlag As Byte '0x08           Non resident data flag
        Dim NameLength As Byte '0x09                Length of attribute name
        Dim OffsetToName As UInt16 '0x0A            Offset to attribute name
        Dim Flags As UInt16 '0x0C                   Attribute Flags
        Dim AttributeId As UInt16 '0x0E             Attribute Identifier
        Dim StartingVCN As UInt64 '0x10             First cluster of file
        Dim LastVCN As UInt64 '0x18                 Last cluster of file
        Dim DataRunOffset As UInt16 '0x20           Offset to data runs
        Dim CompressionSizeUnit As UInt16 '0x22     ?
        Dim Padding As Int32 '0x24                  Padding
        Dim AttributeSize As UInt64 '0x28           File Size
        Dim RealAttributeSize As UInt64 '0x30       Allocated file size
        Dim InitialisedStreamSize As UInt64 '0x38   Allocated file size
    End Structure
    'JPEGS start with FF D8 FF E0 00 10 4A 46 49 46 and end with FF D9
    Dim JPEGSTART() As Byte = {&HFF, &HD8, &HFF, &HE0, &H0, &H10, &H4A, &H46, &H49, &H46}
    Dim JPEGEND() As Byte = {&HFF, &HD9}
    Dim JPEGENDSTR As String = System.Text.UTF8Encoding.UTF8.GetString(JPEGEND)
    'BMPS start with 42 4D ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? where 0x02-0x05 is the bitmap size in bytes
    'PNGS start with 89 50 4E 47 0D 0A 1A 0A and end with 49 45 4E 44 AE 42 60 82
    'GIFS start with 47 49 46 38 39 61 and end with 00 3B
    'SWFS start with "CWS"
    Dim dDive As DirectDriveIO
    Dim LastDrive As String
    Dim Bitmap() As Byte
    Dim Preveiw As ULong = ULong.MinValue

#Region "data"
    Public NumberOfEntries As Long
    Dim DBytesPerCluster As Integer
    Dim DBytesPerSector As Integer
    Dim latesetDrive As IO.DriveInfo
    Dim deletedFiles As Long
    Dim existingFiles As Long
    Dim scan As Boolean
    Dim cExcellent As Long
    Dim cGood As Long
    Dim cOK As Long
    Dim cBad As Long
    Dim cHorrible As Long
    Dim cOverwritten As Long
    Dim cUnknown As Long

    Dim cPicture As Long
    Dim cText As Long
    Dim cDocument As Long
    Dim cProgram As Long
    Dim cMusic As Long
    Dim cVideo As Long

    Dim cSize1 As Long
    Dim cSize2 As Long
    Dim cSize3 As Long
    Dim cSize4 As Long
    Dim cSize5 As Long
    Dim cSize6 As Long
    Dim cSize7 As Long
    Dim cSize8 As Long
    Dim filterStringFileIntegrity As String
    Dim Startup As Boolean
    Dim filter As String
    Dim results As Long
    Dim currentDrive As String



    '0 - 50 KB
    '50 - 100 KB
    '100 - 500 KB
    '500 - 1500 KB
    '1,5 - 5 MB
    '5 - 500 MB
    '500 - 1000 MB
    '>1 GB


    'Dim fileending As New ArrayList


    Public language As String = CultureInfo.CurrentCulture.ToString.ToUpper.Substring(0, 2)

#End Region




#Region "Languages"

    Public rm As Resources.ResourceManager

    Private Property CultureInfo As CultureInfo
    Public Property FolderBrowserDialog1 As Object

    Function getValue(ByVal strValue As String) As String
        Dim strLanguage As String

        strLanguage = language

        If strLanguage = "DE" Then
            rm = My.Resources.German.ResourceManager
        Else
            rm = My.Resources.English.ResourceManager
        End If

        Return rm.GetString(strValue)
    End Function


    Private Sub Form1_Load2(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'MessageBox.Show(getValue("Hello"))
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Picture", CheckState.Checked)

        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Text", System.Windows.Forms.CheckState.Checked)
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Ducument", System.Windows.Forms.CheckState.Checked)
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Program", System.Windows.Forms.CheckState.Checked)
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Music", System.Windows.Forms.CheckState.Checked)
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Video", System.Windows.Forms.CheckState.Checked)
        'RepositoryItemCheckedComboBoxEdit4.Items.Add("Unknown", System.Windows.Forms.CheckState.Checked)
        'combobox.Caption = "Select Filter"
        'RepositoryItemCheckedComboBoxEdit4.AllowMultiSelect = True
        'RepositoryItemCheckedComboBoxEdit4.SelectAllItemVisible = False
        'RepositoryItemCheckedComboBoxEdit4.Items.ToList().ForEach((Sub(i) i.CheckState = CheckState.Checked))
        'combobox.EditValue = RepositoryItemCheckedComboBoxEdit1.GetCheckedItems()
        Startup = True

        Dim asm As System.Reflection.Assembly = GetType(DevExpress.UserSkins.GRUENMetropolis).Assembly

        DevExpress.Skins.SkinManager.Default.RegisterAssembly(asm)


        DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "Metropolis GRUEN"

        filterStringFileIntegrity = ""

        fbPicture.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbText.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbDocument.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbMusicVideo.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbUnknown1.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbProgram.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far

        fbExcellent.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbGood.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbOK.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbBadHorrible.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbUnknown.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far
        fbOverwritten.CaptionAlignment = DevExpress.Utils.HorzAlignment.Far


        refreshLanguage()

        For Each ctrl As Control In Me.Controls
            HelpProvider1.SetShowHelp(ctrl, True)
        Next



        ''IO.File.WriteAllBytes("c:\temp\manual.pdf", My.Resources.Manual_german)
        ''HelpProvider1.HelpNamespace = "c:\temp\manual.pdf"
        'DevExpress.Skins.SkinManager.Default.RegisterAssembly(GetType(DevExpress.UserSkins.GRUENMetropolis).Assembly)
        'MessageBox.Show(DevExpress.Skins.SkinManager.DefaultSkinName)

    End Sub



    Private Sub refreshLanguage()
        ChartControl1.Series.Clear()
        ChartControl1.Titles.Clear()
        ChartControl2.Series.Clear()
        ChartControl2.Titles.Clear()
        ChartControl3.Series.Clear()
        ChartControl3.Titles.Clear()

        GridControl.DataSource = Nothing
        fbPicture.Caption = getValue("picture")
        fbText.Caption = getValue("text")
        fbProgram.Caption = getValue("program")
        fbDocument.Caption = getValue("document")
        fbMusicVideo.Caption = getValue("music") & "/" & getValue("video")

        fbUnknown1.Caption = getValue("unknown")

        fbFormat.Text = "2: " & getValue("filter") & ": " & getValue("Format")

        hOptions.Text = "1: " & getValue("options")
        hPP.Text = "5: " & getValue("print_export")
        bbiPrintPreview.Caption = getValue("print_preview")
        bbiScan.Caption = getValue("scan")
        bbiStop.Caption = getValue("stop")
        bbiRefresh.Caption = getValue("refresh")
        hRecover.Text = "4: " & getValue("recover")
        bbiRecover.Caption = getValue("recover")
        ribbonPage1.Text = getValue("home")
        RibbonPage2.Text = getValue("settings")
        cbLanguage.Caption = getValue("language")
        bsiRecordsCount.Caption = getValue("records") & " : 0"
        ProgressPanel1.Caption = getValue("scanning_progress")
        ProgressPanel1.Description = getValue("wait")
        ProgressPanel1.WaitAnimationType = DevExpress.Utils.Animation.WaitingAnimatorType.Ring
        btnStop.Text = getValue("stop")

        RepositoryItemComboBox2.Items.Clear()
        RepositoryItemComboBox2.Items.Add("Deutsch")
        RepositoryItemComboBox2.Items.Add("English")

        btnlanguage.Caption = getValue("set_language")
        RibbonPageGroup2.Text = "3: " & getValue("reset_filter")
        bbiReset.Caption = getValue("reset_filter")
        sysinfo1.Caption = getValue("scan_for_information")
        RibbonPageGroup6.Text = getValue("drive_information")
        RibbonPageGroup1.Text = getValue("pc_information")
        rpgLanguage.Text = getValue("language")
        HelpOption.Text = "6: " & getValue("help")
        getHelp.Caption = getValue("getHelp")
        TextEdit1.Text = getValue("current_drive")
        BinOption.Text = getValue("bin")
        bbiBin.Caption = getValue("bin")
        InfoOption.Text = getValue("info")
        getInfo.Caption = getValue("info")



    End Sub


#End Region



    Public Class File
        <Display(Name:="...")>
        Public Property File_Name() As String

        <Display(Name:="...")>
        Public Property File_Path() As String


        <Display(Name:="...")>
        Public Property Ending() As String

        <Display(Name:="...")>
        Public Property Format() As String

        <Display(Name:="...")>
        Public Property Size() As Double

        <Display(Name:="...")>
        Public Property MFT_Sector_Adress() As Double

        <Display(Name:="...")>
        Public Property Estimated_File_Integrity() As String


    End Class

    Dim dataSourceGUI As BindingList(Of File)

    Public Sub ScanForFiles(ByVal Drive As String) 'finsished
        'Start scan


        Drive = Drive.TrimEnd("\")
        Try
            Dim driveInfo = My.Computer.FileSystem.GetDriveInfo(Drive & "\")
            latesetDrive = driveInfo
            If Not driveInfo.IsReady Then
                MsgBox("An Error occurred", MsgBoxStyle.Critical, "ERROR")
                Exit Sub
            End If
            If driveInfo.DriveFormat <> "NTFS" Then
                MsgBox("The drive has to be formated as an NTFS", MsgBoxStyle.Critical, "ERROR")
                Exit Sub
            End If
            sysinfo1.Caption = getValue("drive_name") & ": " & driveInfo.Name
            sysinfo2.Caption = getValue("label") & ": " & driveInfo.VolumeLabel
            sysinfo3.Caption = getValue("size") & ": " & driveInfo.TotalSize
            If driveInfo.DriveType.ToString = "Fixed" Or driveInfo.DriveType.ToString = "Removable" Then
                sysinfo1a.Caption = getValue("type") & ": " & getValue(driveInfo.DriveType.ToString.ToLower)
            Else
                sysinfo1a.Caption = getValue("type") & ": " & driveInfo.DriveType.ToString
            End If

            dDive = New DirectDriveIO(Drive & "\")
        Catch
            MsgBox("Could not access drive. Check if the application was started with administrator privileges!", MsgBoxStyle.Critical, "ERROR")
            Exit Sub
        End Try
        Drive = Drive.TrimEnd("\")
        Dim dHandle = CreateFile("\\?\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
        'create disk handle
        If dHandle = 0 Then
            dHandle = CreateFile("\\.\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            If dHandle = 0 Then
                MsgBox("While acessing the drive an error occurred.", MsgBoxStyle.Critical, "ERROR")
                Exit Sub
            End If
        End If

        cExcellent = 0
        cGood = 0
        cOK = 0
        cBad = 0
        cHorrible = 0
        cOverwritten = 0
        cUnknown = 0
        existingFiles = 0
        deletedFiles = 0
        cPicture = 0
        cText = 0
        cDocument = 0
        cProgram = 0
        cMusic = 0
        cVideo = 0

        cSize1 = 0
        cSize2 = 0
        cSize3 = 0
        cSize4 = 0
        cSize5 = 0
        cSize6 = 0
        cSize7 = 0
        cSize8 = 0


        Dim Type As Byte
        Dim Name As String
        Dim BaseAddresse As Long
        Dim Parent As Integer

        deletedFiles = 0
        existingFiles = 0
        bbiScan.Enabled = False
        bbiPrintPreview.Enabled = False
        'ComboBoxEdit1.Enabled = False
        LaufwerkBox.Enabled = False
        bbiRecover.Enabled = False
        bbiStop.Enabled = True
        scan = True
        'ProgressPanel1.Visible = True
        'ProgressBarControl1.Visible = True
        'btnStop.Visible = True
        PopupControlContainer1.Show()


        Dim FSCTL_GET_NFTS_VOLUME_DATA = CTL_CODE(FILE_DEVICE.FILE_DEVICE_FILE_SYSTEM, 25, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim buffer As NTFS_VOLUME_DATA_BUFFER
        DeviceIoControlNTFS(dHandle, FSCTL_GET_NFTS_VOLUME_DATA, 0, 0, buffer, SizeOf(buffer), 0, 0)
        CloseHandle(dHandle)
        LastDrive = Drive
        Dim MFTAdd As Long = buffer.MftStartLcn * CLng(buffer.BytesPerCluster / buffer.BytesPerSector) ' MFTadress is creating
        Dim MFTEntrySize As Integer = buffer.BytesPerFileRecordSegment / buffer.BytesPerSector
        NumberOfEntries = buffer.MftValidDataLength / MFTEntrySize
        Dim Bytes(buffer.BytesPerCluster) As Byte
        Dim CEBytes(MFTEntrySize * buffer.BytesPerSector) As Byte 'Curent Entry Bytes As Byte

        Dim CurrEntry As New STANDARD_MFT_ENTRY
        Dim BaseAddr As Long
        Dim DoEventsCounter As Integer = 0


        ProgressBarControl2.EditValue = 0
        ProgressBarControl2.Properties.Maximum = buffer.MftValidDataLength / buffer.BytesPerFileRecordSegment
        Bitmap = ReadBitmap(Drive)


        Dim BitmapBase As Long = MFTAdd + (MFTEntrySize * 0)
        Bytes = dDive.ReadSectors(BitmapBase, MFTEntrySize)
        BaseAddresse = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
        While Bytes(BaseAddresse) <> &H80
            BaseAddresse = BaseAddresse + MergeToInt(Bytes, BaseAddresse + &H4, BaseAddresse + &H7)
            'Add the length of the attribute to the base address to find the next attribute
        End While
        BaseAddresse = BaseAddresse + &H40
        Dim Length As ULong = 0
        Dim LenLen As Byte = 0
        Dim Path As String = ""
        Dim BaseAddress2 As ULong = 0
        Dim FileSize As ULong = 0
        Dim Offset As ULong = 0
        Dim OffLen As Byte = 0
        Dim PartNumber As Integer = 0
        Dim lC As Integer = 0 'counting variable
        Dim result As New BindingList(Of File)()
        Dim format As String = "Unknown"
        Dim counter As Integer = 0


        Dim strExcellent As String = getValue("excellent")
        Dim strGood As String = getValue("good")
        Dim strOK As String = getValue("ok")
        Dim strBad As String = getValue("bad")
        Dim strHorrible As String = getValue("horrible")
        Dim strUnknown As String = getValue("unknown")
        Dim strPicture As String = getValue("picture")
        Dim strText As String = getValue("text")
        Dim strDocument As String = getValue("document")
        Dim strProgram As String = getValue("program")
        Dim strMusic As String = getValue("music")
        Dim strVideo As String = getValue("video")
        Dim strRecords As String = getValue("records")
        Dim strScanningfile As String = getValue("scanning_file")
        Dim strOverwritten As String = getValue("overwritten")
        'PictureBox1.Image = Image.FromFile()



        DBytesPerCluster = buffer.BytesPerCluster
        DBytesPerSector = buffer.BytesPerSector

        While Bytes(BaseAddresse) > 0 And scan = True
            LenLen = Bytes(BaseAddresse) And &HF
            OffLen = (Bytes(BaseAddresse) And &HF0) / &H10
            Length = MergeToInt(Bytes, BaseAddresse + 1, BaseAddresse + LenLen)
            Offset = Offset + MergeToInt(Bytes, BaseAddresse + 1 + LenLen, BaseAddresse + LenLen + OffLen)

            For Record = 0 To ((Length * buffer.BytesPerCluster) / buffer.BytesPerFileRecordSegment) - 1
                If scan = True Then
                    DoEventsCounter = DoEventsCounter + 1
                    If DoEventsCounter >= 100 Then
                        Application.DoEvents()
                        DoEventsCounter = 0
                    End If

                    Try


                        Try
                            If (PartNumber + 1) >= (CEBytes.Count / buffer.BytesPerFileRecordSegment) Then
                                CEBytes = dDive.ReadSectors((Record * MFTEntrySize) + (Offset * (buffer.BytesPerCluster / buffer.BytesPerSector)), MFTEntrySize * 1024)
                                BaseAddress2 = 0
                                PartNumber = 0
                            Else

                                PartNumber = PartNumber + 1
                                BaseAddress2 = PartNumber * buffer.BytesPerFileRecordSegment
                            End If
                            If CEBytes(BaseAddress2) <> Asc("F") Then

                                GoTo PBplus
                            End If
                            Name = ""
                            Parent = 5
                            Path = ""
                            FileSize = 0
                            Type = CEBytes(&H16 + BaseAddress2)

                            If Type = MFT_ENTRY_FILE_TYPE_FLAGS.DeletedFile Then
                                deletedFiles += 1
                                'BaseAddress2 = BaseAddress2 + MergeToInt(CEBytes, &H14, &H15) 'The offset the the first attribute
                                BaseAddress2 = BaseAddress2 + MergeToInt(CEBytes, BaseAddress2 + &H14, BaseAddress2 + &H15)
                                BaseAddress2 = BaseAddress2 + MergeToInt(CEBytes, BaseAddress2 + &H4, BaseAddress2 + &H7)
                                Try
                                    Parent = MergeToInt(CEBytes, BaseAddress2 + &H18, BaseAddress2 + &H1D)
                                Catch
                                End Try
                                Try
                                    If FileSize = 0 Then FileSize = MergeToInt(CEBytes, BaseAddress2 + &H48, BaseAddress2 + &H4F)
                                    If FileSize > 2 ^ 30 Then FileSize = 0

                                Catch
                                End Try
                                Try
                                    Name = System.Text.UnicodeEncoding.Unicode.GetString(ByteArrayPart(CEBytes, BaseAddress2 + &H5A, (BaseAddress2 + &H5A) + ((2 * CEBytes(BaseAddress2 + &H58)) - 2)))
                                Catch
                                End Try
                                Try
                                    If Name.Contains("~") Then
                                        BaseAddress2 = BaseAddress2 + MergeToInt(CEBytes, BaseAddress2 + &H4, BaseAddress2 + &H7)
                                        If FileSize = 0 Then FileSize = MergeToInt(CEBytes, BaseAddress2 + &H48, BaseAddress2 + &H4F)
                                        If FileSize > 2 ^ 30 Then FileSize = 0

                                        Name = System.Text.UnicodeEncoding.Unicode.GetString(ByteArrayPart(CEBytes, BaseAddress2 + &H5A, (BaseAddress2 + &H5A) + ((2 * CEBytes(BaseAddress2 + &H58)) - 2)))
                                    End If
                                Catch
                                End Try
                                If Name.Length >= 75 Then Name = Mid(Name, 1, 74) & Mid(Name, 76, Name.Length - 75)

                                Try
                                    Path = GetFullPath2(Parent, MFTAdd, MFTEntrySize, buffer.BytesPerCluster) & "\" ' & Name
                                Catch
                                End Try

                                If FileSize = 0 Then
                                    Try
                                        lC = 0
                                        While CEBytes(BaseAddress2) <> &H80 And lC < 5
                                            lC = lC + 1
                                            BaseAddress2 = BaseAddress2 + MergeToInt(CEBytes, BaseAddress2 + &H4, BaseAddress2 + &H7)
                                        End While

                                        If MergeToInt(CEBytes, BaseAddress2 + &HE, BaseAddress2 + &HF) = 1 Then

                                            FileSize = MergeToInt(CEBytes, BaseAddress2 + &H10, BaseAddress2 + &H13)

                                        Else
                                            'FileSize = 0
                                            FileSize = MergeToInt(CEBytes, BaseAddress2 + &H30, BaseAddress2 + &H37)
                                        End If
                                        If FileSize > 2 ^ 30 Then FileSize = 0
                                    Catch
                                    End Try
                                End If
                                If FileSize > 0 Then

                                    Dim ending As String = Name.Substring(Name.LastIndexOf(".") + 1)
                                    If ending = Name Then
                                        'fileending.Add("Unknown")
                                        ending = strUnknown
                                    Else
                                        'fileending.Add(ending)
                                    End If



                                    If ending.ToLower = "jpg" Or ending.ToLower = "gif" Or ending.ToLower = "png" Or ending.ToLower = "tif" Or ending.ToLower = "bmp" Or ending.ToLower = "swf" Or ending.ToLower = "svg" Or ending.ToLower = "jpeg" Or ending.ToLower = "odt" Then
                                        format = strPicture
                                        cPicture += 1
                                    ElseIf ending.ToLower = "txt" Or ending.ToLower = "json" Or ending.ToLower = "html" Or ending.ToLower = "css" Or ending.ToLower = "js" Or ending.ToLower = "rtf" Then
                                        format = strText
                                        cText += 1
                                    ElseIf ending.ToLower = "pdf" Or ending.ToLower = "xlsx" Or ending.ToLower = "xlsm" Or ending.ToLower = "xlsb" Or ending.ToLower = "xlam" Or ending.ToLower = "xltx" Or ending.ToLower = "xlk" Or ending.ToLower = "xll" Or ending.ToLower = "xls" Or ending.ToLower = "doc" Or ending.ToLower = "docx" Or ending.ToLower = "dot" Or ending.ToLower = "dotx" Or ending.ToLower = "mdb" Or ending.ToLower = "accdb" Or ending.ToLower = "ppt" Or ending.ToLower = "pptx" Or ending.ToLower = "zip" Or ending.ToLower = "rar" Or ending.ToLower = "xml" Or ending.ToLower = "xps" Or ending.ToLower = "csv" Or ending.ToLower = "ppt" Then
                                        format = strDocument
                                        cDocument += 1
                                    ElseIf ending.ToLower = "c" Or ending.ToLower = "exe" Or ending.ToLower = "c++" Or ending.ToLower = "dll" Or ending.ToLower = "ini" Or ending.ToLower = "jar" Or ending.ToLower = "java" Or ending.ToLower = "inf" Or ending.ToLower = "sys" Or ending.ToLower = "tmp" Then
                                        format = strProgram
                                        cProgram += 1
                                    ElseIf ending.ToUpper = "WAV" Or ending.ToUpper = "MP3" Or ending.ToUpper = "WMA" Or ending.ToUpper = "AAC" Or ending.ToUpper = "OGG" Or ending.ToUpper = "FLAC" Or ending.ToUpper = "RM" Then
                                        format = strMusic
                                        cMusic += 1
                                    ElseIf ending.ToLower = "mpg" Or ending.ToLower = "mpeg" Or ending.ToLower = "mpg" Or ending.ToLower = "vob" Or ending.ToLower = "m2p" Or ending.ToLower = "ts" Or ending.ToLower = "mp4" Or ending.ToLower = "mov" Or ending.ToLower = "avi" Or ending.ToLower = "wmv" Or ending.ToLower = "asf" Or ending.ToLower = "mkv" Or ending.ToLower = "webm" Or ending.ToLower = "flv" Or ending.ToLower = "3gp" Then
                                        format = strVideo
                                        cVideo += 1
                                    Else
                                        format = strUnknown
                                    End If

                                    Dim skip As Boolean = False
                                    Dim Integrity As String = strUnknown
                                    Try
                                        Integrity = GetFileIntegrity((Record * MFTEntrySize) + (Offset * (buffer.BytesPerCluster / buffer.BytesPerSector)), buffer.BytesPerSector, buffer.BytesPerFileRecordSegment)
                                    Catch ex As Exception
                                        Integrity = strUnknown
                                    End Try


                                    If Integrity = strExcellent And fbExcellent.EditValue = False Then
                                        skip = True
                                    ElseIf Integrity = strGood And fbGood.EditValue = False Then
                                        skip = True
                                    ElseIf Integrity = strOK And fbOK.EditValue = False Then
                                        skip = True
                                    ElseIf Integrity = strBad Or Integrity = strHorrible And fbBadHorrible.EditValue = False Then
                                        skip = True
                                    ElseIf Integrity = strOverwritten And fbOverwritten.EditValue = False Then
                                        skip = True
                                    ElseIf Integrity = strUnknown And fbUnknown.EditValue = False Then
                                        skip = True
                                    End If

                                    If Not skip Then

                                        Dim temp As Long = FileSize / 1000


                                        If temp >= 0 And temp <= 50 Then
                                            cSize1 += 1
                                        ElseIf temp >= 50 And temp <= 100 Then
                                            cSize2 += 1
                                        ElseIf temp >= 100 And temp <= 500 Then
                                            cSize3 += 1
                                        ElseIf temp >= 500 And temp <= 1500 Then
                                            cSize4 += 1
                                        ElseIf temp >= 1500 And temp <= 5000 Then
                                            cSize5 += 1
                                        ElseIf temp >= 5000 And temp <= 5000000 Then
                                            cSize6 += 1
                                        ElseIf temp >= 5000000 And temp <= 1000000 Then
                                            cSize7 += 1
                                        ElseIf temp >= 1000000 Then
                                            cSize8 += 1
                                        End If



                                        Try



                                            result.Add(New File() With {.File_Name = Name, .File_Path = Path, .Ending = ending, .Format = format, .Size = FileSize, .MFT_Sector_Adress = (Record * MFTEntrySize) + (Offset * (buffer.BytesPerCluster / buffer.BytesPerSector)), .Estimated_File_Integrity = GetFileIntegrity((Record * MFTEntrySize) + (Offset * (buffer.BytesPerCluster / buffer.BytesPerSector)), buffer.BytesPerSector, buffer.BytesPerFileRecordSegment)})
                                            If Integrity = strExcellent Then
                                                cExcellent += 1
                                            ElseIf Integrity = strGood Then
                                                cGood += 1
                                            ElseIf Integrity = strOK Then
                                                cOK += 1
                                            ElseIf Integrity = strBad Then
                                                cBad += 1
                                            ElseIf Integrity = strHorrible Then
                                                cHorrible += 1
                                            ElseIf Integrity = strOverwritten Then
                                                cOverwritten += 1
                                            Else
                                                cUnknown += 1
                                            End If


                                        Catch
                                            result.Add(New File() With {.File_Name = Name, .File_Path = Path, .Ending = ending, .Size = FileSize, .MFT_Sector_Adress = (Record * MFTEntrySize) + (Offset * (buffer.BytesPerCluster / buffer.BytesPerSector)), .Estimated_File_Integrity = strUnknown})
                                            cUnknown += 1
                                        End Try



                                        dataSourceGUI = result
                                        GridControl.DataSource = dataSourceGUI
                                        counter = dataSourceGUI.Count

                                        bsiRecordsCount.Caption = strRecords & " : " & dataSourceGUI.Count & "        " & strScanningfile & ": " & existingFiles & " / " & buffer.MftValidDataLength / buffer.BytesPerFileRecordSegment & " (" & CInt((existingFiles / (buffer.MftValidDataLength / buffer.BytesPerFileRecordSegment)) * 100) & "%)"
                                        ''System.Threading.Thread.Sleep(5)

                                    Else

                                    End If


                                End If
                                deletedFiles += 1
                                GoTo PBplus
                            Else

                                existingFiles += 1
                                bsiRecordsCount.Caption = strRecords & " : " & counter & "        " & strScanningfile & ": " & existingFiles & " / " & buffer.MftValidDataLength / buffer.BytesPerFileRecordSegment & " (" & CInt((existingFiles / (buffer.MftValidDataLength / buffer.BytesPerFileRecordSegment)) * 100) & "%)"

                            End If

                        Catch
                        End Try
PBplus:
                        Try
                            ProgressBarControl2.EditValue += 1
                        Catch
                        End Try


                    Catch ex As Exception
                    End Try
                End If
            Next Record
            BaseAddresse = BaseAddresse + (1 + LenLen + OffLen)
        End While
        ProgressBarControl2.EditValue = 0
        bbiScan.Enabled = True
        bbiPrintPreview.Enabled = True
        'ComboBoxEdit1.Enabled = True
        LaufwerkBox.Enabled = True
        bbiRecover.Enabled = True
        bbiStop.Enabled = False
        'ProgressPanel1.Visible = False
        'ProgressBarControl1.Visible = False
        'btnStop.Visible = False


        results = counter
        If counter > 0 Then
            dataSourceGUI = result

            Dim gv As GridColumn = GridView1.Columns.ColumnByName("colFormat")
            gv.Caption = getValue("Format")
            Dim gv1 As GridColumn = GridView1.Columns.ColumnByName("colFile_Name")
            gv1.Caption = getValue("File_Name")
            Dim gv2 As GridColumn = GridView1.Columns.ColumnByName("colFile_Path")
            gv2.Caption = getValue("File_Path")
            Dim gv3 As GridColumn = GridView1.Columns.ColumnByName("colEnding")
            gv3.Caption = getValue("Ending")
            Dim gv4 As GridColumn = GridView1.Columns.ColumnByName("colSize")
            gv4.Caption = getValue("size")
            Dim gv5 As GridColumn = GridView1.Columns.ColumnByName("colMFT_Sector_Adress")
            gv5.Caption = getValue("MFT_Sector_Adress")
            Dim gv6 As GridColumn = GridView1.Columns.ColumnByName("colEstimated_File_Integrity")
            gv6.Caption = getValue("Estimated_File_Integrity")
            GridControl.DataSource = dataSourceGUI
            bsiRecordsCount.Caption = strRecords & " : " & dataSourceGUI.Count
        Else
            bsiRecordsCount.Caption = strRecords & " : 0"
        End If


        If scan = False Then
            MessageBox.Show(getValue("scan_stop"))
        End If
        scan = False
        'MessageBox.Show("Scan done")

        filterChange()

        If GridView1.ActiveFilterString = "" Then

        Else
            GridView1.ActiveFilterString = ""
            filterStringFileIntegrity = ""
            ChartControl1.ClearSelection(True)
            fbPicture.EditValue = True
            fbText.EditValue = True
            fbProgram.EditValue = True
            fbDocument.EditValue = True
            fbUnknown1.EditValue = True
            fbMusicVideo.EditValue = True
            filterChange()

        End If

        c1()
        c2()
        c3()
        PopupControlContainer1.Hide()


    End Sub




    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Startup = True
        RefreshAllDrives()

        sysinfo4.Caption = "OS Platform: " & My.Computer.Info.OSPlatform
        sysinfo5.Caption = "OS Name: " & My.Computer.Info.OSFullName
        sysinfo6.Caption = "Language: " & My.Computer.Info.InstalledUICulture.DisplayName

        'ProgressPanel1.Visible = False
        'ProgressBarControl1.Visible = False
        GridView1.OptionsSelection.MultiSelect = True
        GridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect
        fbPicture.EditValue = True
        fbText.EditValue = True
        fbDocument.EditValue = True
        fbProgram.EditValue = True
        fbMusicVideo.EditValue = True
        fbUnknown1.EditValue = True
        fbExcellent.EditValue = True
        fbGood.EditValue = True
        fbOK.EditValue = True
        fbBadHorrible.EditValue = True
        fbOverwritten.EditValue = True
        fbUnknown.EditValue = True
        fbUnknown1.EditValue = True
        'btnStop.Visible = False
        PopupControlContainer1.Hide()

        bbiStop.Enabled = False
        scan = False

        Startup = False
    End Sub



    'Ctr + K , Ctr + C
    'Ctr + k , Ctr + u
    '    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
    '        Preveiw = Preveiw + 1
    '        If Preveiw >= ULong.MaxValue - 1 Then Preveiw = ULong.MinValue
    '        Dim ThisPreveiw = Preveiw
    '        Try
    '            'If ListView1.SelectedItems(0).Text.Contains(".jpg") Or ListView1.SelectedItems(0).Text.Contains(".gif") Or ListView1.SelectedItems(0).Text.Contains(".png") Or ListView1.SelectedItems(0).Text.Contains(".tiff") Or ListView1.SelectedItems(0).Text.Contains(".jpeg") Or ListView1.SelectedItems(0).Text.Contains(".bmp") Then
    '            If ULong.Parse(ListView1.SelectedItems(0).SubItems(2).Text) < 2 ^ 22 Then
    '                Try
    '                    Dim Drive = LastDrive
    '                    'Dim Drive = Strings.Left(ComboBox1.SelectedText, 2)
    '                    Dim MFTSector = ListView1.SelectedItems(0).SubItems(3).Text
    '                    Dim FileLength = ListView1.SelectedItems(0).SubItems(2).Text
    '                    Dim RBM As DirectDriveIO 'New DirectDriveIO(Drive)
    '                    Drive = Drive.TrimEnd("\")
    '                    Try
    '                        RBM = New DirectDriveIO(Drive & "\")
    '                    Catch
    '                        Exit Sub
    '                    End Try
    '                    Dim diskhandle = CreateFile("\\?\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
    '                    If diskhandle = 0 Then
    '                        diskhandle = CreateFile("\\.\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
    '                        If diskhandle = 0 Then
    '                            Exit Sub
    '                        End If
    '                    End If
    '                    Dim FSCTL_GET_NFTS_VOLUME_DATA = CTL_CODE(FILE_DEVICE.FILE_DEVICE_FILE_SYSTEM, 25, METHOD_BUFFERED, FILE_ANY_ACCESS)
    '                    Dim buffer As NTFS_VOLUME_DATA_BUFFER
    '                    DeviceIoControlNTFS(diskhandle, FSCTL_GET_NFTS_VOLUME_DATA, 0, 0, buffer, SizeOf(buffer), 0, 0)
    '                    CloseHandle(diskhandle)
    '                    Dim Bytes = RBM.ReadSectors(MFTSector, (buffer.BytesPerFileRecordSegment / buffer.BytesPerSector))
    '                    Dim baseaddr = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
    '                    While Bytes(baseaddr) <> &H80
    '                        baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
    '                    End While

    '                    Dim Length As ULong = 0
    '                    Dim LenLen As Byte = 0
    '                    Dim Offset As ULong = 0
    '                    Dim OffLen As Byte = 0
    '                    'Dim ReadSize As ULong = 0
    '                    Dim TempFile As New System.IO.MemoryStream
    '                    'ToolStripStatusLabel1.Text = "Reading File..."
    '                    If MergeToInt(Bytes, baseaddr + &HE, baseaddr + &HF) = 1 And (buffer.BytesPerFileRecordSegment - (baseaddr + &H18 + FileLength)) > 0 Then
    '                        Try
    '                            TempFile.Write(ByteArrayPart(Bytes, baseaddr + &H18, baseaddr + &H18 + FileLength), 0, FileLength)
    '                        Catch
    '                        End Try
    '                        GoTo CleanUp
    '                    End If
    '                    baseaddr = baseaddr + &H40
    '                    While Bytes(baseaddr) > 0
    '                        LenLen = Bytes(baseaddr) And &HF
    '                        OffLen = (Bytes(baseaddr) And &HF0) / &H10
    '                        Length = MergeToInt(Bytes, baseaddr + 1, baseaddr + LenLen)
    '                        Offset = Offset + MergeToInt(Bytes, baseaddr + 1 + LenLen, baseaddr + LenLen + OffLen)
    '                        TempFile.Write(RBM.ReadSectors(Offset * (buffer.BytesPerCluster / buffer.BytesPerSector), Length * (buffer.BytesPerCluster / buffer.BytesPerSector)), 0, Length * buffer.BytesPerCluster)
    '                        If TempFile.Length > FileLength Then Exit While
    '                        'ReadSize = ReadSize + Length
    '                        'ToolStripStatusLabel1.Text = "Reading file " & ListView1.SelectedItems(0).Text & " " & round((ReadSize * buffer.BytesPerCluster * 100) / FileLength, 1) & "%..."
    '                        baseaddr = baseaddr + (1 + LenLen + OffLen)
    '                        Application.DoEvents()
    '                    End While
    'CleanUp:
    '                    Application.DoEvents()
    '                    TempFile.SetLength(FileLength)
    '                    Application.DoEvents()
    '                    Try
    '                        Dim Img As Bitmap
    '                        Try
    '                            Img = Image.FromStream(TempFile, False, True)
    '                            Application.DoEvents()
    '                        Catch
    '                            Img = Image.FromStream(TempFile, False, False)
    '                            Application.DoEvents()
    '                            Img = Img.GetThumbnailImage(Img.Width, Img.Height, Nothing, 0)
    '                            Application.DoEvents()
    '                        End Try
    '                        'Img.SaveAdd
    '                        'Me.Text = GetImageFormat(Img)
    '                        If Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif) Then Img = Img.GetThumbnailImage(Img.Width, Img.Height, Nothing, 0)
    '                        Application.DoEvents()
    '                        Img.SetResolution(300, 300)
    '                        'If Img.Width > 1000 Then
    '                        'Img = Img.GetThumbnailImage(1000, Img.Height / (Img.Width / 1000), Nothing, 0)
    '                        'End If
    '                        'If Img.Height > 1000 Then
    '                        'Img = Img.GetThumbnailImage(Img.Width / (Img.Height / 1000), 1000, Nothing, 0)
    '                        'End If
    '                        If ThisPreveiw = Preveiw Then
    '                            Try
    '                                PictureBox1.Image = Img
    '                            Catch
    '                            End Try
    '                            Application.DoEvents()
    '                            'Img.SelectActiveFrame(New System.Drawing.Imaging.FrameDimension(Img.FrameDimensionsList(0)), 0)
    '                            'PictureBox1.Image = Image.FromStream(TempFile, False, True)
    '                            PictureBox1.Show()
    '                            TextBox1.Hide()
    '                            Application.DoEvents()
    '                        End If
    '                    Catch
    '                        'PictureBox1.Image = PictureBox1.ErrorImage
    '                        TempFile.Seek(0, IO.SeekOrigin.Begin)
    '                        Dim Data(TempFile.Length - 1) As Byte
    '                        Application.DoEvents()
    '                        TempFile.Read(Data, 0, TempFile.Length)
    '                        'Application.DoEvents()
    '                        'TextBox1.Text = Convert.ToBase64String(Data)
    '                        Application.DoEvents()
    '                        'TextBox1.Text = System.Text.ASCIIEncoding.ASCII.GetString(Data).Replace(Chr(0), "")
    '                        If ThisPreveiw = Preveiw Then
    '                            TextBox1.Text = RemoveBadChars(System.Text.ASCIIEncoding.ASCII.GetString(Data))
    '                            Application.DoEvents()
    '                            TextBox1.Show()
    '                            PictureBox1.Hide()
    '                            Application.DoEvents()
    '                        End If
    '                    End Try
    '                    TempFile.Close()
    '                TempFile.Dispose()
    '                Application.DoEvents()
    '                'My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\temp.jpg", FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
    '            Catch
    '            End Try
    '        Else
    '            If ThisPreveiw = Preveiw Then
    '                TextBox1.Text = "File too large to preview."
    '                TextBox1.Show()
    '                PictureBox1.Hide()
    '                Application.DoEvents()
    '            End If
    '        End If
    '    Catch
    '    End Try
    'End Sub

    Private Sub ComboBox1_DropDown(ByVal sender As Object, ByVal e As System.EventArgs)
        RefreshAllDrives()
    End Sub

    Public Sub RefreshAllDrives()

        ''ComboBoxEdit1.Properties.Items.Clear()
        RepositoryItemComboBox3.Items.Clear()

        For Each Drive In My.Computer.FileSystem.Drives
            Try
                If Drive.DriveFormat = "NTFS" Then
                    ''ComboBoxEdit1.Properties.Items.Add(Drive.Name & " (" & " " & Drive.DriveFormat & ")") ''GetSizeStr(Drive.TotalSize)) &
                End If
            Catch
            End Try
        Next

        Try
            ''ComboBoxEdit1.SelectedIndex = 0
            ''BarEditItem3.EditValue = ComboBoxEdit1.Properties.Items(ComboBoxEdit1.SelectedIndex)

        Catch
        End Try
    End Sub



    Private Sub bbiPrintPreview_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles bbiPrintPreview.ItemClick
        GridControl.ShowRibbonPrintPreview()
    End Sub

    Private Sub bbiScan_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiScan.ItemClick

        Dim myModalForm As Drive = New Drive()
        Dim dialogResult As DialogResult
        Dim selectedText As String = ""
        dialogResult = myModalForm.ShowDialog(Me)
        If dialogResult = System.Windows.Forms.DialogResult.OK Then
            selectedText = myModalForm.GetSelectedText()
        Else
            ' Perform default actions here.
        End If
        myModalForm.Dispose()
        currentDrive = selectedText

        TextEdit1.Text = getValue("current_drive") + " " + currentDrive

        Dim drive As String = Strings.Left(currentDrive, 3) ''ComboBoxEdit1.SelectedItem
        ScanForFiles(drive)
    End Sub

    Private Sub bbiStop_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiStop.ItemClick
        PopupControlContainer1.Hide()
        scan = False
    End Sub

    Private Sub bbiRefresh_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiRefresh.ItemClick
        GridView1.ActiveFilterString = filter
        c1()
        c2()
        c3()


    End Sub



    'Public Sub s1ToFileIntegrity()
    '    Chart1.Series("s1").Points.Clear()
    '    Chart1.Titles.Clear()
    '    Chart1.Titles.Add("FileIntegrity")
    '    Chart1.Series("s1").Points.AddXY("Excellent", cExcellent)
    '    Chart1.Series("s1").Points.AddXY("Good", cGood)
    '    Chart1.Series("s1").Points.AddXY("OK", cOK)
    '    Chart1.Series("s1").Points.AddXY("Bad", cBad)
    '    Chart1.Series("s1").Points.AddXY("Horrible", cHorrible)
    '    Chart1.Series("s1").Points.AddXY("Overwritten", cOverwritten)
    '    Chart1.Series("s1").Points.AddXY("Unknown", cUnknown)



    'End Sub

    Public Sub c1()

        If results = 0 Then

        Else


            ChartControl1.Series.Clear()
            ChartControl1.Titles.Clear()

            Dim series1 As New Series("Pie Series 1", ViewType.Pie)


            series1.Points.Add(New SeriesPoint(getValue("excellent"), cExcellent + 0.000001))
            series1.Points.Add(New SeriesPoint(getValue("good"), cGood + 0.000002))
            series1.Points.Add(New SeriesPoint(getValue("ok"), cOK + 0.000003))
            series1.Points.Add(New SeriesPoint(getValue("bad"), cBad + 0.000004))
            series1.Points.Add(New SeriesPoint(getValue("horrible"), cHorrible + 0.000005))
            series1.Points.Add(New SeriesPoint(getValue("overwritten"), cOverwritten + 0.000006))



            series1.LegendTextPattern = "{A}: {VP:P0}"
            series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default
            series1.Label.TextPattern = "{A}: {VP:P0}"



            ChartControl1.Series.Add(series1)


            series1.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent
            series1.PointOptions.ValueNumericOptions.Precision = 0



            Dim chartTitle1 As New ChartTitle()

            chartTitle1.Text = getValue("Estimated_File_Integrity")
            ChartControl1.Titles.Add(chartTitle1)
            ChartControl1.Legend.Visible = True
            ChartControl1.SelectionMode = ElementSelectionMode.Single
            ChartControl1.SeriesSelectionMode = SeriesSelectionMode.Point

        End If

    End Sub



    Private Sub ChartControl_SelectedItemsChanged(ByVal sender As Object, ByVal e As SelectedItemsChangedEventArgs)

    End Sub

    Public Event SelectedItemsChanging As SelectedItemsChangingEventHandler



    Public Sub c2()


        If results = 0 Then

        Else


            ChartControl2.Series.Clear()
            ChartControl2.Titles.Clear()

            Dim series1 As New Series("Pie Series 1", ViewType.Pie)

            If cPicture = 0 And cText = 0 And cDocument = 0 And cProgram = 0 And cMusic = 0 And cVideo = 0 Then

            Else
                series1.Points.Add(New SeriesPoint(getValue("picture"), cPicture + 0.000001))
                series1.Points.Add(New SeriesPoint(getValue("text"), cText + 0.000002))
                series1.Points.Add(New SeriesPoint(getValue("document"), cDocument + 0.000003))
                series1.Points.Add(New SeriesPoint(getValue("program"), cProgram + 0.000004))
                series1.Points.Add(New SeriesPoint(getValue("music"), cMusic + 0.000005))
                series1.Points.Add(New SeriesPoint(getValue("video"), cVideo + 0.000006))
            End If



            series1.LegendTextPattern = "{A}: {VP:P0}"
                series1.Label.TextPattern = "{A}: {VP:P0}"

                series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default




                ChartControl2.Series.Add(series1)


                series1.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent
                series1.PointOptions.ValueNumericOptions.Precision = 0



                Dim chartTitle1 As New ChartTitle()
                chartTitle1.Text = getValue("file") & "-" & getValue("type")
                ChartControl2.Titles.Add(chartTitle1)
                ChartControl2.Legend.Visible = True
                ChartControl2.SelectionMode = ElementSelectionMode.Single
                ChartControl2.SeriesSelectionMode = SeriesSelectionMode.Point

            End If


    End Sub


    Public Sub c3()

        ChartControl3.Series.Clear()
        ChartControl3.Titles.Clear()

        ' Create an empty chart.
        Dim sideBySideBarChart As New ChartControl()


        Dim series1 As New Series("50 - 100 KB", ViewType.Bar)
        series1.Points.Add(New SeriesPoint("50 - 100 KB", cSize2))
        series1.Points.Add(New SeriesPoint("100 - 500 KB", cSize3))
        series1.Points.Add(New SeriesPoint("500 - 1500KB", cSize4))
        series1.Points.Add(New SeriesPoint("1,5 - 5 MB", cSize5))
        series1.Points.Add(New SeriesPoint("5 - 500 MB", cSize6))
        series1.Points.Add(New SeriesPoint("500 - 1000 MB", cSize7))
        series1.Points.Add(New SeriesPoint(">1 GB", cSize8))

        ''Dim series2 As New Series("100 - 500 KB", ViewType.Bar)
        ''series2.Points.Add(New SeriesPoint(getValue("size"), cSize3))

        ''Dim series3 As New Series("500 - 1500KB", ViewType.Bar)
        ''series3.Points.Add(New SeriesPoint(getValue("size"), cSize4))

        ''Dim series4 As New Series("1,5 - 5 MB", ViewType.Bar)
        ''series4.Points.Add(New SeriesPoint(getValue("size"), cSize5))

        ''Dim series5 As New Series("5 - 500 MB", ViewType.Bar)
        ''series5.Points.Add(New SeriesPoint(getValue("size"), cSize6))

        ''Dim series6 As New Series("500 - 1000 MB", ViewType.Bar)
        ''series6.Points.Add(New SeriesPoint(getValue("size"), cSize7))

        ''Dim series7 As New Series(">1 GB", ViewType.Bar)
        ''series7.Points.Add(New SeriesPoint(getValue("size"), cSize8))

        series1.LegendTextPattern = "{VP:P0}"

        series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default



        ' Add the series to the chart.
        ChartControl3.Series.Add(series1)
        ''ChartControl3.Series.Add(series2)
        ''ChartControl3.Series.Add(series3)
        ''ChartControl3.Series.Add(series4)
        ''ChartControl3.Series.Add(series5)
        ''ChartControl3.Series.Add(series6)
        ''ChartControl3.Series.Add(series7)

        ' Hide the legend (if necessary).
        ChartControl3.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False


        ' Add a title to the chart (if necessary).
        Dim chartTitle1 As New ChartTitle()
        chartTitle1.Text = getValue("file") & "-" & getValue("size")
        ChartControl3.Titles.Add(chartTitle1)




    End Sub




    'Public Sub s2ExDelFile()
    '    Chart2.Series("s2").Points.Clear()
    '    Chart2.Titles.Clear()
    '    Chart2.Titles.Add("File_Overwiew")
    '    Chart2.Series("s2").Points.AddXY("Normal_File", existingFiles)
    '    Chart2.Series("s2").Points.AddXY("Deleted_File", deletedFiles)
    'End Sub


    'Public Sub s4ToFileSize()
    '    Chart4.Series("s4").Points.Clear()
    '    Chart4.Titles.Clear()
    '    Chart4.Titles.Add("File Size")
    '    Chart4.Series("s4").Points.AddXY("50 - 100 KB", cSize2)
    '    Chart4.Series("s4").Points.AddXY("100 - 500 KB", cSize3)
    '    Chart4.Series("s4").Points.AddXY("500 - 1500KB", cSize4)
    '    Chart4.Series("s4").Points.AddXY("1,5 - 5 MB", cSize5)
    '    Chart4.Series("s4").Points.AddXY("5 - 500 MB", cSize6)
    '    Chart4.Series("s4").Points.AddXY("500 - 1000 MB", cSize7)
    '    Chart4.Series("s4").Points.AddXY(">1 GB", cSize8)

    'End Sub




    Private Sub pieChart_SelectedItemsChanged2(ByVal sender As Object, ByVal e As SelectedItemsChangedEventArgs) Handles ChartControl2.SelectedItemsChanged

        For Each piePoint As SeriesPoint In ChartControl2.SelectedItems


            If piePoint.Item(0).ToString = "0" Then

            Else
                If CDbl(piePoint.Item(0).ToString) = cPicture + 0.000001 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("picture") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cText + 0.000002 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("text") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cDocument + 0.000003 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("document") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cProgram + 0.000004 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("program") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cMusic + 0.000005 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("music") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cVideo + 0.000006 Then
                    GridView1.ActiveFilterString = "[Format] == '" & getValue("video") & "'"
                Else
                    MessageBox.Show("nothing")
                End If
            End If

        Next piePoint


        fbPicture.EditValue = True
        fbText.EditValue = True
        fbProgram.EditValue = True
        fbDocument.EditValue = True
        fbUnknown1.EditValue = True
        fbMusicVideo.EditValue = True
        ChartControl1.ClearSelection(True)


    End Sub


    Private Sub pieChart_SelectedItemsChanged1(ByVal sender As Object, ByVal e As SelectedItemsChangedEventArgs) Handles ChartControl1.SelectedItemsChanged

        For Each piePoint As SeriesPoint In ChartControl1.SelectedItems


            If piePoint.Item(0).ToString = "0" Then

            Else
                If CDbl(piePoint.Item(0).ToString) = cExcellent + 0.000001 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("excellent") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cGood + 0.000002 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("good") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cOK + 0.000003 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("ok") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cBad + 0.000004 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("bad") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cHorrible + 0.000005 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("horrible") & "'"
                ElseIf CDbl(piePoint.Item(0).ToString) = cOverwritten + 0.000006 Then
                    filterStringFileIntegrity = "[Estimated_File_Integrity] == '" & getValue("overwritten") & "'"

                End If
            End If

            filterChange()

        Next piePoint

        ChartControl2.ClearSelection(True)

    End Sub



    Private Sub CheckedChangedP(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbPicture.EditValueChanged
        filterChange()
    End Sub

    Private Sub CheckedChangedT(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbText.EditValueChanged
        filterChange()
    End Sub

    Private Sub CheckedChangedPr(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbProgram.EditValueChanged
        filterChange()
    End Sub

    Private Sub CheckedChangedU(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbUnknown1.EditValueChanged
        filterChange()
    End Sub

    Private Sub CheckedChangedD(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbDocument.EditValueChanged
        filterChange()
    End Sub

    Private Sub CheckedChangedM(ByVal sender As Object, ByVal e As System.EventArgs) Handles fbMusicVideo.EditValueChanged
        filterChange()
    End Sub


    Function filterChange()

        If Startup = False Then

            Dim tempfsfi As String = ""

            If filterStringFileIntegrity IsNot "" Then
                tempfsfi = " AND " & filterStringFileIntegrity
            End If

            filter = ""

            If fbPicture.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("picture") & "'" & tempfsfi

                Else
                    filter = filter & " OR [Format] == '" & getValue("picture") & "'" & tempfsfi
                End If
            End If
            If fbText.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("text") & "'" & tempfsfi
                Else
                    filter = filter & " OR [Format] == '" & getValue("text") & "'" & tempfsfi
                End If
            End If
            If fbProgram.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("program") & "'" & tempfsfi
                Else
                    filter = filter & " OR [Format] == '" & getValue("program") & "'" & tempfsfi
                End If
            End If
            If fbUnknown1.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("unknown") & "'" & tempfsfi
                Else
                    filter = filter & " OR [Format] == '" & getValue("unknown") & "'" & tempfsfi
                End If
            End If
            If fbDocument.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("document") & "'" & tempfsfi
                Else
                    filter = filter & " OR [Format] == '" & getValue("document") & "'" & tempfsfi
                End If
            End If
            If fbMusicVideo.EditValue = True Then
                If filter = "" Then
                    filter = "[Format] == '" & getValue("music") & "' OR [Format] == '" & getValue("video") & "'" & tempfsfi
                Else
                    filter = filter & " OR [Format] == '" & getValue("music") & "' OR [Format] == '" & getValue("video") & "'" & tempfsfi
                End If
            End If

            If fbMusicVideo.EditValue = True And fbDocument.EditValue = True And fbUnknown1.EditValue = True And fbProgram.EditValue = True And fbText.EditValue = True And fbPicture.EditValue = True Then
                filter = "" & filterStringFileIntegrity
            End If
            If fbMusicVideo.EditValue = False And fbDocument.EditValue = False And fbUnknown1.EditValue = False And fbProgram.EditValue = False And fbText.EditValue = False And fbPicture.EditValue = False Then
                filter = "[Format] == 'none'"
            End If



            'If filterStringFileIntegrity = "" Then

            'Else
            'If filter = "" Then
            'filter = filterStringFileIntegrity
            'Else
            'filter = filter & " AND " & filterStringFileIntegrity
            'End If

            'End If

            GridView1.ActiveFilterString = "" & filter

            'MessageBox.Show(filter)
            'MessageBox.Show(GridView1.ActiveFilterString)


        End If

    End Function


    Private Sub fbText_ItemClick(sender As Object, e As ItemClickEventArgs) Handles fbText.ItemClick

    End Sub

    Private Sub BarEditItem17_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarEditItem17.ItemClick
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs)
        scan = False
    End Sub

    Private Sub btnlanguage_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnlanguage.ItemClick
        'strLanguage = RepositoryItemComboBox2.cse
        'ToString.ToUpper.Substring(0, 2)
        MessageBox.Show(language)
        refreshLanguage()
    End Sub

    Private Sub ComboBoxEdit1_SelectedIndexChanged(ByVal sender As Object,
    ByVal e As System.EventArgs) Handles RepositoryItemComboBox2.SelectedIndexChanged
        Dim ec As EnumConverter = TryCast(TypeDescriptor.GetConverter(GetType(FontStyle)), EnumConverter)
        Dim cBox = TryCast(sender, ComboBoxEdit)
        If cBox.SelectedIndex <> -1 Then

            If scan = False Then
                If cBox.SelectedItem.ToString.ToUpper.Substring(0, 2) = language Then
                Else

                    Dim d As DialogResult = MessageBox.Show(getValue("set_language_to") & " " & cBox.SelectedItem & ". " & getValue("scan_results_lost"), getValue("change_language"), MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                    If d = DialogResult.Yes Then
                        language = cBox.SelectedItem.ToString.ToUpper.Substring(0, 2)
                        refreshLanguage()
                    Else
                        cBox.SelectedIndex = 0
                    End If

                End If

            Else
                MessageBox.Show(getValue("no_language_change_while_scan") & "!")

            End If



        End If
    End Sub

    Private Sub recover(sender As Object, e As ItemClickEventArgs) Handles bbiRecover.ItemClick

        Dim Dir As String = ""
        Dim AddOn As String = ""
        Dim AddOnNum As ULong = 0
        Dim Errors As Boolean = False
        Dim RBM As DirectDriveIO 'New DirectDriveIO(Drive)
        Dim buffer As NTFS_VOLUME_DATA_BUFFER

        Dim fd As FolderBrowserDialog = New FolderBrowserDialog()
        'fd.FileName = "folderselection"
        'fd.Title = "Select folder to recover files"
        'fd.CheckFileExists = False
        'fd.CheckPathExists = True
        fd.ShowDialog()
        MessageBox.Show(fd.SelectedPath)
        Dir = System.IO.Path.GetFullPath(fd.SelectedPath + "\") & "GruenDataRecovery\"
        Dir = Dir.Replace("folderselection", "").Trim()

        If Not Dir.StartsWith(LastDrive.ToString) Then



            Try
                Dim Drive = LastDrive
                Drive = Drive.TrimEnd("\")
                Try
                    RBM = New DirectDriveIO(Drive & "\")
                Catch
                    MsgBox("Could not open drive.", MsgBoxStyle.Critical, "ERROR")
                    Exit Sub
                End Try
                Dim diskhandle = CreateFile("\\?\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
                If diskhandle = 0 Then
                    diskhandle = CreateFile("\\.\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
                    If diskhandle = 0 Then
                        MsgBox("Could not access drive.", MsgBoxStyle.Critical, "ERROR")
                        Exit Sub
                        'GoTo DoNext
                    End If
                End If

                ''MessageBox.Show(diskhandle)
                Dim FSCTL_GET_NFTS_VOLUME_DATA = CTL_CODE(FILE_DEVICE.FILE_DEVICE_FILE_SYSTEM, 25, METHOD_BUFFERED, FILE_ANY_ACCESS)
                DeviceIoControlNTFS(diskhandle, FSCTL_GET_NFTS_VOLUME_DATA, 0, 0, buffer, SizeOf(buffer), 0, 0)
                CloseHandle(diskhandle)
            Catch ex As Exception
                MsgBox("Could not access drive.", MsgBoxStyle.Critical, "ERROR")
                Exit Sub
            End Try

            Dim view As ColumnView = CType(GridControl.MainView, ColumnView)
            Dim colFormat As GridColumn = GridView1.Columns.ColumnByName("colFormat")
            Dim colFile_Name As GridColumn = GridView1.Columns.ColumnByName("colFile_Name")
            Dim colFile_Path As GridColumn = GridView1.Columns.ColumnByName("colFile_Path")
            Dim colEnding As GridColumn = GridView1.Columns.ColumnByName("colEnding")
            Dim colSize As GridColumn = GridView1.Columns.ColumnByName("colSize")
            Dim colMFT_Sector_Adress As GridColumn = GridView1.Columns.ColumnByName("colMFT_Sector_Adress")
            Dim colEstimated_File_Integrity As GridColumn = GridView1.Columns.ColumnByName("colEstimated_File_Integrity")
            Dim selectedRowHandles As Integer() = view.GetSelectedRows()

            ProgressBarControl2.EditValue = 0
            ProgressBarControl2.Properties.Maximum = selectedRowHandles.Length


            Dim Num As ULong = 0

            If selectedRowHandles.Length > 0 Then
                view.FocusedRowHandle = selectedRowHandles(0)
                For i As Integer = 0 To selectedRowHandles.Length - 1

                    Dim name As String = view.GetRowCellDisplayText(selectedRowHandles(i), colFile_Name)
                    Dim mft As String = view.GetRowCellDisplayText(selectedRowHandles(i), colMFT_Sector_Adress)
                    Dim size As String = view.GetRowCellDisplayText(selectedRowHandles(i), colSize)
                    Dim colPath As String = view.GetRowCellDisplayText(selectedRowHandles(i), colFile_Path)
                    Dim Path As String = Dir
                    Path = Path & colPath.Replace(":", "")

                    Dim oFSO As Object
                    oFSO = CreateObject("Scripting.FileSystemObject")
                    If Not oFSO.FolderExists(Path) Then
                        MkDir(Path)
                    End If



                    ProgressBarControl2.EditValue = Num
                    Num = Num + 1
                    Try
                        Dim MFTSector = mft
                        Dim FileLength As ULong = size
                        Dim Bytes = RBM.ReadSectors(CLng(MFTSector), CLng((buffer.BytesPerFileRecordSegment / buffer.BytesPerSector)))
                        Dim baseaddr = MergeToInt(Bytes, &H14, &H15) 'The offset the the first attribute
                        While Bytes(baseaddr) <> &H80
                            baseaddr = baseaddr + MergeToInt(Bytes, baseaddr + &H4, baseaddr + &H7) 'Add the length of the attribute to the base address to find the next attribute
                        End While
                        Dim Length As ULong = 0
                        Dim LenLen As Byte = 0
                        Dim Offset As ULong = 0
                        Dim OffLen As Byte = 0
                        Dim ReadSize As ULong = 0
                        AddOn = ""
                        AddOnNum = 0
ReCheck:
                        If My.Computer.FileSystem.FileExists(Path & "\" & name & AddOn) Then
                            AddOnNum = AddOnNum + 1
                            AddOn = "(" & AddOnNum.ToString & ")"
                            GoTo ReCheck
                        End If
                        Dim TempFile As New System.IO.FileStream(Path & "\" & System.IO.Path.GetFileNameWithoutExtension(name) & AddOn & System.IO.Path.GetExtension(name), IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
                        If MergeToInt(Bytes, baseaddr + &HE, baseaddr + &HF) = 1 And (buffer.BytesPerFileRecordSegment - (baseaddr + &H18 + FileLength)) > 0 Then
                            Try
                                TempFile.Write(ByteArrayPart(Bytes, baseaddr + &H18, baseaddr + &H18 + FileLength), 0, FileLength)
                            Catch
                                Errors = True
                            End Try
                            GoTo CleanUp
                        End If
                        baseaddr = baseaddr + &H40
                        While Bytes(baseaddr) > 0
                            LenLen = Bytes(baseaddr) And &HF
                            OffLen = (Bytes(baseaddr) And &HF0) / &H10
                            Length = MergeToInt(Bytes, baseaddr + 1, baseaddr + LenLen)
                            Offset = Offset + MergeToInt(Bytes, baseaddr + 1 + LenLen, baseaddr + LenLen + OffLen)
                            If Length <= 1024 Then
                                TempFile.Write(RBM.ReadSectors(Offset * (buffer.BytesPerCluster / buffer.BytesPerSector), Length * (buffer.BytesPerCluster / buffer.BytesPerSector)), 0, Length * buffer.BytesPerCluster)
                            Else
                                For Section = 0 To Length \ 1024
                                    If Section = Length \ 1024 Then
                                        TempFile.Write(RBM.ReadSectors((Offset + (Section * 1024)) * (buffer.BytesPerCluster / buffer.BytesPerSector), (Length Mod 1024) * (buffer.BytesPerCluster / buffer.BytesPerSector)), 0, (Length Mod 1024) * buffer.BytesPerCluster)
                                    Else
                                        TempFile.Write(RBM.ReadSectors((Offset + (Section * 1024)) * (buffer.BytesPerCluster / buffer.BytesPerSector), 1024 * (buffer.BytesPerCluster / buffer.BytesPerSector)), 0, 1024 * buffer.BytesPerCluster)
                                    End If
                                    bsiRecordsCount.Caption = "Recovering file " & name & " " & round(((ReadSize + (Section * 1024)) * buffer.BytesPerCluster * 100) / FileLength, 1) & "%..."
                                    If ((ReadSize + (Section * 1024)) * buffer.BytesPerCluster) > FileLength Then Exit While
                                    Application.DoEvents()
                                Next
                            End If
                            ReadSize = ReadSize + Length
                            bsiRecordsCount.Caption = "Recovering file " & name & " " & round((ReadSize * buffer.BytesPerCluster * 100) / FileLength, 1) & "%..."
                            If (ReadSize * buffer.BytesPerCluster) > FileLength Then Exit While
                            'Try
                            'ProgressBar1.Value = CInt((ReadSize * buffer.BytesPerCluster * 1000) / FileLength)
                            'Catch
                            'ProgressBar1.Value = 1000
                            'End Try
                            Application.DoEvents()
                            baseaddr = baseaddr + (1 + LenLen + OffLen)
                        End While
CleanUp:
                        TempFile.SetLength(FileLength)
                        TempFile.Close()
                        TempFile.Dispose()
                        'MsgBox("File recovered successfuly.", MsgBoxStyle.Information, "")
                    Catch
                        Errors = True

                        'MsgBox("File not recovered.", MsgBoxStyle.Critical, "ERROR")
                    End Try
DoNext:






                Next
            End If


            bsiRecordsCount.Caption = ""
            ProgressBarControl2.EditValue = 0
            If Not Errors Then
                MsgBox("Files recovered successfuly.", MsgBoxStyle.Information, "")
            Else
                MsgBox("File recovered with errors.", MsgBoxStyle.Exclamation, "")
            End If

        Else

            MessageBox.Show(getValue("recover_same_drive"))

        End If

    End Sub


    Private Sub bbiReset_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiReset.ItemClick
        GridView1.ActiveFilterString = ""
        filterStringFileIntegrity = ""
        ChartControl1.ClearSelection(True)
        fbPicture.EditValue = True
        fbText.EditValue = True
        fbProgram.EditValue = True
        fbDocument.EditValue = True
        fbUnknown1.EditValue = True
        fbMusicVideo.EditValue = True
        filterChange()

    End Sub

    Private Sub btnStop_Click_1(sender As Object, e As EventArgs) Handles btnStop.Click
        PopupControlContainer1.Hide()
        scan = False
    End Sub

    Private Sub BarButtonItem2_ItemClick(sender As Object, e As ItemClickEventArgs)
        Dim myModalForm As Drive = New Drive()
        Dim dialogResult As DialogResult
        Dim selectedText As String = ""
        dialogResult = myModalForm.ShowDialog(Me)
        If dialogResult = System.Windows.Forms.DialogResult.OK Then
            selectedText = myModalForm.GetSelectedText()
        Else
            ' Perform default actions here.
        End If
        myModalForm.Dispose()
        currentDrive = selectedText
        MessageBox.Show(selectedText)

    End Sub

    Private Sub BarButtonItem4_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiBin.ItemClick
        ShowRecycleBin()
    End Sub

    Private Declare Function ShellExecute Lib "shell32.dll" _
    Alias "ShellExecuteA" (ByVal hWnd As Long,
    ByVal lpOperation As String,
    ByVal lpFile As String,
    ByVal lpParameters As String,
    ByVal lpDirectory As String,
    ByVal nShowCmd As Long) _
    As Long

    Private Const SW_SHOWNORMAL As Long = 1

    Public Function ShowRecycleBin() As Boolean
        Dim lRet As Long
        'if using from a form, you can use me.hwnd instead of 0&
        'for the first argument
        lRet = ShellExecute(0&, "Open", "explorer.exe",
        "/root,::{645FF040-5081-101B-9F08-00AA002F954E}", 0&,
         SW_SHOWNORMAL)
        ShowRecycleBin = lRet > 32
    End Function


    Private Sub getHelp_ItemClick(sender As Object, e As ItemClickEventArgs) Handles getHelp.ItemClick
        Dim webAddress As String

        If currentDrive IsNot Nothing Then


            Dim drivetorecover As IO.DriveInfo = My.Computer.FileSystem.GetDriveInfo(currentDrive & "\")
            webAddress = "https://www.data-recovery.de/datenrettung-anfragen/?os=" + My.Computer.Info.OSPlatform + "&drive=" + drivetorecover.RootDirectory.ToString.Replace(":\", "") + "&size=" + drivetorecover.TotalSize.ToString + "&label=" + drivetorecover.VolumeLabel.ToString

        Else
            webAddress = "https://www.data-recovery.de/datenrettung-anfragen/?os=" + My.Computer.Info.OSPlatform
        End If

        Process.Start(webAddress)
    End Sub

    Private Sub getInfo_ItemClick(sender As Object, e As ItemClickEventArgs) Handles getInfo.ItemClick

        Dim info = New info
        info.Show()

    End Sub
End Class


#Region "DIRECT DISK IO API"
Public Class DirectDriveIO
    Private Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, ByVal lpSecurityAttributes As Integer, ByVal dwCreationDisposition As Integer, ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As Integer) As Integer
    Private Declare Function ReadFile Lib "kernel32" (ByVal hFile As Integer, ByRef lpBuffer As Object, ByVal nNumberOfBytesToRead As Integer, ByRef lpNumberOfBytesRead As Integer, ByVal lpOverlapped As Integer) As Boolean
    Private Declare Function SetFilePointer Lib "kernel32" (ByVal hFile As Integer, ByVal lDistanceToMove As Integer, ByVal lpDistanceToMoveHigh As Integer, ByVal dwMoveMethod As Integer) As Integer
    Private Declare Function SetFilePointerEx Lib "kernel32" (ByVal hFile As Integer, ByVal liDistanceToMove As Int64, ByRef lpNewFilePointer As Int64, ByVal dwMoveMethod As Integer) As Boolean
    Private Declare Function DeviceIoControlNTFS Lib "kernel32" Alias "DeviceIoControl" (ByVal hDevice As Int32, ByVal dwIoControlCode As Int32, ByRef lpInBuffer As Object, ByVal nInBufferSize As Int32, ByRef lpOutBuffer As NTFS_VOLUME_DATA_BUFFER, ByVal nOutBufferSize As Int32, ByRef lpBytesReturned As Int32, ByVal lpOverlapped As Int32) As Int32
    Private Declare Function DeviceIoControlPropertyAccessAlignment Lib "kernel32" Alias "DeviceIoControl" (ByVal hDevice As Int32, ByVal dwIoControlCode As Int32, ByRef lpInBuffer As STORAGE_PROPERTY_QUERY, ByVal nInBufferSize As Int32, ByRef lpOutBuffer As STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR, ByVal nOutBufferSize As Int32, ByRef lpBytesReturned As Int32, ByVal lpOverlapped As Int32) As Int32
    Private Declare Function DeviceIoControlNumber Lib "kernel32" Alias "DeviceIoControl" (ByVal hDevice As Int32, ByVal dwIoControlCode As Int32, ByRef lpInBuffer As Object, ByVal nInBufferSize As Int32, ByRef lpOutBuffer As _STORAGE_DEVICE_NUMBER, ByVal nOutBufferSize As Int32, ByRef lpBytesReturned As Int32, ByVal lpOverlapped As Int32) As Int32
    Private Structure _STORAGE_DEVICE_NUMBER
        Dim DeviceType As Int32
        Dim DeviceNumber As ULong
        Dim PartitionNumber As ULong
    End Structure
    Private Structure STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR
        Dim Version As Integer
        Dim Size As Integer
        Dim BytesPerCacheLine As Integer
        Dim BytesOffsetForCacheAllignment As Integer
        Dim BytesPerLogicalSector As Integer
        Dim BytesPerPhysicalSector As Integer
        Dim BytesOffsetForSectorAllignment As Integer
    End Structure
    Private Structure STORAGE_PROPERTY_QUERY
        Dim PropertyId As STORAGE_PROPERTY_ID
        Dim QueryType As STORAGE_QUERY_TYPE
        Dim AdditionalParameters As Byte
    End Structure
    Private Structure NTFS_VOLUME_DATA_BUFFER
        Dim VolumeSerialNumber As Int64
        Dim NumberSectors As Int64
        Dim TotalClusters As Int64
        Dim FreeClusters As Int64
        Dim TotalReserved As Int64
        Dim BytesPerSector As Int32 'UInt32
        Dim BytesPerCluster As Int32 'UInt32
        Dim BytesPerFileRecordSegment As Int32 'UInt32
        Dim ClustersPerFileRecordSegment As Int32 'UInt32
        Dim MftValidDataLength As Int64
        Dim MftStartLcn As Int64
        Dim Mft2StartLcn As Int64
        Dim MftZoneStart As Int64
        Dim MftZoneEnd As Int64
    End Structure
    Private Enum STORAGE_PROPERTY_ID
        StorageDeviceProperty = 0
        StorageAdapterProperty
        StorageDeviceIdProperty
        StorageDeviceUniqueIdProperty
        StorageDeviceWriteCacheProperty
        StorageMiniportProperty
        StorageAccessAlignmentProperty
        StorageDeviceSeekPenaltyProperty
        StorageDeviceTrimProperty
    End Enum
    Private Enum STORAGE_QUERY_TYPE
        PropertyStandardQuery = 0
        PropertyExistsQuery
    End Enum
    Private Enum EFileAccess As System.Int32
        DELETE = &H10000
        READ_CONTROL = &H20000
        WRITE_DAC = &H40000
        WRITE_OWNER = &H80000
        SYNCHRONIZE = &H100000
        STANDARD_RIGHTS_REQUIRED = &HF0000
        STANDARD_RIGHTS_READ = READ_CONTROL
        STANDARD_RIGHTS_WRITE = READ_CONTROL
        STANDARD_RIGHTS_EXECUTE = READ_CONTROL
        STANDARD_RIGHTS_ALL = &H1F0000
        SPECIFIC_RIGHTS_ALL = &HFFFF
        ACCESS_SYSTEM_SECURITY = &H1000000
        MAXIMUM_ALLOWED = &H2000000
        GENERIC_READ = &H80000000
        GENERIC_WRITE = &H40000000
        GENERIC_EXECUTE = &H20000000
        GENERIC_ALL = &H10000000
    End Enum
    Private Enum EFileShare
        FILE_SHARE_NONE = &H0
        FILE_SHARE_READ = &H1
        FILE_SHARE_WRITE = &H2
        FILE_SHARE_DELETE = &H4
    End Enum
    Private Enum ECreationDisposition
        CREATE_NEW = 1
        CREATE_ALWAYS = 2
        OPEN_EXISTING = 3
        OPEN_ALWAYS = 4
        TRUNCATE_EXISTING = 5
    End Enum
    Private Enum EFileAttributes
        FILE_ATTRIBUTE_READONLY = &H1
        FILE_ATTRIBUTE_HIDDEN = &H2
        FILE_ATTRIBUTE_SYSTEM = &H4
        FILE_ATTRIBUTE_DIRECTORY = &H10
        FILE_ATTRIBUTE_ARCHIVE = &H20
        FILE_ATTRIBUTE_DEVICE = &H40
        FILE_ATTRIBUTE_NORMAL = &H80
        FILE_ATTRIBUTE_TEMPORARY = &H100
        FILE_ATTRIBUTE_SPARSE_FILE = &H200
        FILE_ATTRIBUTE_REPARSE_POINT = &H400
        FILE_ATTRIBUTE_COMPRESSED = &H800
        FILE_ATTRIBUTE_OFFLINE = &H1000
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = &H2000
        FILE_ATTRIBUTE_ENCRYPTED = &H4000
        FILE_ATTRIBUTE_VIRTUAL = &H10000
        FILE_FLAG_BACKUP_SEMANTICS = &H2000000
        FILE_FLAG_DELETE_ON_CLOSE = &H4000000
        FILE_FLAG_NO_BUFFERING = &H2000000
        FILE_FLAG_OPEN_NO_RECALL = &H100000
        FILE_FLAG_OPEN_REPARSE_POINT = &H200000
        FILE_FLAG_OVERLAPPED = &H40000000
        FILE_FLAG_POSIX_SEMANTICS = &H100000
        FILE_FLAG_RANDOM_ACCESS = &H10000000
        FILE_FLAG_SEQUENTIAL_SCAN = &H8000000
        FILE_FLAG_WRITE_THROUGH = &H80000000
    End Enum
    Private Enum FILE_DEVICE
        FILE_DEVICE_BEEP = 1
        FILE_DEVICE_CD_ROM
        FILE_DEVICE_CD_ROM_FILE_SYSTEM
        FILE_DEVICE_CONTROLLER
        FILE_DEVICE_DATALINK
        FILE_DEVICE_DFS
        FILE_DEVICE_DISK
        FILE_DEVICE_DISK_FILE_SYSTEM
        FILE_DEVICE_FILE_SYSTEM
        FILE_DEVICE_INPORT_PORT
        FILE_DEVICE_KEYBOARD
        FILE_DEVICE_MAILSLOT
        FILE_DEVICE_MIDI_IN
        FILE_DEVICE_MIDI_OUT
        FILE_DEVICE_MOUSE
        FILE_DEVICE_MULTI_UNC_PROVIDER
        FILE_DEVICE_NAMED_PIPE
        FILE_DEVICE_NETWORK
        FILE_DEVICE_NETWORK_BROWSER
        FILE_DEVICE_NETWORK_FILE_SYSTEM
        FILE_DEVICE_NULL
        FILE_DEVICE_PARALLEL_PORT
        FILE_DEVICE_PHYSICAL_NETCARD
        FILE_DEVICE_PRINTER
        FILE_DEVICE_SCANNER
        FILE_DEVICE_SERIAL_MOUSE_PORT
        FILE_DEVICE_SERIAL_PORT
        FILE_DEVICE_SCREEN
        FILE_DEVICE_SOUND
        FILE_DEVICE_DEVICE_STREAMS
        FILE_DEVICE_TAPE
        FILE_DEVICE_TAPE_FILE_SYSTEM
        FILE_DEVICE_TRANSPORT
        FILE_DEVICE_UNKNOWN
        FILE_DEVICE_VIDEO
        FILE_DEVICE_VIRTUAL_DISK
        FILE_DEVICE_WAVE_IN
        FILE_DEVICE_WAVE_OUT
        FILE_DEVICE_8042_PORT
        FILE_DEVICE_NETWORK_REDIRECTOR
        FILE_DEVICE_BATTERY
        FILE_DEVICE_BUS_EXTENDER
        FILE_DEVICE_MODEM
        FILE_DEVICE_VDM
        FILE_DEVICE_MASS_STORAGE
        FILE_DEVICE_SMB
        FILE_DEVICE_KS
        FILE_DEVICE_CHANGER
        FILE_DEVICE_SMARTCARD
        FILE_DEVICE_ACPI
        FILE_DEVICE_DVD
        FILE_DEVICE_FULLSCREEN_VIDEO
        FILE_DEVICE_DFS_FILE_SYSTEM
        FILE_DEVICE_DFS_VOLUME
    End Enum
    Private Const FILE_ANY_ACCESS = &H0
    Private Const FILE_READ_ACCESS = &H1
    Private Const FILE_WRITE_ACCESS = &H2
    Private Const METHOD_BUFFERED = &H0
    Private Const METHOD_IN_DIRECT = &H1
    Private Const METHOD_OUT_DIRECT = &H2
    Private Const METHOD_NEITHER = &H3
    Private DriveHandle As IntPtr
    Private DiskName As String
    Private SectorSize As Integer
    Private MaxSector As Long
    Private DirectDeviceAccess As Boolean = False
    Private Const FILE_BEGIN = 0
    Private fs As System.IO.FileStream
    Sub New(ByVal Drive As String, Optional ByVal DirectAccess As Boolean = False)
        DirectDeviceAccess = DirectAccess
        Disk = Drive
    End Sub
    Public Function ReadSectors(ByVal StartingLogicalSector As Long, ByVal NumberOfSectors As Integer) As Byte()
        'SetFilePointer(DriveHandle, StartingLogicalSector * SectorSize, 0, FILE_BEGIN)
        fs.Seek(StartingLogicalSector * CLng(SectorSize), IO.SeekOrigin.Begin)
        'SetFilePointerEx(DriveHandle, StartingLogicalSector * CLng(SectorSize), 0, FILE_BEGIN)
        'Dim Buffer() As Byte
        Dim Buffer((NumberOfSectors * SectorSize) - 1) As Byte
        'If Not ReadFile(DriveHandle, Buffer(0), NumberOfSectors * SectorSize, 0, 0) Then Return Nothing
        fs.Read(Buffer, 0, NumberOfSectors * SectorSize)
        'ReadFile(DriveHandle, Buffer(0), NumberOfSectors * SectorSize, 0, 0)
        'MsgBox(Err.LastDllError)
        Return Buffer
    End Function

    Public Property Disk() As String
        Get
            Return DiskName
        End Get
        Set(ByVal value As String)
            DiskName = value
            Dim drive = DiskName
            drive = drive.TrimEnd("\")
            'Dim diskhandle = CreateFile(devicepathname(drive & "\").TrimEnd("\"), EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            Dim diskhandle As Integer
            If DirectDeviceAccess Then
                diskhandle = CreateFile("\\?\" & PhysicalDrive(drive), EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            Else
                diskhandle = CreateFile("\\?\" & drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            End If
            If diskhandle = 0 Then
                diskhandle = CreateFile("\\.\" & drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
                If diskhandle = 0 Then
                    Throw New Exception("Could not access disk.")
                    Exit Property
                End If
            End If
            DriveHandle = diskhandle
            fs = New System.IO.FileStream(DriveHandle, IO.FileAccess.ReadWrite)
            Dim FSCTL_GET_NFTS_VOLUME_DATA = CTL_CODE(FILE_DEVICE.FILE_DEVICE_FILE_SYSTEM, 25, METHOD_BUFFERED, FILE_ANY_ACCESS)
            Dim IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE(FILE_DEVICE.FILE_DEVICE_MASS_STORAGE, &H500, METHOD_BUFFERED, FILE_ANY_ACCESS)
            Dim buffer As NTFS_VOLUME_DATA_BUFFER
            Dim propaabuffer As STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR
            DeviceIoControlNTFS(diskhandle, FSCTL_GET_NFTS_VOLUME_DATA, 0, 0, buffer, SizeOf(buffer), 0, 0)
            Dim query As New STORAGE_PROPERTY_QUERY
            query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery
            query.PropertyId = STORAGE_PROPERTY_ID.StorageAccessAlignmentProperty
            DeviceIoControlPropertyAccessAlignment(diskhandle, IOCTL_STORAGE_QUERY_PROPERTY, query, SizeOf(query), propaabuffer, SizeOf(propaabuffer), 0, 0)
            Dim BytesPerSector = buffer.BytesPerSector
            If BytesPerSector = 0 Then
                BytesPerSector = propaabuffer.BytesPerPhysicalSector
                If BytesPerSector = 0 Then BytesPerSector = 512
            End If
            SectorSize = BytesPerSector
            MaxSector = buffer.NumberSectors - 1
            If MaxSector = 0 Or MaxSector = -1 Then
                Try
                    MaxSector = CLng(My.Computer.FileSystem.GetDriveInfo(DiskName).TotalSize / CLng(SectorSize))
                Catch
                    Throw New Exception("Could not access disk.")
                    Exit Property
                End Try
                If MaxSector = 0 Then
                    Throw New Exception("Could not access disk.")
                    Exit Property
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property BytesPerSector() As Integer
        Get
            Return SectorSize
        End Get
    End Property
    Private Function CTL_CODE(ByVal DeviceType As Int32, ByVal FunctionNumber As Int32, ByVal Method As Int32, ByVal Access As Int32) As Int32
        Return (DeviceType << 16) Or (Access << 14) Or (FunctionNumber << 2) Or Method
    End Function
    Private Function PhysicalDrive(ByVal Drive As String) As String
        Dim devn As Integer = -1
        For Each Driver As System.IO.DriveInfo In My.Computer.FileSystem.Drives
            If Driver.DriveType = IO.DriveType.Fixed Or Driver.DriveType = IO.DriveType.Removable Then devn = devn + 1
            If Driver.Name.TrimEnd("\") = UCase(Drive.TrimEnd("\")) Then Return "PhysicalDrive" & devn.ToString
        Next
        Dim diskhandle = CreateFile("\\?\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
        If diskhandle = 0 Then
            diskhandle = CreateFile("\\.\" & Drive, EFileAccess.GENERIC_READ + EFileAccess.GENERIC_WRITE, EFileShare.FILE_SHARE_READ + EFileShare.FILE_SHARE_WRITE, Nothing, ECreationDisposition.OPEN_EXISTING, 0, Nothing)
            If diskhandle = 0 Then
                Throw New Exception("Could not access disk.")
                Exit Function
            End If
        End If
        Dim IOCTL_STORAGE_GET_DEVICE_NUMBER = CTL_CODE(FILE_DEVICE.FILE_DEVICE_MASS_STORAGE, &H420, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim dnbuffer As _STORAGE_DEVICE_NUMBER
        DeviceIoControlNumber(diskhandle, IOCTL_STORAGE_GET_DEVICE_NUMBER, 0, 0, dnbuffer, SizeOf(dnbuffer), 0, 0)
        dnbuffer.DeviceNumber = 0
        Return "PhysicalDrive" & dnbuffer.DeviceNumber.ToString
    End Function
End Class
#End Region





