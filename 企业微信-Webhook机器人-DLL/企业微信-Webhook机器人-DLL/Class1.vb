Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

Public Class WeChatMessageSender

    Public Event MessageSent(ByVal message As String)
    Public Event ErrorOccurred(ByVal errorMessage As String)

    Public Sub SendMessage(ByVal content As String)
        Try
            Dim url As String = ""  '此处替换成自己的 企业微信  webhook url
            Dim title As String = ""
            Dim jsonPayload As String = "{""msgtype"": ""text"", ""text"": {""title"": """ & title & """, ""content"": """ & content & """}}"
            Dim byteData As Byte() = Encoding.UTF8.GetBytes(jsonPayload)

            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.ContentLength = byteData.Length

            Using requestStream As Stream = request.GetRequestStream()
                requestStream.Write(byteData, 0, byteData.Length)
            End Using

            Dim response As HttpWebResponse = request.GetResponse()
            Dim responseStream As Stream = response.GetResponseStream()

            Dim reader As New StreamReader(responseStream)
            Dim result As String = reader.ReadToEnd()

            Dim errorCodeFilePath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "errorcode.txt")
            Dim errorCodeLines As String() = File.ReadAllLines(errorCodeFilePath)

            Dim errCodeMatch As Match = Regex.Match(result, """errcode"":(\d+)")
            Dim errCode As Integer = 0
            If errCodeMatch.Success Then
                Integer.TryParse(errCodeMatch.Groups(1).Value, errCode)
            End If

            Dim errMsg As String = "未知"
            Dim errSolution As String = "请参考相关文档或联系技术支持"
            For Each line As String In errorCodeLines
                Dim fields As String() = line.Split(vbTab)
                If fields.Length >= 3 AndAlso fields(0).Trim() = errCode.ToString() Then
                    errMsg = fields(1).Trim()
                    errSolution = fields(2).Trim()
                    Exit For
                End If
            Next

            RaiseEvent MessageSent("响应说明：" & errMsg & vbCrLf & "返回结果：" & result & vbCrLf & "具体说明：" & errSolution)

        Catch ex As Exception
            RaiseEvent ErrorOccurred("发送失败！" & vbCrLf & vbCrLf & "错误信息：" & ex.Message)
        End Try
    End Sub

End Class
