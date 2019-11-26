Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Instructions.Primitives
    Public Class Filled : Inherits Instruction

        Public Enum FillTypes
            Rectangle
            Circle
        End Enum

        Public Sub New(Location As Vertex, Size As Vertex, FillType As FillTypes, Color As Color, LineColor As Color, Width As Single, Pattern As Single())
            Me.New({
                New Vertex(Location.X - Size.X / 2, Location.Y + Size.Y / 2),
                New Vertex(Location.X + Size.X / 2, Location.Y - Size.Y / 2)
            }, FillType, Color, LineColor, Width, Pattern)
        End Sub

        Public Sub New(Bounds As Vertex(), FillType As FillTypes, Color As Color, LineColor As Color, Width As Single, Pattern As Single())
            Me.Bounds = Bounds
            Me.Color = Color
            Me.LineColor = LineColor
            Me.LineWidth = Width
            Me.Pattern = Pattern
            Me.FillType = FillType
        End Sub

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            context.Primitive(Me, Engine)
        End Sub

        Public ReadOnly Property Bounds As Vertex()
        Public ReadOnly Property Color As Color
        Public ReadOnly Property LineWidth As Single
        Public ReadOnly Property Pattern As Single()
        Public ReadOnly Property LineColor As Color
        Public ReadOnly Property [FillType] As FillTypes

    End Class

End Namespace
