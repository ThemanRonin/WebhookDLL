Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

Public Class WeChatMessageSender

    Public Event MessageSent(ByVal message As String)
    Public Event ErrorOccurred(ByVal errorMessage As String)

    Public Sub SendMessage(ByVal content As String)
        Try
            Dim url As String = ""  '此处替换成自己的企业微信 webhook url
            Dim title As String = ""

            ' 构建 JSON 格式的消息内容
            Dim byteData As Byte() = Encoding.UTF8.GetBytes("{""msgtype"": ""text"", ""text"": {""title"": """ & title & """, ""content"": """ & content & """}}")

            ' 创建 HTTP 请求对象
            Dim request As HttpWebRequest = WebRequest.Create(url)
            ' 设置请求方法为 POST
            request.Method = "POST"
            ' 设置请求内容类型为 application/json
            request.ContentType = "application/json"
            ' 设置请求内容长度
            request.ContentLength = byteData.Length

            ' 将消息内容写入请求流
            Using requestStream As Stream = request.GetRequestStream()
                requestStream.Write(byteData, 0, byteData.Length)
            End Using
            ' 获取响应流
            Dim responseStream As Stream = request.GetResponse().GetResponseStream()
            ' 读取响应结果
            Dim reader As New StreamReader(responseStream)
            Dim result As String = reader.ReadToEnd()
            ' 读取错误码映射文件
            Dim errorCodeLines As String() = File.ReadAllLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "errorcode.txt"))
            ' 从响应结果中提取错误码
            Dim errCodeMatch As Match = Regex.Match(result, """errcode"":(\d+)")
            Dim errCode As Integer = 0
            If errCodeMatch.Success Then
                Integer.TryParse(errCodeMatch.Groups(1).Value, errCode)
            End If

            ' 根据错误码获取错误信息和解决方案
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

            ' 触发 MessageSent 事件,返回响应说明、返回结果和具体说明
            RaiseEvent MessageSent("响应说明：" & errMsg & vbCrLf & "返回结果：" & result & vbCrLf & "具体说明：" & errSolution)
        Catch ex As Exception
            ' 触发 ErrorOccurred 事件,返回错误信息
            RaiseEvent ErrorOccurred("发送失败！" & vbCrLf & vbCrLf & "错误信息：" & ex.Message)
        End Try
    End Sub


End Class
