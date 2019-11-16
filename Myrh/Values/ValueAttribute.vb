Namespace Values
    Public Class ValueAttribute : Inherits Attribute
        Public ReadOnly Property Pattern As String
        Public Sub New(pattern As String)
            Me.Pattern = pattern
        End Sub

    End Class

End Namespace