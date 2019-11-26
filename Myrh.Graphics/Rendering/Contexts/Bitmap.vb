Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public Class Bitmap : Inherits GDI32

        Private _bitmap As Drawing.Bitmap

        Public Overrides ReadOnly Property DefaultExtension As String
            Get
                Return "png"
            End Get
        End Property

        Public Sub New(setup As PageSetup)
            MyBase.New(setup)
        End Sub

        Public Overrides Sub Save(Filename As String)
            Me._bitmap.Save(Filename, Imaging.ImageFormat.Png)
        End Sub

        Protected Overrides Function CreateGraphics() As Drawing.Graphics
            Me._bitmap = New Drawing.Bitmap(Me.Size.Width, Me.Size.Height, Imaging.PixelFormat.Format32bppArgb)
            Return Drawing.Graphics.FromImage(Me._bitmap)
        End Function

        Protected Overrides Sub _dispose()
            MyBase._dispose()
            Me._bitmap.Dispose()
        End Sub

        Public Shared Narrowing Operator CType(obj As Bitmap) As Drawing.Bitmap
            Return obj._bitmap
        End Operator

    End Class

End Namespace
