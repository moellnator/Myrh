Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public Class Metafile : Inherits GDI32

        <Runtime.InteropServices.DllImport("gdi32")>
        Private Shared Function GetEnhMetaFileBits(hemf As Integer, cbBuffer As Integer, lpbBuffer As Byte()) As Integer
        End Function

        Private _metafile As Imaging.Metafile
        Private _bitmap As Drawing.Bitmap
        Private _bmp_grfx As Drawing.Graphics

        Public Overrides ReadOnly Property DefaultExtension As String
            Get
                Return "emf"
            End Get
        End Property

        Public Sub New(setup As PageSetup)
            MyBase.New(setup)
        End Sub

        Public Overrides Sub Save(Filename As String)
            MyBase._dispose()
            Dim enhMetafileHandle As Integer = Me._metafile.GetHenhmetafile().ToInt32()
            Dim bufferSize As Integer = GetEnhMetaFileBits(enhMetafileHandle, 0, Nothing)
            Dim buffer As Byte() : ReDim buffer(bufferSize - 1)
            If GetEnhMetaFileBits(enhMetafileHandle, bufferSize, buffer) <= 0 Then Throw New Exception("Unable to get metafile data.")
            Using fs As New IO.FileStream(Filename, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
                fs.Write(buffer, 0, bufferSize)
            End Using
            Me.Dispose()
        End Sub

        Protected Overrides Function CreateGraphics() As Drawing.Graphics
            Dim size As New Size(Math.Round(Me.Setup.Device.Width), Math.Round(Me.Setup.Device.Height))
            Dim bounds As New Rectangle(0, 0, Math.Round(size.Width), Math.Round(size.Height))
            Me._bitmap = New Drawing.Bitmap(bounds.Width, bounds.Height, Imaging.PixelFormat.Format32bppArgb)
            Me._bitmap.SetResolution(Me.Setup.DPI, Me.Setup.DPI)
            Dim grfx As Drawing.Graphics = Drawing.Graphics.FromImage(Me._bitmap)
            Dim hdc As IntPtr = grfx.GetHdc
            Me._metafile = New Imaging.Metafile(hdc, bounds, Imaging.MetafileFrameUnit.Pixel, Imaging.EmfType.EmfPlusOnly, "...")
            grfx.ReleaseHdc(hdc)
            Me._bmp_grfx = grfx
            Return Drawing.Graphics.FromImage(Me._metafile)
        End Function

        Protected Overrides Sub _dispose()
            MyBase._dispose()
            Me._bmp_grfx.Dispose()
            Me._metafile.Dispose()
            Me._bitmap.Dispose()
        End Sub

    End Class

End Namespace
