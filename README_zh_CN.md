# 基于企业微信webhook制作的dll，方便在程序中直接调用

## 初步设置：
1. 项目拉取后使用vs2010及以上版本打开项目`企业微信-Webhook机器人-DLL.sln`后修改`url`的值，将自己的`webhookurl`填入 参考[https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx]
1. 编译后生成`workweixinwebhook.dll`作为备用
   
## 使用示例：
1. 将`workweixinwebhook.dll`引用到项目中
1. Imports `workweixinwebhook`
1. 定义变量`Dim messageSender As New WeChatMessageSender()`
1. 调用 `messageSender.SendMessage("此处填写需调用、发送的内容")`

## 调用示例(Form)：
```vbnet
Imports workweixinwebhook

Public Class Form1
    Dim messageSender As New WeChatMessageSender()

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim content As String = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]")
        messageSender.SendMessage(content)
    End Sub

    Private Sub OnMessageSent(ByVal message As String)
        ' 在消息发送成功时处理逻辑
        MessageBox.Show(message, "消息发送成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub OnErrorOccurred(ByVal errorMessage As String)
        ' 在发送消息出现错误时处理逻辑
        MessageBox.Show(errorMessage, "发送消息出错", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

 Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' 在窗体加载时订阅事件
        AddHandler messageSender.MessageSent, AddressOf OnMessageSent
        AddHandler messageSender.ErrorOccurred, AddressOf OnErrorOccurred

    End Sub

    Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' 在窗体关闭时取消订阅事件
        RemoveHandler messageSender.MessageSent, AddressOf OnMessageSent
        RemoveHandler messageSender.ErrorOccurred, AddressOf OnErrorOccurred
    End Sub
```

## 调用参考：
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-21_08.49.30.png)
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-19_07.46.51.png)


## 企业微信接收参考：
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-21_08.55.11.png)

## License
MIT