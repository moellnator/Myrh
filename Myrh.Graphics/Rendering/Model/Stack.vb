Namespace Rendering.Model
    Public Class Stack(Of T)

        Private ReadOnly _data As New List(Of T)
        Private _ptr As Integer = -1

        Public Sub Push(Item As T)
            Me._data.Add(Item)
            Me._ptr += 1
        End Sub

        Public Function Peek() As T
            If _ptr = -1 Then Throw New InvalidOperationException("The stack does not contain any elements.")
            Return Me._data.Item(Me._ptr)
        End Function

        Public Function Pop() As T
            Dim retval As T = Me.Peek
            Me._data.RemoveAt(Me._ptr)
            Me._ptr -= 1
            Return retval
        End Function

    End Class

End Namespace
