Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Instructions.Primitives
    Public Class Lines : Inherits Instruction

        Public Sub New(Verticies As IEnumerable(Of Vertex), Color As Color, Width As Single, Pattern As Single())
            Me.Verticies = Verticies
            Me.Color = Color
            Me.Width = Width
            Me.Pattern = Pattern
        End Sub

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            context.Primitive(Me, Engine)
        End Sub

        Public ReadOnly Property Verticies As IEnumerable(Of Vertex)
        Public ReadOnly Property Color As Color
        Public ReadOnly Property Width As Single
        Public ReadOnly Property Pattern As Single()

    End Class

End Namespace
