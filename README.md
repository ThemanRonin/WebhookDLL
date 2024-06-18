# 基于企业微信webhook制作的dll，方便在程序中直接调用

## 初步设置
1. 项目拉取后使用vs2010及以上版本修改`url`的值，将自己的`webhookurl`填入
1. 编译后生成`workweixinwebhook.dll`作为备用
   
## 使用示例：
1. 将`workweixinwebhook.dll`引用到项目中
1. Imports `workweixinwebhook`
1. 定义变量`Dim messageSender As New WeChatMessageSender()`
1. 调用 `messageSender.SendMessage("此处填写需调用、发送的内容")`

## 调用示例：
![](https://cdn.jsdelivr.net/gh/ThemanRonin/JPG@main/2024-06-19_07.46.51.png)
