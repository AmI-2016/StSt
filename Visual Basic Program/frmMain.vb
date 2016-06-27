'Option Strict On
Option Infer On
Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Math
Public Class mainForm
    Dim tabChar As String = Char.ConvertFromUtf32(9)
    Dim portdata As String
    Dim vArray, vArray2 As Array
    Dim time, initial_time, final_time As String
    Dim Ready As Integer = 0
    Dim Lux As New Series
    Dim risposta As String 'Risposta nel campo "inserisci nome"
    Dim avrgValue, avrgValueLux As Double
    Dim i As Integer = 0 'iteration variable in avrg detect
    Dim NameArray As Array
    Dim globalPos, xpos, ypos, zpos As Double
    Dim x2pos, y2pos, z2pos As Double
    Private readBuffer As String = String.Empty
    Private readBuffer2 As String = String.Empty
    Private Bytenumber, Bytenumber2 As Integer
    Private ByteToRead As Integer
    Private byteEnd(2), byteEnd2(2) As Char
    Private comOpen, comOpen2 As Boolean
    Dim FileName As String


#Region "form events"

    Private Sub Form1_FormClosed(ByVal sender As System.Object,
                                 ByVal e As System.Windows.Forms.FormClosedEventArgs) _
                                 Handles MyBase.FormClosed
        If comOpen Then SerialPort1.Close()
    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        initial_time = TimeOfDay
        If (MsgBox("Do you want to open the StSt Tutorial?", MsgBoxStyle.YesNo, "Tutorial") = MsgBoxResult.Yes) Then
            Process.Start("C:\Users\s203625\Desktop\Nuova cartella (2)\Misure.pdf")
        End If
        risposta = InputBox("Specify your FullName:", "Full Name request")
        NameArray = Split(risposta, " ")
        Chart4.Series.Clear()
        positionLabel.Visible = False
        Label23.Visible = False
        positionLabel.ForeColor = Color.Red
        Lux.Name = "Luce"
        Label23.ForeColor = Color.Red
        Lux.ChartType = SeriesChartType.Spline

        Chart4.Series.Add(Lux)

        picOpen.BackColor = Color.Red
        ' read avaiable COM Ports:
        Dim Portnames As String() = System.IO.Ports.SerialPort.GetPortNames
        If Portnames Is Nothing Then
            MsgBox("There are no Com Ports detected!")
            Me.Close()
        End If
        cboComPort.Items.AddRange(Portnames)
        ComboBox1.Items.AddRange(Portnames)
        cboBaudRate.Text = "9600"
    End Sub

    Private Sub Button_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnComClose.Click
        If comOpen Then
            ' clear input buffer
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
        End If
        comOpen = False
        picOpen.BackColor = Color.Red
        picDataReceived.BackColor = Color.Gray
        cboComPort.Enabled = True
        cboBaudRate.Enabled = True
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object,
                            ByVal e As System.EventArgs) Handles Timer1.Tick
        picDataReceived.BackColor = Color.Gray
        Timer1.Enabled = False
    End Sub


#End Region

#Region "ComPort read data"
    Private Sub SerialPort2_DataReceived(ByVal sender As System.Object,
                                         ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) _
                                         Handles SerialPort2.DataReceived
        If comOpen2 Then
            Try
                byteEnd2 = SerialPort2.NewLine.ToCharArray
                Bytenumber2 = SerialPort2.BytesToRead

                If Ready = 1 Then

                Else
                    readBuffer2 = SerialPort2.ReadLine()
                    vArray2 = Split(readBuffer2, ",")
                    Me.Invoke(New EventHandler(AddressOf DoUpdate))
                End If




            Catch ex As Exception
                MsgBox("read " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object,
                                         ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) _
                                         Handles SerialPort1.DataReceived
        If comOpen Then
            Try
                byteEnd = SerialPort1.NewLine.ToCharArray

                ' get number off bytes in buffer
                Bytenumber = SerialPort1.BytesToRead

                ' read one byte from buffer
                'ByteToRead = SerialPort1.ReadByte()

                ' read one char from buffer
                'CharToRead = SerialPort1.ReadChar()

                ' read until string "90"
                'readBuffer1 = SerialPort1.ReadTo("90")

                ' read entire string until .Newline 
                If Ready = 1 Then

                Else
                    readBuffer = SerialPort1.ReadLine()
                    vArray = Split(readBuffer, ",")
                    'Insert_Values(vArray(0), vArray(1), vArray(2), 0, 0, 0, vArray(3))
                    Me.Invoke(New EventHandler(AddressOf DoUpdate))
                End If
                'data to UI thread
                'My.Computer.FileSystem.WriteAllText("C:\TestFolder1\123.txt", Now, True)


            Catch ex As Exception
                MsgBox("read " & ex.Message)
            End Try
        End If
    End Sub

    Public Sub DoUpdate(ByVal sender As Object, ByVal e As System.EventArgs)
        i = i + 1
        tbRx.AppendText(vArray(0) & vbCrLf)
        TextBox1.AppendText(vArray(1) & vbCrLf)
        TextBox2.AppendText(vArray(2) & vbCrLf)
        TextBox3.AppendText(vArray2(0) & vbCrLf)
        TextBox4.AppendText(vArray2(1) & vbCrLf)
        TextBox5.AppendText(vArray2(2) & vbCrLf)
        time = TimeOfDay
        Lux.Points.AddXY(time, vArray(3))
        Label5.Text = vArray(3) & "Lux"
        xpos = Round((CDbl(vArray(0)) * 0.47) / 100, 2)
        ypos = Round((CDbl(vArray(1)) * -9.53) / 100, 2)
        zpos = Round((CDbl(vArray(2)) * 2.75) / 100, 2)
        x2pos = Round((CDbl(vArray2(0)) * -0.2) / 100, 2)
        y2pos = Round((CDbl(vArray2(1)) * -7.77) / 100, 2)
        z2pos = Round((CDbl(vArray2(2)) * 6.12) / 100, 2)
        globalPos = Round(((x2pos + y2pos + z2pos + zpos + ypos + xpos) * -8.16) / 100, 2)
        avrgValue = globalPos + avrgValue
        avrgValueLux = CDbl(vArray(3)) + avrgValueLux
        If (globalPos < 70) Then
            Label14.ForeColor = Color.Red

        Else
            Label14.ForeColor = Color.Green
        End If

        If (xpos < 70) Then
            Label17.ForeColor = Color.Red
            positionLabel.Visible = True
            Label23.Visible = True
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"
        Else
            Label17.ForeColor = Color.Green
        End If

        If (ypos < 70) Then
            positionLabel.Visible = True
            Label23.Visible = True
            Label19.ForeColor = Color.Red
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"

        Else
            Label19.ForeColor = Color.Green
        End If

        If (zpos < 70) Then
            positionLabel.Visible = True
            Label23.Visible = True
            Label20.ForeColor = Color.Red
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"
        Else
            Label20.ForeColor = Color.Green
        End If

        If (x2pos < 70) Then
            positionLabel.Visible = True
            Label23.Visible = True
            Label29.ForeColor = Color.Red
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"
        Else
            Label29.ForeColor = Color.Green
        End If
        If (y2pos < 70) Then
            positionLabel.Visible = True
            Label23.Visible = True
            Label27.ForeColor = Color.Red
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"
        Else
            Label27.ForeColor = Color.Green
        End If
        If (z2pos < 70) Then
            positionLabel.Visible = True
            Label23.Visible = True
            Label26.ForeColor = Color.Red
            positionLabel.Text = "Warning! You have assumed a wrong position!"
            Label23.Text = "You have to change your posture in order to reach 100%"
        Else
            Label26.ForeColor = Color.Green
        End If
        Label17.Text = CStr(xpos) + "%"
        Label19.Text = CStr(ypos) + "%"
        Label20.Text = CStr(zpos) + "%"
        Label14.Text = CStr(globalPos) + "%"

        Label29.Text = CStr(x2pos) + "%"
        Label27.Text = CStr(y2pos) + "%"
        Label26.Text = CStr(z2pos) + "%"


        picDataReceived.BackColor = Color.Green
        Timer1.Enabled = True
    End Sub

#End Region
#Region "Buttons"
    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        With SerialPort2
            .ParityReplace = &H3B
            .PortName = ComboBox1.Text
            .BaudRate = CInt(cboBaudRate.Text)
            .Parity = IO.Ports.Parity.None
            .DataBits = 8
            .StopBits = IO.Ports.StopBits.One
            .Handshake = IO.Ports.Handshake.None
            .RtsEnable = False
            .ReceivedBytesThreshold = 1
            .NewLine = vbCrLf         ' CR must be the last char in frame. This terminates the SerialPort.readLine
            .ReadTimeout = 10000
        End With

        Try
            SerialPort2.Open()
            comOpen2 = SerialPort2.IsOpen
        Catch ex As Exception
            comOpen2 = False
            MsgBox("Error Open: " & ex.Message)
            'picOpen.BackColor = Color.Red
        End Try
        If comOpen2 Then
            'Create_Table()
            'picOpen.BackColor = Color.Green
            ComboBox1.Enabled = False
            cboBaudRate.Enabled = False
        End If

        With SerialPort1

            .ParityReplace = &H3B                    ' replace ";" when parity error occurs
            .PortName = cboComPort.Text
            .BaudRate = CInt(cboBaudRate.Text)
            .Parity = IO.Ports.Parity.None
            .DataBits = 8
            .StopBits = IO.Ports.StopBits.One
            .Handshake = IO.Ports.Handshake.None
            .RtsEnable = False
            .ReceivedBytesThreshold = 1             'threshold: one byte in buffer > event is fired
            .NewLine = vbCrLf        ' CR must be the last char in frame. This terminates the SerialPort.readLine
            .ReadTimeout = 10000

        End With

        ' check whether device is avaiable:
        Try
            SerialPort1.Open()
            comOpen = SerialPort1.IsOpen
        Catch ex As Exception
            comOpen = False
            MsgBox("Error Open: " & ex.Message)
            picOpen.BackColor = Color.Red
        End Try

        If comOpen Then
            'Create_Table()
            picOpen.BackColor = Color.Green
            cboComPort.Enabled = False
            cboBaudRate.Enabled = False
        End If
        If comOpen Then SerialPort1.WriteLine("1")
        If comOpen2 Then SerialPort2.WriteLine("1")
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        Ready = 0
        final_time = TimeOfDay
        If comOpen2 Then
            SerialPort2.WriteLine("0")
            SerialPort2.DiscardInBuffer()
            SerialPort2.Close()
        End If
        comOpen2 = False
        If comOpen Then
            ' clear input buffer
            SerialPort1.WriteLine("0")
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
        End If
        comOpen = False
        picOpen.BackColor = Color.Red
        'Web_Put()
        Me.Close()
    End Sub

    Private Sub PictureBox1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Process.Start("http://ami-2016.github.io/StSt/")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Ready = 1
    End Sub
#End Region
#Region "WebRequest"
    Sub Web_Put()
        Dim dtmTest As Date
        dtmTest = DateValue(Now)
        ' Create a request using a URL that can receive a post. 
        Dim request As WebRequest = WebRequest.Create("http://192.168.0.22:5000/sessions")
        ' Set the Method property of the request to POST.
        request.Method = "POST"
        ' Create POST data and convert it to a byte array.
        Dim postData As String = "{" + vbCr + tabChar + """start"": """ + initial_time + """," + vbCr + tabChar + """stop"": """ + final_time + """," + vbCr + tabChar + """day"": """ + CStr(dtmTest) + """," + vbCr + tabChar + """position"": " + "2.0" + "," + vbCr + tabChar + """light"": " + "1.0" + "," + vbCr + tabChar + """name"": """ + CStr(risposta) + """" + vbCr + "}"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        ' Set the ContentType property of the WebRequest.
        request.ContentType = "application/json"
        ' Set the ContentLength property of the WebRequest.
        request.ContentLength = byteArray.Length
        ' Get the request stream.
        Dim dataStream As Stream = request.GetRequestStream()
        ' Write the data to the request stream.
        dataStream.Write(byteArray, 0, byteArray.Length)
        ' Close the Stream object.
        dataStream.Close()
        ' Get the response.
        Dim response As WebResponse = request.GetResponse()
        ' Display the status.
        If (CType(response, HttpWebResponse).StatusDescription = "OK") Then
            PictureBox4.BackColor = Color.Green
        End If
        ' Get the stream containing content returned by the server.
        dataStream = response.GetResponseStream()
        ' Open the stream using a StreamReader for easy access.
        Dim reader As New StreamReader(dataStream)
        ' Read the content.
        Dim responseFromServer As String = reader.ReadToEnd()
        ' Display the content.
        Console.WriteLine(responseFromServer)
        ' Clean up the streams.
        reader.Close()
        dataStream.Close()
        response.Close()
    End Sub

#End Region

End Class

