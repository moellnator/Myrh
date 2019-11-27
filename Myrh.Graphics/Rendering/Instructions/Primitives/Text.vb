Imports Myrh.Graphics.Rendering.Model
Imports Myrh.Graphics.Rendering.Contexts

Namespace Rendering.Instructions.Primitives
    Public Class Text : Inherits Instruction

        Public Class TextAlignment

            Public Enum HAlignments
                Left
                Right
                Center
            End Enum

            Public Enum VAlignments
                Top
                Bottom
                Center
            End Enum

            Public ReadOnly Property Horizontal As HAlignments
            Public ReadOnly Property Vertical As VAlignments

            Public Sub New(Horizontal As HAlignments, Vertical As VAlignments)
                Me.Horizontal = Horizontal
                Me.Vertical = Vertical
            End Sub

        End Class

        Public Sub New(Origin As Vertex, Content As String, Color As Color, Font As Font, Alignment As TextAlignment, Rotation As Double)
            Me.Origin = Origin
            Me.Text = Content
            Me.Color = Color
            Me.Font = Font
            Me.Alignment = Alignment
            Me.Rotation = Rotation
        End Sub

        Public Overrides Sub Execute(Engine As Engine, context As Context)
            context.Primitive(Me, Engine)
        End Sub

        Public ReadOnly Property Origin As Vertex
        Public ReadOnly Property Text As String
        Public ReadOnly Property Color As Color
        Public ReadOnly Property Font As Font
        Public ReadOnly Property Alignment As TextAlignment
        Public ReadOnly Property Rotation As Double

    End Class

End Namespace
