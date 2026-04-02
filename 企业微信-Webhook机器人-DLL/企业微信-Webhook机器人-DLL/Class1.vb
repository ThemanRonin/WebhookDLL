Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' 企业微信 Webhook 消息发送器
''' </summary>
''' <remarks>
''' 该类提供了通过企业微信 Webhook 发送消息的功能，支持错误码解析和详细的错误信息反馈。
''' </remarks>
Public Class WeChatMessageSender

#Region "常量定义"

    Private Const DEFAULT_CONTENT_TYPE As String = "application/json"
    Private Const HTTP_POST_METHOD As String = "POST"
    Private Const ERROR_CODE_FILE_NAME As String = "errorcode.txt"
    Private Const UNKNOWN_ERROR_MESSAGE As String = "未知错误"
    Private Const DEFAULT_SOLUTION As String = "请参考相关文档或联系技术支持"

#End Region

#Region "私有字段"

    Private _webhookUrl As String = String.Empty
    Private _messageTitle As String = String.Empty
    Private _errorCodeFilePath As String = String.Empty

#End Region

#Region "公共属性"

    ''' <summary>
    ''' 获取或设置企业微信 Webhook URL
    ''' </summary>
    ''' <value>Webhook URL 地址</value>
    Public Property WebhookUrl As String
        Get
            Return _webhookUrl
        End Get
        Set(ByVal value As String)
            _webhookUrl = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置消息标题
    ''' </summary>
    ''' <value>消息标题文本</value>
    Public Property MessageTitle As String
        Get
            Return _messageTitle
        End Get
        Set(ByVal value As String)
            _messageTitle = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置错误码映射文件路径
    ''' </summary>
    ''' <value>错误码文件完整路径</value>
    Public Property ErrorCodeFilePath As String
        Get
            If String.IsNullOrEmpty(_errorCodeFilePath) Then
                _errorCodeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ERROR_CODE_FILE_NAME)
            End If
            Return _errorCodeFilePath
        End Get
        Set(ByVal value As String)
            _errorCodeFilePath = value
        End Set
    End Property

#End Region

#Region "事件定义"

    ''' <summary>
    ''' 消息发送成功事件
    ''' </summary>
    Public Event MessageSent(ByVal message As String)

    ''' <summary>
    ''' 错误发生事件
    ''' </summary>
    Public Event ErrorOccurred(ByVal errorMessage As String)

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 初始化 WeChatMessageSender 类的新实例
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 使用指定的 Webhook URL 初始化 WeChatMessageSender 类的新实例
    ''' </summary>
    ''' <param name="webhookUrl">企业微信 Webhook URL</param>
    Public Sub New(ByVal webhookUrl As String)
        _webhookUrl = webhookUrl
    End Sub

    ''' <summary>
    ''' 使用指定的 Webhook URL 和消息标题初始化 WeChatMessageSender 类的新实例
    ''' </summary>
    ''' <param name="webhookUrl">企业微信 Webhook URL</param>
    ''' <param name="messageTitle">消息标题</param>
    Public Sub New(ByVal webhookUrl As String, ByVal messageTitle As String)
        _webhookUrl = webhookUrl
        _messageTitle = messageTitle
    End Sub

#End Region

#Region "公共方法"

    ''' <summary>
    ''' 发送文本消息到企业微信
    ''' </summary>
    ''' <param name="content">要发送的消息内容</param>
    Public Sub SendMessage(ByVal content As String)
        Try
            ValidateConfiguration()

            Dim jsonMessage As String = BuildJsonMessage(content)
            Dim requestData As Byte() = Encoding.UTF8.GetBytes(jsonMessage)
            Dim request As HttpWebRequest = CreateHttpRequest(requestData.Length)

            WriteRequestData(request, requestData)
            ProcessResponse(request)

        Catch webEx As WebException
            HandleWebException(webEx)
        Catch ioEx As IOException
            HandleIOException(ioEx)
        Catch ex As Exception
            HandleGeneralException(ex)
        End Try
    End Sub

#End Region

#Region "私有方法 - 配置验证"

    ''' <summary>
    ''' 验证配置是否有效
    ''' </summary>
    Private Sub ValidateConfiguration()
        If String.IsNullOrEmpty(_webhookUrl) Then
            Throw New InvalidOperationException("Webhook URL 未配置，请先设置 WebhookUrl 属性")
        End If
    End Sub

#End Region

#Region "私有方法 - JSON 构建"

    ''' <summary>
    ''' 构建 JSON 格式的消息
    ''' </summary>
    ''' <param name="content">消息内容</param>
    ''' <returns>JSON 格式的消息字符串</returns>
    Private Function BuildJsonMessage(ByVal content As String) As String
        Dim escapedContent As String = EscapeJsonString(content)
        Dim jsonBuilder As New StringBuilder()

        jsonBuilder.Append("{")
        jsonBuilder.Append("""msgtype"": ""text"", ")
        jsonBuilder.Append("""text"": {")
        jsonBuilder.Append("""content"": """).Append(escapedContent).Append("""")
        jsonBuilder.Append("}")
        jsonBuilder.Append("}")

        Return jsonBuilder.ToString()
    End Function

    ''' <summary>
    ''' 转义 JSON 字符串中的特殊字符
    ''' </summary>
    ''' <param name="input">原始字符串</param>
    ''' <returns>转义后的字符串</returns>
    Private Function EscapeJsonString(ByVal input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        Dim result As String = input
        result = result.Replace("\", "\\")
        result = result.Replace("""", "\""")
        result = result.Replace(vbCr, "\r")
        result = result.Replace(vbLf, "\n")
        result = result.Replace(vbTab, "\t")

        Return result
    End Function

#End Region

#Region "私有方法 - HTTP 请求处理"

    ''' <summary>
    ''' 创建 HTTP 请求对象
    ''' </summary>
    ''' <param name="contentLength">请求内容长度</param>
    ''' <returns>配置好的 HttpWebRequest 对象</returns>
    Private Function CreateHttpRequest(ByVal contentLength As Integer) As HttpWebRequest
        Dim request As HttpWebRequest = CType(WebRequest.Create(_webhookUrl), HttpWebRequest)

        request.Method = HTTP_POST_METHOD
        request.ContentType = DEFAULT_CONTENT_TYPE
        request.ContentLength = contentLength

        Return request
    End Function

    ''' <summary>
    ''' 将数据写入请求流
    ''' </summary>
    ''' <param name="request">HTTP 请求对象</param>
    ''' <param name="data">要写入的数据</param>
    Private Sub WriteRequestData(ByVal request As HttpWebRequest, ByVal data As Byte())
        Using requestStream As Stream = request.GetRequestStream()
            requestStream.Write(data, 0, data.Length)
        End Using
    End Sub

#End Region

#Region "私有方法 - 响应处理"

    ''' <summary>
    ''' 处理 HTTP 响应
    ''' </summary>
    ''' <param name="request">HTTP 请求对象</param>
    Private Sub ProcessResponse(ByVal request As HttpWebRequest)
        Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            If response.StatusCode = HttpStatusCode.OK Then
                Dim responseContent As String = ReadResponseContent(response)
                HandleSuccessResponse(responseContent)
            Else
                Dim errorMsg As String = String.Format("HTTP 响应错误，状态码：{0} ({1})", CInt(response.StatusCode), response.StatusDescription)
                RaiseEvent ErrorOccurred(errorMsg)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 读取响应内容
    ''' </summary>
    ''' <param name="response">HTTP 响应对象</param>
    ''' <returns>响应内容字符串</returns>
    Private Function ReadResponseContent(ByVal response As HttpWebResponse) As String
        Using responseStream As Stream = response.GetResponseStream()
            Using reader As New StreamReader(responseStream, Encoding.UTF8)
                Return reader.ReadToEnd()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 处理成功的响应
    ''' </summary>
    ''' <param name="responseContent">响应内容</param>
    Private Sub HandleSuccessResponse(ByVal responseContent As String)
        Dim errorCode As Integer = ExtractErrorCode(responseContent)
        Dim errorDetails As ErrorDetail = GetErrorDetails(errorCode)

        Dim resultMessage As New StringBuilder()
        resultMessage.AppendLine(String.Format("响应说明：{0}", errorDetails.Message))
        resultMessage.AppendLine(String.Format("返回结果：{0}", responseContent))
        resultMessage.AppendLine(String.Format("具体说明：{0}", errorDetails.Solution))

        RaiseEvent MessageSent(resultMessage.ToString())
    End Sub

#End Region

#Region "私有方法 - 错误码处理"

    ''' <summary>
    ''' 从响应中提取错误码
    ''' </summary>
    ''' <param name="responseContent">响应内容</param>
    ''' <returns>错误码</returns>
    Private Function ExtractErrorCode(ByVal responseContent As String) As Integer
        Dim pattern As String = """errcode"":\s*(\d+)"
        Dim match As Match = Regex.Match(responseContent, pattern)

        If match.Success Then
            Dim errorCode As Integer = 0
            If Integer.TryParse(match.Groups(1).Value, errorCode) Then
                Return errorCode
            End If
        End If

        Return -1
    End Function

    ''' <summary>
    ''' 加载错误码映射文件内容
    ''' </summary>
    ''' <returns>错误码映射文件内容</returns>
    Private Function LoadErrorCodeMapping() As String
        Try
            If File.Exists(ErrorCodeFilePath) Then
                Return File.ReadAllText(ErrorCodeFilePath, Encoding.UTF8)
            End If
        Catch ex As Exception
        End Try

        Return String.Empty
    End Function

    ''' <summary>
    ''' 根据错误码获取错误详情
    ''' </summary>
    ''' <param name="errorCode">错误码</param>
    ''' <returns>错误详情对象</returns>
    Private Function GetErrorDetails(ByVal errorCode As Integer) As ErrorDetail
        Dim errorCodeContent As String = LoadErrorCodeMapping()

        If String.IsNullOrEmpty(errorCodeContent) Then
            Return New ErrorDetail(UNKNOWN_ERROR_MESSAGE, DEFAULT_SOLUTION)
        End If

        Dim pattern As String = String.Format("错误码：{0}\s+([\s\S]+?)(?=错误码：|\Z)", errorCode)
        Dim match As Match = Regex.Match(errorCodeContent, pattern, RegexOptions.Multiline)

        If match.Success Then
            Dim details As String() = match.Groups(1).Value.Split(New String() {vbCrLf, vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            If details.Length > 0 Then
                Dim message As String = details(0).Trim()
                Dim solution As String = DEFAULT_SOLUTION

                If details.Length > 1 Then
                    solution = String.Join(Environment.NewLine, details.Skip(1)).Trim()
                End If

                Return New ErrorDetail(message, solution)
            End If
        End If

        Return New ErrorDetail(UNKNOWN_ERROR_MESSAGE, DEFAULT_SOLUTION)
    End Function

#End Region

#Region "私有方法 - 异常处理"

    ''' <summary>
    ''' 处理 WebException 异常
    ''' </summary>
    ''' <param name="webEx">WebException 对象</param>
    Private Sub HandleWebException(ByVal webEx As WebException)
        Dim errorMessage As New StringBuilder()
        errorMessage.AppendLine(String.Format("网络请求异常：{0}", webEx.Message))

        If webEx.Response IsNot Nothing Then
            Try
                Using response As HttpWebResponse = CType(webEx.Response, HttpWebResponse)
                    Dim responseContent As String = ReadResponseContent(response)
                    errorMessage.AppendLine(String.Format("状态码：{0}", CInt(response.StatusCode)))
                    errorMessage.AppendLine(String.Format("响应内容：{0}", responseContent))
                End Using
            Catch readEx As Exception
                errorMessage.AppendLine(String.Format("读取响应失败：{0}", readEx.Message))
            End Try
        End If

        If webEx.InnerException IsNot Nothing Then
            errorMessage.AppendLine(String.Format("内部异常：{0}", webEx.InnerException.Message))
        End If

        RaiseEvent ErrorOccurred(errorMessage.ToString())
    End Sub

    ''' <summary>
    ''' 处理 IOException 异常
    ''' </summary>
    ''' <param name="ioEx">IOException 对象</param>
    Private Sub HandleIOException(ByVal ioEx As IOException)
        Dim errorMessage As New StringBuilder()
        errorMessage.AppendLine(String.Format("IO 操作异常：{0}", ioEx.Message))

        If ioEx.InnerException IsNot Nothing Then
            errorMessage.AppendLine(String.Format("内部异常：{0}", ioEx.InnerException.Message))
        End If

        RaiseEvent ErrorOccurred(errorMessage.ToString())
    End Sub

    ''' <summary>
    ''' 处理一般异常
    ''' </summary>
    ''' <param name="ex">Exception 对象</param>
    Private Sub HandleGeneralException(ByVal ex As Exception)
        Dim errorMessage As New StringBuilder()
        errorMessage.AppendLine("发送失败！")
        errorMessage.AppendLine(String.Format("异常类型：{0}", ex.GetType().Name))
        errorMessage.AppendLine(String.Format("错误信息：{0}", ex.Message))

        If ex.InnerException IsNot Nothing Then
            errorMessage.AppendLine(String.Format("内部异常：{0}", ex.InnerException.Message))
        End If

        RaiseEvent ErrorOccurred(errorMessage.ToString())
    End Sub

#End Region

#Region "内部类"

    ''' <summary>
    ''' 错误详情类
    ''' </summary>
    Private Class ErrorDetail
        Private _message As String
        Private _solution As String

        Public ReadOnly Property Message As String
            Get
                Return _message
            End Get
        End Property

        Public ReadOnly Property Solution As String
            Get
                Return _solution
            End Get
        End Property

        Public Sub New(ByVal message As String, ByVal solution As String)
            _message = message
            _solution = solution
        End Sub
    End Class

#End Region

End Class
