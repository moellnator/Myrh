Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Projections
    Public Class P2DFlat : Inherits Projection

        Private ReadOnly _l As Double
        Private ReadOnly _r As Double
        Private ReadOnly _t As Double
        Private ReadOnly _b As Double

        Public Sub New(Width As Single, Height As Single)
            Me._r = Width
            Me._l = -Me._r
            Me._t = Height
            Me._b = -Me._t
        End Sub

        Public Overrides Function Project(v As Vertex) As Vertex
            Return _clip(New Vertex(v.X / (Me._r - Me._l), v.Y / (Me._t - Me._b), 0, 1))
        End Function

        Private Function _clip(v As Vertex) As Vertex
            Return New Vertex(Math.Max(Math.Min(0.5, v.X), -0.5), Math.Max(Math.Min(0.5, v.Y), -0.5))
        End Function

    End Class

End Namespace
