Imports Myrh.Entities.Model
Imports SimpleTexUnicode

Namespace Entities
    Public Class Dimension : Inherits Compound(Of Dimension)

        Private Shared ReadOnly _Dimensions As Dimension() = {
            Length,
            Mass,
            Time,
            Temperature,
            ElectricCurrent,
            LuminousIntensity,
            AmoundOfSubstance,
            One
        }

        Private Sub New(name As String, symbol As String)
            MyBase.New(name, symbol)
        End Sub

        Private Sub New(components As IEnumerable(Of Component(Of Dimension)))
            MyBase.New(_FilterEmpty(components, One))
        End Sub

        Protected Overrides Function CreateNew(components As IEnumerable(Of Component(Of Dimension))) As Dimension
            Return New Dimension(components)
        End Function

        Public Shared ReadOnly Property Length As Dimension
            Get
                Return New Dimension("length", "L")
            End Get
        End Property

        Public Shared ReadOnly Property Mass As Dimension
            Get
                Return New Dimension("mass", "M")
            End Get
        End Property

        Public Shared ReadOnly Property Time As Dimension
            Get
                Return New Dimension("time", "T")
            End Get
        End Property

        Public Shared ReadOnly Property Temperature As Dimension
            Get
                Return New Dimension("temperature", SimpleTex.LatexToUnicode("\theta"))
            End Get
        End Property

        Public Shared ReadOnly Property ElectricCurrent As Dimension
            Get
                Return New Dimension("electric current", "I")
            End Get
        End Property

        Public Shared ReadOnly Property AmoundOfSubstance As Dimension
            Get
                Return New Dimension("amount of substance", "N")
            End Get
        End Property

        Public Shared ReadOnly Property LuminousIntensity As Dimension
            Get
                Return New Dimension("luminous intensity", "J")
            End Get
        End Property

        Public Shared ReadOnly Property One As Dimension
            Get
                Return New Dimension("one", "1")
            End Get
        End Property

        Public Shared Shadows Function Parse(text As String) As Dimension
            Dim retval As New Dimension({})
            For Each component As Tuple(Of String, Double) In Compound(Of Dimension).Parse(text)
                Dim base_name As String = component.Item1
                Dim base As Dimension = _Dimensions.First(Function(d) base_name = If(base_name.Contains("("), d.Name, d.Symbol))
                retval *= base ^ component.Item2
            Next
            Return retval
        End Function

    End Class


End Namespace
