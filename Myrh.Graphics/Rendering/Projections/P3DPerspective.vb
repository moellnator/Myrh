Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Projections
    Public Class P3DPerspective : Inherits Projection

        Private ReadOnly _l As Single
        Private ReadOnly _r As Single
        Private ReadOnly _n As Single
        Private ReadOnly _f As Single
        Private ReadOnly _t As Single
        Private ReadOnly _b As Single

        Public Sub New(FOV As Single, Aspect As Single, Near As Single, Far As Single)
            Me._n = Near
            Me._f = Far
            Me._t = Math.Tan(FOV / 180 / 2 * Math.PI) * Near
            Me._b = -Me._t
            Me._r = Me._t * Aspect
            Me._l = -Me._r
        End Sub

        Public Overrides Function Project(v As Vertex) As Vertex
            Dim c As New Vertex(
                v.X * _n / _r,
                v.Y * _n / _t,
                v.Z * -(_f + _n) / (_f - _n) + v.W * -2 * _f * _n / (_f - _n),
                -v.Z
            )
            Return New Vertex(_clip(c.X, c.W), _clip(c.Y, c.W), _clip(c.Z, c.W))
        End Function

        Private Function _clip(d As Single, c As Single) As Single
            If d > c Then
                Return 1
            ElseIf d < -c Then
                Return -1
            Else
                Return d / c
            End If
        End Function

    End Class

End Namespace
