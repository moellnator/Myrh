Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Instructions
Imports Myrh.Graphics.Rendering.Model
Imports Myrh.Graphics.Rendering.ModelViews
Imports Myrh.Graphics.Rendering.Projections

Namespace Rendering
    Public Class Document

        Private ReadOnly _name As String
        Private ReadOnly _engine As Engine
        Private ReadOnly _page As PageSetup
        Private ReadOnly _contexts As New List(Of Context)

        Public Sub New(name As String, page As PageSetup, modelView As ModelView, projection As Projection)
            Me._page = page
            Me._name = name
            Me._engine = New Engine(modelView, projection)
        End Sub

        Public Sub Execute(instructions As IEnumerable(Of Instruction), Optional fullPipeline As Boolean = False)
            For Each c As Context In Me._contexts
                If fullPipeline Then
                    Me._engine.ExecuteFull(instructions, c)
                Else
                    Me._engine.Execute(instructions, c)
                End If
            Next
        End Sub

        Public Sub Save(documentFolder As String)
            For Each c As Context In Me._contexts
                c.Save(IO.Path.Combine(documentFolder, $"{Me._name}.{c.DefaultExtension}"))
            Next
        End Sub

        Public Sub AttachContext(factory As IContextFactory)
            Me._contexts.Add(factory.CreateContext(Me._page))
        End Sub

        Public Sub AttachContexts(factory As IEnumerable(Of IContextFactory))
            For Each f As IContextFactory In factory
                Me.AttachContext(f)
            Next
        End Sub

        Public ReadOnly Property AttachedContexts As IEnumerable(Of Context)
            Get
                Return Me._contexts
            End Get
        End Property

    End Class

End Namespace
