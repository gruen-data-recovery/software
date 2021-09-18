Public Class Drive


    Private Sub XtraPopup(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        CheckedListBoxControl1.Items.Clear()

        For Each Drive In My.Computer.FileSystem.Drives
            Try
                If Drive.DriveFormat = "NTFS" Then
                    CheckedListBoxControl1.Items.Add(Drive.Name & " (" & " " & Drive.DriveFormat & ")") ''GetSizeStr(Drive.TotalSize)) &
                End If
            Catch
            End Try
        Next
    End Sub

    Public Function GetSelectedText() As String
        Return Me.CheckedListBoxControl1.Text
    End Function

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class