Imports Myrh.Graphics.Rendering.Contexts

Namespace Rendering.Instructions
    Public Class ClearModelView : Inherits Instruction

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            Engine.ModelView.Clear()
        End Sub

    End Class

End Namespace