Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Instructions
    Public Class Rotate : Inherits Instruction

        Private ReadOnly _r As Vertex

        Public Sub New(R As Vertex)
            Me._r = R
        End Sub

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            Engine.ModelView.Rotate(Me._r)
        End Sub

    End Class

End Namespace