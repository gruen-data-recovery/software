<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class info
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim SnapOptions1 As DevExpress.Utils.Controls.SnapOptions = New DevExpress.Utils.Controls.SnapOptions()
        Me.PictureEdit1 = New DevExpress.XtraEditors.PictureEdit()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.version = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.BehaviorManager1 = New DevExpress.Utils.Behaviors.BehaviorManager(Me.components)
        CType(Me.PictureEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BehaviorManager1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureEdit1
        '
        Me.PictureEdit1.EditValue = Global.DataRecoveryGruen.My.Resources.Resources.logo
        Me.PictureEdit1.Location = New System.Drawing.Point(12, 52)
        Me.PictureEdit1.Name = "PictureEdit1"
        Me.PictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.[Auto]
        Me.PictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom
        Me.PictureEdit1.Size = New System.Drawing.Size(137, 140)
        Me.PictureEdit1.TabIndex = 0
        '
        'LabelControl1
        '
        Me.LabelControl1.Location = New System.Drawing.Point(12, 22)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(245, 13)
        Me.LabelControl1.TabIndex = 1
        Me.LabelControl1.Text = "GRÜN Data Recovery - Wir retten Daten seit 1991."
        '
        'LabelControl2
        '
        Me.LabelControl2.Location = New System.Drawing.Point(12, 198)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(152, 13)
        Me.LabelControl2.TabIndex = 2
        Me.LabelControl2.Text = "GRÜN Data Recovery Freeware"
        '
        'version
        '
        Me.version.Location = New System.Drawing.Point(12, 217)
        Me.version.Name = "version"
        Me.version.Size = New System.Drawing.Size(62, 13)
        Me.version.TabIndex = 3
        Me.version.Text = "Version ......"
        '
        'LabelControl3
        '
        Me.LabelControl3.Location = New System.Drawing.Point(12, 248)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(184, 13)
        Me.LabelControl3.TabIndex = 4
        Me.LabelControl3.Text = "Copyright GRÜN Data Recovery GmbH"
        '
        'LabelControl4
        '
        Me.LabelControl4.Location = New System.Drawing.Point(12, 267)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(113, 13)
        Me.LabelControl4.TabIndex = 5
        Me.LabelControl4.Text = "www.data-recovery.de"
        '
        'info
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        SnapOptions1.SnapOnMoving = DevExpress.Utils.DefaultBoolean.[False]
        SnapOptions1.SnapOnResizing = DevExpress.Utils.DefaultBoolean.[False]
        SnapOptions1.SnapToGrid = DevExpress.Utils.DefaultBoolean.[False]
        SnapOptions1.SnapToScreen = DevExpress.Utils.DefaultBoolean.[False]
        SnapOptions1.SnapToSnapForms = DevExpress.Utils.DefaultBoolean.[False]
        Me.BehaviorManager1.SetBehaviors(Me, New DevExpress.Utils.Behaviors.Behavior() {CType(DevExpress.Utils.Behaviors.Common.SnapWindowBehavior.Create(GetType(DevExpress.Utils.BehaviorSource.SnapWindowBehaviorSourceForForm), SnapOptions1), DevExpress.Utils.Behaviors.Behavior)})
        Me.ClientSize = New System.Drawing.Size(335, 309)
        Me.Controls.Add(Me.LabelControl4)
        Me.Controls.Add(Me.LabelControl3)
        Me.Controls.Add(Me.version)
        Me.Controls.Add(Me.LabelControl2)
        Me.Controls.Add(Me.LabelControl1)
        Me.Controls.Add(Me.PictureEdit1)
        Me.IconOptions.Image = Global.DataRecoveryGruen.My.Resources.Resources.favicon_32x32
        Me.Name = "info"
        Me.Text = "GRÜN Data Recovery Info"
        CType(Me.PictureEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BehaviorManager1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PictureEdit1 As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents version As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BehaviorManager1 As DevExpress.Utils.Behaviors.BehaviorManager
End Class
