Imports Myrh.Entities
Imports SimpleTexUnicode

Namespace Physics
    Public Class System

        Public Shared Sub FromFile(fileName As String)
            _FromLines(_LineWise(fileName))
        End Sub

        Private Shared Iterator Function _LineWise(fileName As String) As IEnumerable(Of String)
            Using f As New IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using r As New IO.StreamReader(f, Text.Encoding.UTF8, False, 4096, True)
                    While Not r.EndOfStream
                        Yield r.ReadLine
                    End While
                End Using
            End Using
        End Function

        Public Shared Sub FromDescription(text As String)
            _FromLines(text.Split({vbNewLine}, StringSplitOptions.RemoveEmptyEntries))
        End Sub

        Public Shared Sub FromName(name As String)
            Select Case name
                Case "metric"
                    FromDescription(My.Resources.metric)
                Case Else
                    Throw New Exception($"Resource {name} not found.")
            End Select
        End Sub

        Private Shared Sub _FromLines(lines As IEnumerable(Of String))
            Dim current_name As String = ""
            For Each line As String In lines
                Dim command As String = If(line.Contains(" "c), line.Substring(0, line.IndexOf(" "c)).Trim.ToLower, line)
                line = line.Substring(command.Length)
                Dim parts As String() = line.Split(",").Select(Function(p) p.Trim).ToArray
                Select Case command
                    Case "r", "reset"
                        Domain.Current.Reset()
                    Case "n", "name"
                        current_name = parts.First
                    Case "d", "default"
                        Domain.Current.SetDefaultSystem(current_name)
                    Case "u", "unit"
                        Unit.Define(
                            SimpleTex.LatexToUnicode(parts(0)),
                            SimpleTex.LatexToUnicode(parts(1)),
                            If(parts(2) = "", Prefix.One, Prefix.Parse(SimpleTex.LatexToUnicode(parts(2)))),
                            current_name,
                            Dimension.Parse(SimpleTex.LatexToUnicode(parts(3)))
                        )
                    Case "q", "quantity"
                        Quantity.Define(
                            SimpleTex.LatexToUnicode(parts(0)),
                            SimpleTex.LatexToUnicode(parts(1)),
                            If(parts(2) = "", Nothing, Unit.Parse(SimpleTex.LatexToUnicode(parts(2)))),
                            If(parts(3) = "", Nothing, Quantity.Parse(SimpleTex.LatexToUnicode(parts(3))))
                        )
                    Case "l", "link"
                        Link.Define(
                            Unit.Parse(SimpleTex.LatexToUnicode(parts(0))).AsAtom,
                            Unit.Parse(SimpleTex.LatexToUnicode(parts(1))).AsAtom,
                            Values.Formatting.FromScientific(parts(2))
                        )
                    Case "#"
                    Case Else
                        Throw New Exception($"Unknown command in description: {command}.")
                End Select
            Next
        End Sub

        'Dynamic viscosity 	pascal second 	Pa·s
        'moment of force 	newton meter 	N·m
        'surface tension 	newton per meter 	N/m
        'angular velocity 	radian per second 	rad/s
        'angular acceleration 	radian per second squared 	rad/s2
        'heat flux density, irradiance 	watt per square meter 	W/m2
        'heat capacity, entropy 	joule per kelvin 	J/K
        'specific heat capacity, specific entropy 	joule per kilogram kelvin 	J/(kg·K)
        'specific energy 	joule per kilogram 	J/kg
        'thermal conductivity 	watt per meter kelvin 	W/(m·K)
        'energy density 	joule per cubic meter 	J/m3
        'electric field strength 	volt per meter 	V/m
        'electric charge density 	coulomb per cubic meter 	C/m3
        'electric flux density 	coulomb per square meter 	C/m2
        'permittivity    farad per meter 	F/m
        'permeability    henry per meter 	H/m
        'molar energy 	joule per mole 	J/mol
        'molar entropy, molar heat capacity 	joule per mole kelvin 	J/(mol·K)
        'exposure(x And gamma rays) 	coulomb per kilogram 	C/kg
        'absorbed dose rate 	gray per second 	Gy/s
        'radiant intensity 	watt per steradian 	W/sr
        'radiance    watt per square meter steradian 	W/(m2·sr)
        'catalytic(activity) concentration 	katal per cubic meter 	kat/m3

        'http://kirste.userpage.fu-berlin.de/chemistry/general/si.html

    End Class

End Namespace
