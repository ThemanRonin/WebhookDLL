Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

' 定义一个发送微信消息的类
Public Class WeChatMessageSender

    ' 定义消息发送成功事件
    Public Event MessageSent(ByVal message As String)
    ' 定义错误发生事件
    Public Event ErrorOccurred(ByVal errorMessage As String)

    ' 定义发送消息的方法
    Public Sub SendMessage(ByVal content As String)
        Try
            ' 定义 webhook URL，用于发送消息
            Dim url As String = ""  ' 此处替换成自己的企业微信 webhook URL
            Dim title As String = "" ' 可以根据需求定义消息的标题

            ' 构建 JSON 格式的消息内容
            ' 注意：在此处对内容进行适当的转义和处理
            Dim jsonContent As String = """" & content.Replace("""", "\""") & """"
            Dim jsonMessage As String = "{""msgtype"": ""text"", ""text"": {""content"": " & jsonContent & "}}"
            Dim byteData As Byte() = Encoding.UTF8.GetBytes(jsonMessage)

            ' 创建 HTTP 请求对象
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
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

            ' 获取 HTTP 响应对象
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                ' 检查响应状态码
                If response.StatusCode = HttpStatusCode.OK Then
                    ' 获取响应流
                    Using responseStream As Stream = response.GetResponseStream()
                        ' 读取响应内容
                        Using reader As New StreamReader(responseStream)
                            Dim result As String = reader.ReadToEnd()
                    
                            ' 读取错误码映射文件
                            Dim errorCodeFilePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorcode.txt")
                            Dim errorCodeContent As String = File.ReadAllText(errorCodeFilePath)
                    
                            ' 从响应结果中提取错误码
                            Dim errCodeMatch As Match = Regex.Match(result, """errcode"":(\d+)")
                            Dim errCode As Integer = 0
                            If errCodeMatch.Success Then
                                Integer.TryParse(errCodeMatch.Groups(1).Value, errCode)
                            End If
                    
                            ' 根据错误码获取错误信息和解决方案
                            Dim errMsg As String = "未知错误"
                            Dim errSolution As String = "请参考相关文档或联系技术支持"
                            Dim errCodePattern As String = $"错误码：{errCode}\s+([\s\S]+?)(?=错误码：|\Z)"
                    
                            Dim errCodeMatchDetails As Match = Regex.Match(errorCodeContent, errCodePattern, RegexOptions.Multiline)
                    
                            If errCodeMatchDetails.Success Then
                                Dim details As String() = errCodeMatchDetails.Groups(1).Value.Split(New String() {vbCrLf}, StringSplitOptions.None)
                                errMsg = details(0).Trim()
                                errSolution = String.Join(vbCrLf, details.Skip(1)).Trim()
                            End If
                    
                            ' 触发 MessageSent 事件,返回响应说明、返回结果和具体说明
                            RaiseEvent MessageSent("响应说明：" & errMsg & vbCrLf & "返回结果：" & result & vbCrLf & "具体说明：" & errSolution)
                        End Using
                    End Using

                Else
                    ' 处理非 200 OK 的响应
                    RaiseEvent ErrorOccurred("HTTP 响应错误，状态码：" & response.StatusCode)
                End If
            End Using
        Catch webEx As WebException
            ' 处理 WebException 异常
            Dim webResponse As HttpWebResponse = CType(webEx.Response, HttpWebResponse)
            If webResponse IsNot Nothing Then
                Using responseStream As Stream = webResponse.GetResponseStream()
                    Using reader As New StreamReader(responseStream)
                        Dim errorResponse As String = reader.ReadToEnd()
                        RaiseEvent ErrorOccurred("WebException 发生：" & webEx.Message & vbCrLf & "响应内容：" & errorResponse)
                    End Using
                End Using
            Else
                RaiseEvent ErrorOccurred("WebException 发生：" & webEx.Message)
            End If
        Catch ioEx As IOException
            ' 处理 IO 异常
            RaiseEvent ErrorOccurred("IOException 发生：" & ioEx.Message)
        Catch ex As Exception
            ' 处理所有其他异常
            RaiseEvent ErrorOccurred("发送失败！" & vbCrLf & "错误信息：" & ex.Message)
        End Try
    End Sub

End Class
