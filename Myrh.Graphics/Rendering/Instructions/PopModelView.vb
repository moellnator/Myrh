Imports Myrh.Graphics.Rendering.Contexts

Namespace Rendering.Instructions
    Public Class PopModelView : Inherits Instruction

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            Engine.Pop_ModelView()
        End Sub

    End Class

End Namespace