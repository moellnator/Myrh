Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.ModelViews
    Public Class ModelDefault : Inherits ModelView

        Const D2R As Double = Math.PI / 180
        Private _matrix As Matrix

        Public Sub New()
            Me._matrix = Matrix.Identity
        End Sub

        Public Overrides Sub Clear()
            Me._matrix = Matrix.Identity
        End Sub

        Public Overrides Function Clone() As ModelView
            Return New ModelDefault() With {._matrix = Me._matrix.Clone}
        End Function

        Public Overrides Sub Rotate(r As Vertex)
            Dim alpha As Double = Math.Sqrt(r.X ^ 2 + r.Y ^ 2 + r.Z ^ 2)
            If alpha = 0 Then Exit Sub
            Dim n As Vertex = New Vertex(r.X / alpha, r.Y / alpha, r.Z / alpha)
            Dim c As Double = Math.Cos(alpha * D2R) : Dim s As Double = Math.Sin(alpha * D2R)
            Dim m As Double = 1 - c
            Me._matrix = Me._matrix * New Matrix(
                {
                    {n.X ^ 2 * m + c, n.X * n.Y * m - n.Z * s, n.X * n.Z * m + n.Y * s, 0},
                    {n.Y * n.X * m + n.Z * s, n.Y ^ 2 * m + c, n.Y * n.Z * m - n.X * s, 0},
                    {n.Z * n.X * m - n.Y * s, n.Z * n.Y * m + n.X * s, n.Z ^ 2 * m + c, 0},
                    {0, 0, 0, 1}
                }
            )
        End Sub

        Public Overrides Function Transform(v As Vertex) As Vertex
            Return Me._matrix * v
        End Function

        Public Overrides Sub Translate(t As Vertex)
            Me._matrix = Me._matrix * New Matrix({{1, 0, 0, t.X}, {0, 1, 0, t.Y}, {0, 0, 1, t.Z}, {0, 0, 0, 1}})
        End Sub

        Public Overrides Sub Scale(s As Vertex)
            Me._matrix = Me._matrix * New Matrix({{s.X, 0, 0, 0}, {0, s.Y, 0, 0}, {0, 0, s.Z, 0}, {0, 0, 0, 1}})
        End Sub

    End Class

End Namespace
