Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.ModelViews
    Public MustInherit Class ModelView : Implements ICloneable

        Public Shared ReadOnly Property [Default] As ModelView
            Get
                Return New ModelDefault
            End Get
        End Property

        Public MustOverride Sub Clear()
        Public MustOverride Sub Translate(v As Vertex)
        Public MustOverride Sub Rotate(r As Vertex)

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.Clone
        End Function

        Public MustOverride Function Clone() As ModelView
        Public MustOverride Function Transform(v As Vertex) As Vertex
        Public MustOverride Sub Scale(s As Vertex)

    End Class

End Namespace
