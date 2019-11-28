Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public Class Vectormap : Inherits Context

        Private Const FONT_CORR As Single = 14 / 12
        Protected ReadOnly Setup As PageSetup
        Private ReadOnly _doc As XDocument

        Public Sub New(setup As PageSetup)
            MyBase.New(
                New Display(
                    setup.Device.Width,
                    -setup.Device.Height,
                    setup.Device.Width / 2,
                    setup.Device.Height / 2
                )
            )
            Me.Setup = setup
            Me._doc =
                <?xml version="1.0"?>
                <svg
                    width=<%= setup.Context.Width.ToString.Replace(",", ".") & "in" %>
                    height=<%= setup.Context.Height.ToString.Replace(",", ".") & "in" %>
                    viewBox=<%= "0 0 " & (setup.Device.Width & " " & setup.Device.Height).Replace(",", ".") %>>
                </svg>
        End Sub

        Public Overrides ReadOnly Property DefaultExtension As String
            Get
                Return "svg"
            End Get
        End Property

        Public Overrides Sub Primitive(line As Lines, engine As Engine)
            Dim verts As Vertex() = (From v As Vertex In line.Verticies Select engine.Pipeline(v, Me.Display)).ToArray
            Dim xline As XElement =
                <polyline
                    points=<%= String.Join(", ", From v As Vertex In verts Select v.X.ToString.Replace(",", ".") & ", " & v.Y.ToString.Replace(",", ".")) %>
                    fill="none"
                    stroke-width=<%= line.Width.ToString.Replace(",", ".") & "pt" %>
                    stroke=<%= line.Color.SolidRGB %>
                    stroke-opacity=<%= line.Color.Alpha.ToString.Replace(",", ".") %>
                    stroke-dasharray=<%= If(line.Pattern.Count = 1, "", String.Join(", ", From i As Single In line.Pattern Select i.ToString.Replace(",", "."))) %>
                    stroke-linecap="round"
                />
            Me._doc.Root.Add(xline)
        End Sub

        Public Overrides Sub Primitive(text As Text, engine As Engine)
            Dim font_size As Single = text.Font.SizeInPoints / 72 * Setup.DPI
            Dim a As Double = text.Rotation / 180 * Math.PI
            Dim font As New Font(text.Font.FontFamily, font_size, text.Font.Style, GraphicsUnit.Pixel)
            Dim text_size As SizeF = TextRenderer.MeasureText(text.Text, font)
            Dim text_size_tr As New SizeF(
                Math.Abs(text_size.Width * Math.Cos(a) + text_size.Height * Math.Sin(a)),
                Math.Abs(text_size.Height * Math.Cos(a) + text_size.Width * Math.Sin(a))
            )
            Dim origin As Vertex = engine.Pipeline(text.Origin, Me.Display)
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
            Dim xtext As XElement =
                <text
                    x=<%= 0 %>
                    y=<%= 0 %>
                    style=<%= "font-family: " & text.Font.FontFamily.Name & ";" &
                         "font-size: " & Math.Round(font_size * FONT_CORR).ToString.Replace(",", ".") & " px" & ";" &
                         "stroke: none;" &
                         If(text.Font.Style And FontStyle.Italic = 1, "font-style: italic;", "") &
                         If(text.Font.Style And FontStyle.Bold = 1, "font-weight: bold;", "") &
                         "fill: " & text.Color.SolidRGB & ";" %>
                    opacity=<%= text.Color.Alpha.ToString.Replace(",", ".") %>
                    text-anchor=<%= "start" %>
                    dominant-baseline=<%= "hanging" %>
                    transform=<%= "translate(" & (origin.X - text_size.Width / 2).ToString.Replace(",", ".") & " " &
                                                 (origin.Y - text_size.Height / 2).ToString.Replace(",", ".") & ") " &
                                  "rotate(" & text.Rotation.ToString.Replace(",", ".") & "," &
                                              (text_size.Width / 2).ToString.Replace(",", ".") & "," &
                                              (text_size.Height / 2).ToString.Replace(",", ".") & ")" %>
                    ><%= text.Text %>
                </text>
            Me._doc.Root.Add(xtext)
        End Sub

        Public Overrides Sub Primitive(filled As Filled, engine As Engine)
            Dim bounds As Vertex() = filled.Bounds.Select(Function(v) engine.Pipeline(v, Me.Display)).ToArray
            Dim rect As New RectangleF(bounds.First.ToPointF, New SizeF((bounds.Last - bounds.First).ToPointF))
            Dim xfill As XElement =
                <<%= If(filled.FillType = Filled.FillTypes.Circle, "ellipse", "rect") %>
                    cx=<%= (rect.X + rect.Width / 2).ToString.Replace(",", ".") %>
                    cy=<%= (rect.Y + rect.Height / 2).ToString.Replace(",", ".") %>
                    rx=<%= (rect.Width / 2).ToString.Replace(",", ".") %>
                    ry=<%= (rect.Height / 2).ToString.Replace(",", ".") %>
                    fill=<%= filled.Color.SolidRGB %>
                    opacity=<%= filled.Color.Alpha.ToString.Replace(",", ".") %>
                    stroke-width=<%= filled.LineWidth.ToString.Replace(",", ".") & " pt" %>
                    stroke=<%= filled.LineColor.SolidRGB %>
                    stroke-opacity=<%= filled.LineColor.Alpha.ToString.Replace(",", ".") %>
                    stroke-dasharray=<%= If(filled.Pattern.Count = 1, "", String.Join(", ", From i As Single In filled.Pattern Select i.ToString.Replace(",", "."))) %>
                />
            Me._doc.Root.Add(xfill)
        End Sub

        Public Overrides Sub Save(Filename As String)
            Me._doc.Save(Filename)
        End Sub

        Protected Overrides Sub _dispose()
        End Sub

    End Class

End Namespace
