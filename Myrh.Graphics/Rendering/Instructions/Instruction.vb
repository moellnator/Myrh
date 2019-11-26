Imports Myrh.Graphics.Rendering.Contexts

Namespace Rendering.Instructions
    Public MustInherit Class Instruction : Implements IEquatable(Of Instruction)

        Private Shared __current_id As Integer = 0
        Private Shared Function __next_id() As Integer
            Dim retval As Integer = __current_id
            __current_id += 1
            Return retval
        End Function

        Public ReadOnly Property ID As Integer

        Public MustOverride Sub Execute(Engine As Engine, context As Context)

        Protected Sub New()
            Me.ID = __next_id()
        End Sub

        Private Function IEquatable_Equals(other As Instruction) As Boolean Implements IEquatable(Of Instruction).Equals
            Return Me.ID = other.ID
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If GetType(Instruction).IsAssignableFrom(obj.GetType) Then
                Return Me.IEquatable_Equals(obj)
            Else
                Return MyBase.Equals(obj)
            End If
        End Function

    End Class

End Namespace
