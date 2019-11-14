Namespace Entities
    Public Class Domain
        Public Shared ReadOnly Property Current As New Domain

        Private _default_system As String
        Public ReadOnly Property DefaultSystem
            Get
                Return Me._default_system
            End Get
        End Property

        Public Sub SetDefaultSystem(system As String)
            Me._default_system = system
        End Sub

        Public ReadOnly Property Units As New List(Of Unit)
        Public ReadOnly Property Links As New List(Of Link)
        Public ReadOnly Property Quantities As New List(Of Quantity)

        Public Sub Reset()
            Me._default_system = ""
            Me.Units.Clear()
            Me.Links.Clear()
            Me.Quantities.Clear()
        End Sub

    End Class

End Namespace
