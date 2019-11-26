Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Instructions
    Public Class Translate : Inherits Instruction

        Private ReadOnly _v As Vertex

        Public Sub New(V As Vertex)
            Me._v = V
        End Sub

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            Engine.ModelView.Translate(Me._v)
        End Sub

    End Class

End Namespace
