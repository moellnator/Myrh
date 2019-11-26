Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Instructions
Imports Myrh.Graphics.Rendering.Model
Imports Myrh.Graphics.Rendering.ModelViews
Imports Myrh.Graphics.Rendering.Projections

Namespace Rendering

    Public Class Engine

        Protected _model_view As ModelView
        Protected _model_view_stack As Stack(Of ModelView)
        Protected _projection As Projection

        Private _skip_pipeline As Boolean = False

        Public ReadOnly Property Preprocessing As New List(Of Preprocessing.Preprocessor) From {New Preprocessing.ZOrdering}

        Public Sub New(ModelView As ModelView, Projection As Projection)
            Me._model_view = ModelView
            Me._model_view_stack = New Stack(Of ModelView)
            Me._projection = Projection
        End Sub

        Public Sub Push_ModelView()
            Me._model_view_stack.Push(Me._model_view)
            Me._model_view = Me._model_view.Clone
        End Sub

        Public Sub Pop_ModelView()
            Me._model_view = Me._model_view_stack.Pop
        End Sub

        Public ReadOnly Property ModelView As ModelView
            Get
                Return Me._model_view
            End Get
        End Property

        Public ReadOnly Property Projection As Projection
            Get
                Return Me._projection
            End Get
        End Property

        Public Function Pipeline(v As Vertex, display As Display) As Vertex
            If Me._skip_pipeline Then Return v
            Return display.Correct(Me.Projection.Project(Me.ModelView.Transform(v)))
        End Function

        Public Sub Execute(instructions As IEnumerable(Of Instruction), context As Context)
            For Each instruction In instructions
                instruction.Execute(Me, context)
            Next
        End Sub

        Public Sub ExecuteFull(instructions As IEnumerable(Of Instruction), context As Context)
            Dim prelist As IEnumerable(Of Instruction)
            Dim buffer As New Buffer(context)
            Me.Execute(instructions, buffer)
            prelist = buffer.RenderList
            For Each p As Preprocessing.Preprocessor In Me.Preprocessing
                prelist = p.Process(prelist)
            Next
            Me._skip_pipeline = True
            Me.Execute(prelist, context)
            Me._skip_pipeline = False
        End Sub

    End Class

End Namespace
