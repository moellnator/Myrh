Namespace Rendering.Model
    Public Class Vertex

        Public Sub New(z As Single)
            Me.New(0, 0, z, 1)
        End Sub

        Public Sub New(x As Single, y As Single)
            Me.New(x, y, 0, 1)
        End Sub

        Public Sub New(x As Single, y As Single, z As Single)
            Me.New(x, y, z, 1)
        End Sub

        Public Sub New(x As Single, y As Single, z As Single, w As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
            Me.W = w
        End Sub

        Public ReadOnly Property X As Single
        Public ReadOnly Property Y As Single
        Public ReadOnly Property Z As Single
        Public ReadOnly Property W As Single

        Public ReadOnly Property Data As Single()
            Get
                Return {Me._X, Me._Y, Me._Z, Me._W}
            End Get
        End Property

        Public Function ToPointF() As PointF
            Return New PointF(Me._X, Me._Y)
        End Function

        Public Shared Operator -(a As Vertex, b As Vertex) As Vertex
            Return New Vertex(a.X - b.X, a.Y - b.Y, a.Z - b.Z, 1)
        End Operator

        Public Shared Operator -(a As Vertex) As Vertex
            Return New Vertex(-a.X, -a.Y, -a.Z, 1)
        End Operator

        Public Shared Operator +(a As Vertex, b As Vertex) As Vertex
            Return New Vertex(a.X + b.X, a.Y + b.Y, a.Z + b.Z, 1)
        End Operator

        Public Shared Operator *(a As Vertex, r As Double) As Vertex
            Return New Vertex(a.X * r, a.Y * r, a.Z * r, 1)
        End Operator

        Public Shared Operator *(r As Double, a As Vertex) As Vertex
            Return a * r
        End Operator

        Public Function ScaledBy(scale As Vertex) As Vertex
            Return New Vertex(Me.X * scale.X, Me.Y * scale.Y, Me.Z * scale.Z, 1)
        End Function

        Public Function Dot(other As Vertex) As Double
            Return Me.X * other.X + Me.Y * other.Y + Me.Z * other.Z
        End Function

        Public Function Cross(other As Vertex) As Vertex
            Return New Vertex(Me.Y * other.Z - Me.Z * other.Y, Me.Z * other.X - Me.X * other.Z, Me.X * other.Y - Me.Y * other.X, 1)
        End Function

        Public ReadOnly Property Length As Double
            Get
                Return Math.Sqrt(Me.X ^ 2 + Me.Y ^ 2 + Me.Z ^ 2)
            End Get
        End Property

        Public Function Normalized() As Vertex
            Return 1 / Me.Length * Me
        End Function

    End Class

End Namespace
