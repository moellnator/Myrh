Namespace Rendering.Model
    Public Class Matrix : Implements ICloneable

        Private _data As Single(,)

        Public Sub New()
            ReDim Me._data(3, 3)
        End Sub

        Public Sub New(Data As Single(,))
            If Data.GetUpperBound(0) <> 3 Or Data.GetUpperBound(1) <> 3 Then Throw New ArgumentException("Viewmatrix must by a four by four entry double array.")
            Me._data = Data
        End Sub

        Public Shared ReadOnly Property Identity As Matrix
            Get
                Return New Matrix({{1.0, 0.0, 0.0, 0.0}, {0.0, 1.0, 0.0, 0.0}, {0.0, 0.0, 1.0, 0.0}, {0.0, 0.0, 0.0, 1.0}})
            End Get
        End Property

        Public Overloads Shared Operator *(A As Matrix, B As Matrix) As Matrix
            Dim retval As Single(,) : ReDim retval(3, 3)
            For i As Integer = 0 To 3
                For j As Integer = 0 To 3
                    For k As Integer = 0 To 3
                        retval(i, j) += A._data(i, k) * B._data(k, j)
                    Next
                Next
            Next
            Return New Matrix(retval)
        End Operator

        Public Overloads Shared Operator *(A As Matrix, v As Vertex) As Vertex
            Dim retval As Single() : ReDim retval(3)
            For i As Integer = 0 To 3
                For j As Integer = 0 To 3
                    retval(i) += A._data(i, j) * v.Data(j)
                Next
            Next
            Return New Vertex(retval(0), retval(1), retval(2), retval(3))
        End Operator

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.Clone
        End Function

        Public Function Clone() As Matrix
            Dim retval As Single(,) : ReDim retval(3, 3)
            Array.Copy(Me._data, retval, Me._data.Length)
            Return New Matrix(retval)
        End Function

    End Class

End Namespace