# 基于企业微信webhook制作的dll，方便在程序中直接调用

## 使用示例：
1. 先将`workweixinwebhook.dll`添加到项目中
1. 引入 `Imports workweixinwebhook`
2. 定义变量`Dim messageSender As New WeChatMessageSender()`
3. 调用 `messageSender.SendMessage("需调用的内容")`

## 调用示例：
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-19_07.46.51.png)
