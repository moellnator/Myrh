Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Projections
    Public Class P3DOblique : Inherits Projection

        Private ReadOnly _l As Single
        Private ReadOnly _r As Single
        Private ReadOnly _n As Single
        Private ReadOnly _f As Single
        Private ReadOnly _t As Single
        Private ReadOnly _b As Single
        Private ReadOnly _cphi As Single
        Private ReadOnly _ctheta As Single

        Public Sub New(Max As Single, Aspect As Single, Near As Single, Far As Single, Theta As Single, Phi As Single)
            Me._n = Near
            Me._f = Far
            Me._t = Max
            Me._b = -Me._t
            Me._r = Me._t * Aspect
            Me._l = -Me._r
            Me._ctheta = 1 / Math.Tan(Theta / 180 * Math.PI)
            Me._cphi = 1 / Math.Tan(Phi / 180 * Math.PI)
        End Sub

        Public Overrides Function Project(v As Vertex) As Vertex
            Dim c As New Vertex(
                v.X * 2 / (_r - _l) - v.W * (_r + _l) / (_r - _l) - v.Z * 2 * _ctheta / (_r - _l),
                v.Y * 2 / (_t - _b) - v.W * (_t + _b) / (_t - _b) - v.Z * 2 * _cphi / (_t - _b),
                -v.Z * 2 / (_f - _n) - v.W * (_f + _n) / (_f - _n),
                -v.W
            )
            Return New Vertex(_clip(c.X), _clip(c.Y), _clip(c.Z))
        End Function

        Private Function _clip(d As Single) As Single
            If d > 1 Then
                Return 1
            ElseIf d < -1 Then
                Return -1
            Else
                Return d
            End If
        End Function

    End Class

End Namespace
