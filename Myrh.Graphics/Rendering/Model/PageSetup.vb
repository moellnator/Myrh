
Namespace Rendering.Model
    Public Class PageSetup

        Public Class DIN

            Public Shared ReadOnly Property A4 As PageSetup
                Get
                    Return New PageSetup(New SizeF(8.3F, 11.7F), New SizeF(1.0, 1.0), 300)
                End Get
            End Property

        End Class

        Public ReadOnly Property DPI As Single
        Public ReadOnly Property Size As New SizeF
        Public ReadOnly Property PageCropping As SizeF

        Public Sub New(size As SizeF, cropping As SizeF, dpi As Single)
            Me.Size = size
            Me.PageCropping = cropping
            Me.DPI = dpi
        End Sub

        Public ReadOnly Property Device() As SizeF
            Get
                Return New SizeF(Me.Context.Width * Me.DPI, Me.Context.Height * DPI)
            End Get
        End Property

        Public ReadOnly Property Context() As SizeF
            Get
                Return New SizeF(Me.Size.Width * Me.PageCropping.Width, Me.Size.Height * Me.PageCropping.Height)
            End Get
        End Property

        Public ReadOnly Property AspectRatio As Single
            Get
                Return Size.Width * Me.PageCropping.Width / (Size.Height * Me.PageCropping.Height)
            End Get
        End Property

        Public Function Flipped() As PageSetup
            Return New PageSetup(
                New SizeF(Me.Size.Height, Me.Size.Width),
                New SizeF(Me.PageCropping.Height, Me.PageCropping.Width),
                Me.DPI
            )
        End Function

        Public Function Cropped(cropping As SizeF) As PageSetup
            Return New PageSetup(
                Me.Size,
                New SizeF(Me.PageCropping.Width * cropping.Width, Me.PageCropping.Height * cropping.Height),
                Me.DPI
            )
        End Function

        Public Function Resolution(dpi As Double) As PageSetup
            Return New PageSetup(
                Me.Size,
                Me.PageCropping,
                dpi
            )
        End Function

    End Class

End Namespace
