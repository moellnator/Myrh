Namespace Rendering.Model
    Public Class Display

        Private ReadOnly _s_x As Single
        Private ReadOnly _o_x As Single
        Private ReadOnly _s_y As Single
        Private ReadOnly _o_y As Single

        Public Sub New(ScaleX As Single, ScaleY As Single, OffsetX As Single, OffsetY As Single)
            Me._s_x = ScaleX
            Me._s_y = ScaleY
            Me._o_x = OffsetX
            Me._o_y = OffsetY
        End Sub

        Public Function Correct(v As Vertex) As Vertex
            Return New Vertex(v.X * Me._s_x + Me._o_x, v.Y * Me._s_y + Me._o_y, v.Z)
        End Function

    End Class

End Namespace
