Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public MustInherit Class GDI32 : Inherits Context

        Private ReadOnly _graphics As Drawing.Graphics
        Protected ReadOnly Size As Size

        Public Sub New(setup As PageSetup)
            MyBase.New(
                New Display(
                    Math.Round(setup.Device.Width),
                    -Math.Round(setup.Device.Height),
                    setup.Device.Width / 2,
                    setup.Device.Height / 2
                )
            )
            Me.Size = New Size(Math.Round(setup.Device.Width), Math.Round(setup.Device.Height))
            Me._graphics = Me.CreateGraphics()
            Me._graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Me._graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            Me._graphics.Clear(Drawing.Color.Transparent)
        End Sub

        Protected MustOverride Function CreateGraphics() As Drawing.Graphics

        Public Overrides Sub Primitive(line As Lines, engine As Engine)
            Dim pen As New Pen(line.Color, line.Width) With {
                .EndCap = Drawing2D.LineCap.Round
            }
            If line.Pattern.Count > 1 Then pen.DashPattern = line.Pattern
            Me._graphics.DrawLines(pen, (From v As Vertex In line.Verticies Select engine.Pipeline(v, Me.Display).ToPointF).ToArray)
        End Sub

        Public Overrides Sub Primitive(text As Text, engine As Engine)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub Primitive(filled As Filled, engine As Engine)
            Dim pen As New Pen(filled.LineColor, filled.LineWidth)
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
