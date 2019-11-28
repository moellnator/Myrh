Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public MustInherit Class GDI32 : Inherits Context

        Private ReadOnly _graphics As Drawing.Graphics
        Protected ReadOnly Setup As PageSetup

        Public Sub New(setup As PageSetup)
            MyBase.New(
                New Display(
                    Math.Round(setup.Device.Width),
                    -Math.Round(setup.Device.Height),
                    setup.Device.Width / 2,
                    setup.Device.Height / 2
                )
            )
            Me.Setup = setup
            Me._graphics = Me.CreateGraphics()
            Me._graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Me._graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            Me._graphics.Clear(Drawing.Color.Transparent)
        End Sub

        Protected MustOverride Function CreateGraphics() As Drawing.Graphics

        Public Overrides Sub Primitive(line As Lines, engine As Engine)
            Dim pen As New Pen(line.Color, line.Width / 72 * Setup.DPI) With {
                .EndCap = Drawing2D.LineCap.Round,
                .StartCap = Drawing2D.LineCap.Round
            }
            If line.Pattern.Count > 1 Then pen.DashPattern = line.Pattern
            Me._graphics.DrawLines(pen, (From v As Vertex In line.Verticies Select engine.Pipeline(v, Me.Display).ToPointF).ToArray)
        End Sub

        Public Overrides Sub Primitive(text As Text, engine As Engine)
            Dim font_size As Single = text.Font.SizeInPoints / 72 * Setup.DPI
            Dim font As New Font(text.Font.FontFamily, font_size, text.Font.Style, GraphicsUnit.Pixel)
            Dim a As Double = text.Rotation / 180 * Math.PI
            Dim text_size As SizeF = Me._graphics.MeasureString(text.Text, font)
            Dim text_size_tr As New SizeF(
                Math.Abs(text_size.Width * Math.Cos(a) + text_size.Height * Math.Sin(a)),
                Math.Abs(text_size.Height * Math.Cos(a) + text_size.Width * Math.Sin(a))
            )
            Dim origin As Vertex = engine.Pipeline(text.Origin, Me.Display)
            Dim brush As New SolidBrush(text.Color)
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
            origin += New Vertex(text_size_tr.Width, text_size_tr.Height) * 0.5
            Me._graphics.TranslateTransform(origin.X, origin.Y)
            Me._graphics.RotateTransform(text.Rotation)
            Me._graphics.DrawString(text.Text, font, brush, New PointF(-text_size.Width / 2, -text_size.Height / 2))
            Me._graphics.ResetTransform()
        End Sub

        Public Overrides Sub Primitive(filled As Filled, engine As Engine)
            Dim pen As New Pen(filled.LineColor, filled.LineWidth / 72 * Setup.DPI)
            If filled.Pattern.Count > 1 Then pen.DashPattern = filled.Pattern
            Dim brush As New SolidBrush(filled.Color)
            Dim bounds As Vertex() = filled.Bounds.Select(Function(v) engine.Pipeline(v, Me.Display)).ToArray
            Dim rect As New RectangleF(bounds.First.ToPointF, New SizeF((bounds.Last - bounds.First).ToPointF))
            Select Case filled.FillType
                Case Filled.FillTypes.Circle
                    Me._graphics.FillEllipse(brush, rect)
                    Me._graphics.DrawEllipse(pen, rect)
                Case Filled.FillTypes.Rectangle
                    Me._graphics.FillRectangle(brush, rect)
                    Me._graphics.DrawRectangles(pen, {rect})
            End Select
        End Sub

        Protected Overrides Sub _dispose()
            Me._graphics.Dispose()
        End Sub
    End Class

End Namespace
