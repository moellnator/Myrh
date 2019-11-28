Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public Class ContextFactory : Implements IContextFactory

        Protected _context_type As Type

        Protected Sub New(t As Type)
            Me._context_type = t
        End Sub

        Public Function CreateContext(setup As PageSetup) As Context Implements IContextFactory.CreateContext
            Dim retval As Context = Nothing
            Select Case Me._context_type
                Case GetType(Bitmap)
                    retval = New Bitmap(setup)
                Case GetType(Metafile)
                    retval = New Metafile(setup)
                Case GetType(Vectormap)
                    retval = New Vectormap(setup)
                Case GetType(DocumentFile)
                    retval = New DocumentFile(setup)
                Case Else
                    Throw New InvalidOperationException("Unkown context type provided for class creation.")
            End Select
            Return retval
        End Function

        Public Shared ReadOnly Property Bitmap As ContextFactory
            Get
                Return New ContextFactory(GetType(Bitmap))
            End Get
        End Property

        Public Shared ReadOnly Property MetaFile As ContextFactory
            Get
                Return New ContextFactory(GetType(Metafile))
            End Get
        End Property

        Public Shared ReadOnly Property Vectormap As ContextFactory
            Get
                Return New ContextFactory(GetType(Vectormap))
            End Get
        End Property

        Public Shared ReadOnly Property DocumentFile As ContextFactory
            Get
                Return New ContextFactory(GetType(DocumentFile))
            End Get
        End Property
    End Class

End Namespace
