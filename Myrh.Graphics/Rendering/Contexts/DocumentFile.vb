Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf

Namespace Rendering.Contexts
    Public Class DocumentFile : Inherits Context

        Private ReadOnly _doc As PdfDocument
        Private ReadOnly _page As PdfPage
        Private ReadOnly _graphics As XGraphics

        Public Sub New(setup As PageSetup)
            MyBase.New(
                New Display(
                    setup.Context.Width,
                    -setup.Context.Height,
                    setup.Context.Width / 2,
                    setup.Context.Height / 2
                )
            )
            Me._doc = New PdfDocument()
            Me._doc.Options.ColorMode = PdfColorMode.Rgb
            Me._doc.Options.CompressContentStreams = False
            Me._page = Me._doc.AddPage
            Me._page.Width = New XUnit(setup.Context.Width, XGraphicsUnit.Inch)
            Me._page.Height = New XUnit(setup.Context.Height, XGraphicsUnit.Inch)
            Me._graphics = XGraphics.FromPdfPage(Me._page)
        End Sub

        Public Overrides ReadOnly Property DefaultExtension As String
            Get
                Return "pdf"
            End Get
        End Property

        Public Overrides Sub Save(Filename As String)
            Me._doc.Save(Filename)
        End Sub

        Public Overrides Sub Primitive(line As Lines, engine As Engine)
            Dim pen As New XPen(XColor.FromArgb(line.Color.A, line.Color.R, line.Color.G, line.Color.B), line.Width) With {
                .LineCap = XLineCap.Round
            }
            If line.Pattern.Count > 1 Then pen.DashPattern = line.Pattern.Select(Function(s) CDbl(s)).ToArray
            Dim points As PointF() = (From v As Vertex In line.Verticies Select engine.Pipeline(v, Me.Display).ToPointF).ToArray
            Me._graphics.DrawLines(pen, points.Select(Function(p) New XPoint(p.X * 72, p.Y * 72)).ToArray)
        End Sub

        Public Overrides Sub Primitive(text As Text, engine As Engine)
            Dim font_size As Single = text.Font.SizeInPoints
            Dim a As Double = text.Rotation / 180 * Math.PI
            Dim foption As New XPdfFontOptions(PdfFontEncoding.Unicode)
            Dim font As New XFont(New Font(text.Font.FontFamily, font_size, text.Font.Style, GraphicsUnit.World))
            Dim text_size As XSize = Me._graphics.MeasureString(text.Text, font)
            Dim text_size_tr As New XSize(
                Math.Abs(text_size.Width * Math.Cos(a) + text_size.Height * Math.Sin(a)),
                Math.Abs(text_size.Height * Math.Cos(a) + text_size.Width * Math.Sin(a))
            )
            Dim origin As Vertex = engine.Pipeline(text.Origin, Me.Display) * 72
            Dim format As New XStringFormat With {
                .Alignment = XStringAlignment.Near,
                .LineAlignment = XLineAlignment.Near
            }
            Select Case text.Alignment.Horizontal
                Case Text.TextAlignment.HAlignments.Center
                    origin = origin - New Vertex(text_size_tr.Width / 2, 0)
                Case Text.TextAlignment.HAlignments.Right
                    origin = origin - New Vertex(text_size_tr.Width, 0)
            End Select
            Select Case text.Alignment.Vertical
                Case Text.TextAlignment.VAlignments.Center
                    origin = origin - New Vertex(0, text_size_tr.Height / 2)
                Case Text.TextAlignment.VAlignments.Bottom
                    origin = origin - New Vertex(0, text_size_tr.Height)
            End Select
            origin += New Vertex(text_size_tr.Width / 2, text_size_tr.Height / 2)
            Dim brush As New XSolidBrush(XColor.FromArgb(text.Color.A, text.Color.R, text.Color.G, text.Color.B))
            Dim state As XGraphicsState = Me._graphics.Save
            Me._graphics.TranslateTransform(origin.X, origin.Y)
            Me._graphics.RotateTransform(text.Rotation)
            Me._graphics.DrawString(text.Text, font, brush, New XRect(-text_size.Width / 2, -text_size.Height / 2, text_size.Width, text_size.Height), format)
            Me._graphics.Restore(state)
        End Sub

        Public Overrides Sub Primitive(filled As Filled, engine As Engine)
            Dim pen As New XPen(XColor.FromArgb(filled.LineColor.A, filled.LineColor.R, filled.LineColor.G, filled.LineColor.B), filled.LineWidth)
            Dim brush As New XSolidBrush(XColor.FromArgb(filled.Color.A, filled.Color.R, filled.Color.G, filled.Color.B))
            If filled.Pattern.Count > 1 Then pen.DashPattern = filled.Pattern.Select(Function(s) CDbl(s)).ToArray
            Dim bounds As Vertex() = filled.Bounds.Select(Function(v) engine.Pipeline(v, Me.Display) * 72).ToArray
            Dim rect As New RectangleF(bounds.First.ToPointF, New SizeF((bounds.Last - bounds.First).ToPointF))
            Dim xrect As New XRect(rect.X, rect.Y, rect.Width, rect.Height)
            Select Case filled.FillType
                Case Filled.FillTypes.Circle
                    Me._graphics.DrawEllipse(pen, brush, xrect)
                Case Filled.FillTypes.Rectangle
                    Me._graphics.DrawRectangle(pen, brush, xrect)
            End Select
        End Sub

        Protected Overrides Sub _dispose()
            Me._graphics.Dispose()
            Me._doc.Dispose()
        End Sub

    End Class

End Namespace
