Imports Myrh.Graphics.Rendering.Contexts

Namespace Rendering.Instructions
    Public Class PushModelView : Inherits Instruction

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            Engine.Push_ModelView()
        End Sub

    End Class

End Namespace