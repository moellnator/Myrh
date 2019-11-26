Namespace Rendering.Model
    Public Class Color

        Public Shared Function FromHTML(html As String) As Color
            Dim values As Byte() = (From i As Integer In Enumerable.Range(0, (html.Count - 1) \ 2) Select CByte(Val("&H" & html.Substring(i * 2 + 1, 2)))).ToArray
            Return New Color(
                values(0) / 255.0F,
                values(1) / 255.0F,
                values(2) / 255.0F,
                values(3) / 255.0F
            )
        End Function

        Public Shared Function FromARGB(A As Byte, R As Byte, G As Byte, B As Byte) As Color
            Return New Color(
                A / 255.0F,
                R / 255.0F,
                G / 255.0F,
                B / 255.0F
            )
        End Function

        Public Shared Function FromRGBA(value As Integer) As Color
            Return New Color(
                ((value >> 24) And 255) / 255.0F,
                (value And 255) / 255.0F,
                ((value >> 8) And 255) / 255.0F,
                ((value >> 16) And 255) / 255.0F
            )
        End Function

        Public Sub New(Alpha As Single, Red As Single, Green As Single, Blue As Single)
            Me.Alpha = _clamp(Alpha)
            Me.Red = _clamp(Red)
            Me.Green = _clamp(Green)
            Me.Blue = _clamp(Blue)
        End Sub

        Public Sub New(Alpha As Single, Color As Color)
            Me.Alpha = _clamp(Alpha)
            Me.Red = Color.Red
            Me.Green = Color.Green
            Me.Blue = Color.Blue
        End Sub

        Public ReadOnly Property Alpha As Single
        Public ReadOnly Property Red As Single
        Public ReadOnly Property Green As Single
        Public ReadOnly Property Blue As Single

        Public ReadOnly Property RGBA As Integer
            Get
                Return (Me.A << 24) Or (Me.B << 16) Or (Me.G << 8) Or Me.R
            End Get
        End Property

        Public ReadOnly Property HTML As String
            Get
                Return "#" & _to_hex(Me.Alpha) & _to_hex(Me.Red) & _to_hex(Me.Green) & _to_hex(Me.Blue)
            End Get
        End Property

        Public ReadOnly Property SolidRGB As String
            Get
                Return "#" & _to_hex(Me.Red) & _to_hex(Me.Green) & _to_hex(Me.Blue)
            End Get
        End Property

        Private Shared Function _clamp(Value As Single) As Single
            Return Math.Max(Math.Min(Value, 1.0), 0.0)
        End Function

        Private Shared Function _clamp_angle(angle As Single) As Single
            If angle < 0 Then
                Return (Math.Floor(-angle / 360) * 360 + angle) Mod 360
            Else
                Return angle Mod 360
            End If
        End Function

        Private Function _to_hex(Value As Single) As String
            Return Hex(Value * 255).PadLeft(2, "0"c)
        End Function

        Public Shared Widening Operator CType(color As Drawing.Color) As Color
            Return FromARGB(color.A, color.R, color.G, color.B)
        End Operator

        Public Shared Narrowing Operator CType(color As Color) As Drawing.Color
            Return Drawing.Color.FromArgb(color.Alpha * 255, color.Red * 255, color.Green * 255, color.Blue * 255)
        End Operator

        Public Shared Function FromAHSB(alpha As Single, hue As Single, saturation As Single, brightness As Single) As Color
            Dim r As Double = 0
            Dim g As Double = 0
            Dim b As Double = 0
            If saturation = 0 Then
                r = brightness
                g = brightness
                b = brightness
            Else
                Dim sectorPos As Double = hue / 60.0
                Dim sectorNumber As Integer = Math.Floor(sectorPos)
                Dim fractionalSector As Double = sectorPos - sectorNumber
                Dim p As Double = brightness * (1.0 - saturation)
                Dim q As Double = brightness * (1.0 - (saturation * fractionalSector))
                Dim t As Double = brightness * (1.0 - (saturation * (1 - fractionalSector)))
                Select Case sectorNumber
                    Case 0
                        r = brightness
                        g = t
                        b = p
                    Case 1
                        r = q
                        g = brightness
                        b = p
                    Case 2
                        r = p
                        g = brightness
                        b = t
                    Case 3
                        r = p
                        g = q
                        b = brightness
                    Case 4
                        r = t
                        g = p
                        b = brightness
                    Case 5
                        r = brightness
                        g = p
                        b = q
                End Select
            End If
            Return New Color(alpha, r, g, b)
        End Function

        Public ReadOnly Property Hue As Single
            Get
                If Me.Red = Me.Green And Me.Green = Me.Blue Then Return 0
                Dim retval As Single
                Dim min = {Me.Red, Me.Green, Me.Blue}.Min
                Dim max = {Me.Red, Me.Green, Me.Blue}.Max
                Dim delta = max - min
                If _Almost(Me.Red, max) Then
                    retval = (Me.Green - Me.Blue) / delta
                ElseIf _Almost(Me.Green, max) Then
                    retval = 2 + (Me.Blue - Me.Red) / delta
                Else
                    retval = 4 + (Me.Red - Me.Green) / delta
                End If
                retval *= 60
                If retval < 0 Then retval += 360
                Return retval * 182.04F
            End Get
        End Property

        Public ReadOnly Property Saturation As Single
            Get
                Dim min = {Me.Red, Me.Green, Me.Blue}.Min
                Dim max = {Me.Red, Me.Green, Me.Blue}.Max
                If _Almost(max, min) Then Return 0
                Return (If(_Almost(max, 0F), 0F, 1.0F - (1.0F * min / max)))
            End Get
        End Property

        Public ReadOnly Property Brightness As Single
            Get
                Return {Me.Red, Me.Green, Me.Blue}.Max
            End Get
        End Property

        Public Shared Function FromAHSL(alpha As Single, hue As Single, saturation As Single, lightness As Single) As Color
            Dim r As Double = 0, g As Double = 0, b As Double = 0
            If lightness <> 0 Then
                If saturation = 0 Then
                    r = lightness
                    g = lightness
                    b = lightness
                Else
                    Dim yy As Double
                    If lightness < 0.5 Then
                        yy = lightness * (1.0 + saturation)
                    Else
                        yy = lightness + saturation - (lightness * saturation)
                    End If
                    Dim xx As Double = 2.0 * lightness - yy
                    r = _GetColorComponent(xx, yy, hue + 1.0 / 3.0)
                    g = _GetColorComponent(xx, yy, hue)
                    b = _GetColorComponent(xx, yy, hue - 1.0 / 3.0)
                End If
            End If
            Return New Color(alpha, r, g, b)
        End Function

        Private Shared Function _GetColorComponent(ByVal xx As Double, ByVal yy As Double, ByVal zz As Double) As Double
            If zz < 0.0 Then
                zz += 1.0
            ElseIf zz > 1.0 Then
                zz -= 1.0
            End If
            If zz < 1.0 / 6.0 Then
                Return xx + (yy - xx) * 6.0 * zz
            ElseIf zz < 0.5 Then
                Return yy
            ElseIf zz < 2.0 / 3.0 Then
                Return xx + ((yy - xx) * ((2.0 / 3.0) - zz) * 6.0)
            Else
                Return xx
            End If
        End Function


        Public ReadOnly Property A As Byte
            Get
                Return Me.Alpha * 255
            End Get
        End Property

        Public ReadOnly Property R As Byte
            Get
                Return Me.Red * 255
            End Get
        End Property

        Public ReadOnly Property G As Byte
            Get
                Return Me.Green * 255
            End Get
        End Property

        Public ReadOnly Property B As Byte
            Get
                Return Me.Blue * 255
            End Get
        End Property

        Private Shared Function _Almost(x As Single, y As Single, Optional precision As Single = Single.Epsilon) As Boolean
            Return Math.Abs(x - y) <= precision
        End Function

        Public ReadOnly Property Luminance As Single
            Get
                Return _clamp(Me.Red * 0.299 + Me.Green * 0.587 + Me.Blue * 0.114)
            End Get
        End Property

        Public Function GrayScaled() As Color
            Dim gray As Single = Me.Luminance
            Return New Color(Me.Alpha, gray, gray, gray)
        End Function

        Public Shared Function FromWavelength(alpha As Single, waveLength As Double) As Color
            Dim attenuation As Double = 1.0
            Dim r, g, b As Double
            Select Case waveLength
                Case 380 To 440
                    attenuation = 0.3 + 0.7 * (waveLength - 380) / (440 - 380)
                    r = ((-(waveLength - 440) / (440 - 380)) * attenuation)
                    g = 0.0
                    b = (1.0 * attenuation)
                Case 440 To 490
                    r = 0.0
                    g = ((waveLength - 440) / (490 - 440))
                    b = 1.0
                Case 490 To 510
                    r = 0.0
                    g = 1.0
                    b = (-(waveLength - 510) / (510 - 490))
                Case 510 To 580
                    r = ((waveLength - 510) / (580 - 510))
                    g = 1.0
                    b = 0.0
                Case 580 To 645
                    r = 1.0
                    g = (-(waveLength - 645) / (645 - 580))
                    b = 0.0
                Case 645 To 750
                    attenuation = 0.3 + 0.7 * (750 - waveLength) / (750 - 645)
                    r = (1.0 * attenuation)
                    g = 0.0
                    b = 0.0
                Case Else
                    r = 0.0
                    g = 0.0
                    b = 0.0
            End Select
            Return New Color(alpha, r, g, b)
        End Function

        Public Shared Function FromTemperature(alpha As Single, temperature As Double, Optional maxValue As Double = Double.NaN) As Color
            Dim red As Double() = {0.64, 0.33, 1 - 0.64 - 0.33}
            Dim green As Double() = {0.29, 0.6, 1 - 0.29 - 0.6}
            Dim blue As Double() = {0.15, 0.06, 1 - 0.15 - 0.06}
            Dim rgb As Double() = _RGBFromCIE(red, green, blue, _BlackBodyToCIE(temperature, maxValue))
            Dim max As Double = Math.Max(0.0000000001, rgb.Max)
            Return New Color(alpha, rgb(0) / max, rgb(1) / max, rgb(2) / max)
        End Function

        Private Shared Function _RGBFromCIE(x As Double(), y As Double(), z As Double(), color As Double()) As Double()
            Dim denominator As Double = (x(0) * y(1) - x(1) * y(0)) * z(2) + (x(2) * y(0) - x(0) * y(2)) * z(1) + (x(1) * y(2) - x(2) * y(1)) * z(0)
            Dim retval As Double() = {0, 0, 0}
            retval(0) = ((color(0) * y(1) - x(1) * color(1)) * z(2) + (x(1) * y(2) - x(2) * y(1)) * color(2) + (x(2) * color(1) - color(0) * y(2)) * z(1)) / denominator
            retval(1) = ((x(0) * color(1) - color(0) * y(0)) * z(2) + (x(2) * y(0) - x(0) * y(2)) * color(2) + (color(0) * y(2) - x(2) * color(1)) * z(0)) / denominator
            retval(2) = ((x(0) * y(1) - x(1) * y(0)) * color(2) + (color(0) * y(0) - x(0) * color(1)) * z(1) + (x(1) * color(1) - color(0) * y(1)) * z(0)) / denominator
            For i = 0 To 2
                retval(i) = Math.Max(Math.Min(retval(i), 1), 0)
            Next
            Return retval
        End Function

        Private Shared Function _BlackBodyToCIE(temperature As Double, Optional maxValue As Double = Double.NaN) As Double()
            Dim fColorMatch As Double(,) = {
                {0.0014, 0.0000, 0.0065},
                {0.0022, 0.0001, 0.0105},
                {0.0042, 0.0001, 0.0201},
                {0.0076, 0.0002, 0.0362},
                {0.0143, 0.0004, 0.0679},
                {0.0232, 0.0006, 0.1102},
                {0.0435, 0.0012, 0.2074},
                {0.0776, 0.0022, 0.3713},
                {0.1344, 0.004, 0.6456},
                {0.2148, 0.0073, 1.0391},
                {0.2839, 0.0116, 1.3856},
                {0.3285, 0.0168, 1.623},
                {0.3483, 0.023, 1.7471},
                {0.3481, 0.0298, 1.7826},
                {0.3362, 0.038, 1.7721},
                {0.3187, 0.048, 1.7441},
                {0.2908, 0.06, 1.6692},
                {0.2511, 0.0739, 1.5281},
                {0.1954, 0.091, 1.2876},
                {0.1421, 0.1126, 1.0419},
                {0.0956, 0.139, 0.813},
                {0.058, 0.1693, 0.6162},
                {0.032, 0.208, 0.4652},
                {0.0147, 0.2586, 0.3533},
                {0.0049, 0.323, 0.272},
                {0.0024, 0.4073, 0.2123},
                {0.0093, 0.503, 0.1582},
                {0.0291, 0.6082, 0.1117},
                {0.0633, 0.71, 0.0782},
                {0.1096, 0.7932, 0.0573},
                {0.1655, 0.862, 0.0422},
                {0.2257, 0.9149, 0.0298},
                {0.2904, 0.954, 0.0203},
                {0.3597, 0.9803, 0.0134},
                {0.4334, 0.995, 0.0087},
                {0.5121, 1.0, 0.0057},
                {0.5945, 0.995, 0.0039},
                {0.6784, 0.9786, 0.0027},
                {0.7621, 0.952, 0.0021},
                {0.8425, 0.9154, 0.0018},
                {0.9163, 0.87, 0.0017},
                {0.9786, 0.8163, 0.0014},
                {1.0263, 0.757, 0.0011},
                {1.0567, 0.6949, 0.001},
                {1.0622, 0.631, 0.0008},
                {1.0456, 0.5668, 0.0006},
                {1.0026, 0.503, 0.0003},
                {0.9384, 0.4412, 0.0002},
                {0.8544, 0.381, 0.0002},
                {0.7514, 0.321, 0.0001},
                {0.6424, 0.265, 0.0000},
                {0.5419, 0.217, 0.0000},
                {0.4479, 0.175, 0.0000},
                {0.3608, 0.1382, 0.0000},
                {0.2835, 0.107, 0.0000},
                {0.2187, 0.0816, 0.0000},
                {0.1649, 0.061, 0.0000},
                {0.1212, 0.0446, 0.0000},
                {0.0874, 0.032, 0.0000},
                {0.0636, 0.0232, 0.0000},
                {0.0468, 0.017, 0.0000},
                {0.0329, 0.0119, 0.0000},
                {0.0227, 0.0082, 0.0000},
                {0.0158, 0.0057, 0.0000},
                {0.0114, 0.0041, 0.0000},
                {0.0081, 0.0029, 0.0000},
                {0.0058, 0.0021, 0.0000},
                {0.0041, 0.0015, 0.0000},
                {0.0029, 0.001, 0.0000},
                {0.002, 0.0007, 0.0000},
                {0.0014, 0.0005, 0.0000},
                {0.001, 0.0004, 0.0000},
                {0.0007, 0.0002, 0.0000},
                {0.0005, 0.0002, 0.0000},
                {0.0003, 0.0001, 0.0000},
                {0.0002, 0.0001, 0.0000},
                {0.0002, 0.0001, 0.0000},
                {0.0001, 0.0000, 0.0000},
                {0.0001, 0.0000, 0.0000},
                {0.0001, 0.0000, 0.0000},
                {0.0000, 0.0000, 0.0000}
            }
            Dim xx As Double = 0
            Dim yy As Double = 0
            Dim zz As Double = 0
            Dim dis As Double
            Dim wavelength As Double
            Dim weight As Double
            Dim nbands As Integer = 81
            For band = 0 To nbands - 1
                If (band = 0 Or band = nbands - 1) Then weight = 0.5
                wavelength = (380 + band * 5) * 0.000000001
                dis = 0.00000000000000037417717900752592 / wavelength ^ 5 / (Math.Exp(0.014387773538277204 / (wavelength * temperature)) - 1)
                xx = xx + dis * fColorMatch(band, 0)
                yy = yy + dis * fColorMatch(band, 1)
                zz = zz + dis * fColorMatch(band, 2)
            Next
            Dim max As Double = If(Double.IsNaN(maxValue), Math.Max(xx, Math.Max(yy, zz)), maxValue)
            Return {xx / max, yy / max, zz / max}
        End Function

        Public Shared Operator ^(color As Color, gamma As Double) As Color
            Return New Color(color.Alpha, color.Red ^ gamma, color.Green ^ gamma, color.Blue ^ gamma)
        End Operator

        Public Shared Function Rotate(color As Color, phi As Single) As Color
            Return color.Rotated(phi)
        End Function

        Public Function Rotated(phi As Single) As Color
            Dim hue As Single = _clamp_angle(Me.Hue + phi)
            Return FromAHSB(Me.Alpha, hue, Me.Saturation, Me.Brightness)
        End Function

        Public Shared Function Brighten(color As Color, delta As Single) As Color
            Return color.Brightened(delta)
        End Function

        Public Function Brightened(delta As Single) As Color
            Dim brightness As Single = _clamp(Me.Brightness + delta)
            Return FromAHSB(Me.Alpha, Me.Hue, Me.Saturation, brightness)
        End Function

        Public Shared Function Saturate(color As Color, delta As Single) As Color
            Return color.Saturated(delta)
        End Function

        Public Function Saturated(delta As Single) As Color
            Dim saturation As Single = _clamp(Me.Saturation + delta)
            Return FromAHSB(Me.Alpha, Me.Hue, saturation, Me.Brightness)
        End Function

    End Class

End Namespace
