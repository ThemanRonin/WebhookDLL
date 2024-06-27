# A DLL created based on the WeCom webhook for enterprises, making it convenient to directly invoke in programs.

## Initial Setup:        
1. After pulling the project, use VS2010 or above to open the project `企业微信-Webhook机器人-DLL.sln`. Then modify the value of `url` by filling in your own `webhookurl`. Refer to [https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx].
2. After compiling, generate `workweixinwebhook.dll` for backup.

## Usage Example:        
1. Reference `workweixinwebhook.dll` in your project.
2. Import `workweixinwebhook`.
3. Define a variable `Dim messageSender As New WeChatMessageSender()`.
4. Call `messageSender.SendMessage("Enter the content to be called and sent here")`.

## Call Example (Form):
```vbnet        
Imports workweixinwebhook

Public Class Form1
    Dim messageSender As New WeChatMessageSender()
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim content As String = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]")
        messageSender.SendMessage(content)
    End Sub
    Private Sub OnMessageSent(ByVal message As String)
        ' Logic to handle when the message is successfully sent
        MessageBox.Show(message, "Message Sent Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
    Private Sub OnErrorOccurred(ByVal errorMessage As String)
        ' Logic to handle when there is an error sending the message
        MessageBox.Show(errorMessage, "Error Sending Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub
    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Subscribe to events when the form loads
        AddHandler messageSender.MessageSent, AddressOf OnMessageSent
        AddHandler messageSender.ErrorOccurred, AddressOf OnErrorOccurred
    End Sub
    Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Unsubscribe from events when the form closes
        RemoveHandler messageSender.MessageSent, AddressOf OnMessageSent
        RemoveHandler messageSender.ErrorOccurred, AddressOf OnErrorOccurred
    End Sub
```

## Reference for Calling:
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-21_08.49.30.png)
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-19_07.46.51.png)

## Reference for WeChat Work Reception:
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-21_08.55.11.png)

## License        
MIT
