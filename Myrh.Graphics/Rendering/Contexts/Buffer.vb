Imports Myrh.Graphics.Rendering.Instructions
Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public Class Buffer : Inherits Context

        Private ReadOnly _render_list As New List(Of Instruction)
        Public ReadOnly Property RenderList As IEnumerable(Of Instruction)
            Get
                Return Me._render_list
            End Get
        End Property

        Public Overrides ReadOnly Property DefaultExtension As String
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Sub New(base As Context)
            MyBase.New(base.Display)
        End Sub

        Public Overrides Sub Save(Filename As String)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub Primitive(line As Lines, engine As Engine)
            Dim pV As Vertex() = (From v As Vertex In line.Verticies Select engine.Pipeline(v, Me.Display)).ToArray
            Me._render_list.Add(New Lines(pV, line.Color, line.Width, line.Pattern))
        End Sub

        Public Overrides Sub Primitive(text As Text, engine As Engine)
            Dim org As Vertex = engine.Pipeline(text.Origin, Me.Display)
            Me._render_list.Add(New Text(org, text.Text, text.Color, text.Font, text.Alignment, text.Rotation))
        End Sub

        Public Overrides Sub Primitive(filled As Filled, engine As Engine)
            Dim bounds As Vertex() = filled.Bounds.Select(Function(v) engine.Pipeline(v, Me.Display)).ToArray
            Me._render_list.Add(New Filled(bounds, filled.FillType, filled.Color, filled.LineColor, filled.LineWidth, filled.Pattern))
        End Sub

        Protected Overrides Sub _dispose()
        End Sub

    End Class

End Namespace
