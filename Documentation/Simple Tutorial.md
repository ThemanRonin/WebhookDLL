# 简易教程

最后更新：2022/01/20

为让开发者快速理解开发流程，本篇章展示如何一步步设计一个能与企业后台互动的自建应用。
![](https://p.qpic.cn/pic_wework/3903686901/0107e00df75b83aac9934d61515996a1ddc7c93c146cc5b6/0)

### 添加自建应用

登录企业微信管理端 -> 应用与小程序 -> 应用 -> 自建，点击“创建应用”，设置应用logo、应用名称等信息，创建应用。
创建完成后，在管理端的应用列表里进入该应用，可以看到agentid、secret等信息，这些信息在使用企业微信API时会用到。
创建完成后，该应用会自动出现在可见范围内的成员的企业微信终端上（包括手机端、pc端、微信插件）

### 使用工具调试api

我们已经创建好一个自建应用，且拿到了可用的应用id及secret，如何调用api控制这个应用呢？下面以发消息为例说明如何调试api接口。

调用api的过程，本质上就是发送http请求给企业微信后台，在正式开发前，我们可以使用工具模拟http请求调试api。这里以 postman 为例（[下载地址](https://www.getpostman.com/apps)，使用方法略），当然你也可以使用其它http模拟工具。

发消息api见[发送应用消息](https://developer.work.weixin.qq.com/document/path/90487#10167)，可以看到其实就是一个post请求

> **请求方式：** POST（**HTTPS**）
> **请求地址：**  https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=ACCESS_TOKEN

post 参数为 access_token 和 消息体。

**1.获取access_token**

参考[开始开发](https://developer.work.weixin.qq.com/document/path/90487#14952)，access_token是应用调用api的凭证，由 [corpid](https://developer.work.weixin.qq.com/document/path/90487#14953/corpid)和[corpsecret](https://developer.work.weixin.qq.com/document/path/90487#14953/secret)换取。

> **请求方式：** GET（**HTTPS**）
> **请求URL：** https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=ID&corpsecret=SECRET

使用postman发送这样一个GET请求即可得到access_token（请把截图中的corpid、corpsecret换为自己的corpid、应用secret）

![](https://p.qpic.cn/pic_wework/23479275/cbcd1bc76736185aac9b29d80e9662914a1420f8b70c8d5c/0)

**2.构造消息体**

参考[发送应用消息](https://developer.work.weixin.qq.com/document/path/90487#10167)，可以发送文本、图片、视频等多种类型的应用消息，这里以最简单的文本消息为例：（注意修改touser、agentid为自己想要的接收者userid列表、应用id）

```json
{
   "touser" : "abelzhu|ZhuShengben",
   "msgtype" : "text",
   "agentid" : 1000002,
   "text" : {
       "content" : "我就试一下"
   },
   "safe":0
}
```

**3.发送消息**

如下图示，以上面两步得到的access_token和消息体为参数，在postman中发送post请求即可（红色箭头所指为需要注意的点）

![](https://p.qpic.cn/pic_wework/23479275/7375a37f245a6996a62365f450262cd61ed10a8eab3b0412/0)

如果发送成功，在接收者的企业微信中的相应应用里，会收到一条文本消息。

![](https://p.qpic.cn/pic_wework/23479275/4a4c9a46383e0b3a226a28b2c4c079b371e3b73ac5202697/0)

实际上，企业微信提供了一套更方便的模拟工具，见[开发者工具](https://developer.work.weixin.qq.com/document/path/90487#12222)的“接口调试工具”。

### debug模式调用接口

在开发过程中，可能由于你调用的参数有问题，我们的接口会返回errcode, 此时你可以在“[全局错误码说明](https://developer.work.weixin.qq.com/document/path/90487#10649)”查阅相应的错误原因。有时候可能根据错误码说明你仍然不知道自己的参数在哪里出错，这时候你可以在接口请求url里加上debug=1参数（暂未支持微盘相关接口），之后从接口返回的errmsg复制出hint值，再用以下工具进行查询，我们会返回你请求的完整参数（包括header与body）。

> 请求示例：https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=ACCESS_TOKEN&debug=1

> 查询页面：[https://open.work.weixin.qq.com/devtool/query](https://open.work.weixin.qq.com/devtool/query)
> ![](https://wework.qpic.cn/wwpic/918154_NkmdWW2GRUGpcBe_1599227618/0)

注意: debug模式有使用频率限制，同一个api每分钟不能超过5次，所以在完成调试之后，请记得要去掉debug=1参数。

### 使用php版本demo开始开发

为体验以代码的方式调用api的乐趣，下面以php开发语言为例，开发者需有一定的php基础。

从[github](https://github.com/sbzhu/weworkapi_php)下载示例代码。可以看到代码结构为：

> ├── api // API 接口
> │ ├── datastructure // API接口需要使用到的一些数据结构
> │ ├── examples // API接口的测试用例
> │ ├── README.md
> │ └── src // API接口的关键逻辑
> ├── callback // 消息回调的一些方法
> ├── config.php // 基础配置
> ├── README.md
> └── utils // 一些基础方法

在 api/example/ 路径下，有个 config.php 文件，用于配置自己的企业id、应用id等信息。

如下图所示，修改 CORP_ID、APP_ID、APP_SECRET 为自己的企业信息。

![](https://p.qpic.cn/pic_wework/23479275/e7e137ebb7ef90b168829eeafa901cac50ef476ac73e8bdd/0)

在 api/example/ 路径下，有个发送消息的示例 **MessageTest.php** ，如下图所示，修改 touser（发送给的成员id列表）、toparty（发送给的部门id列表）、totag（发送给的标签id列表）等参数为自己的企业的信息。

![](https://p.qpic.cn/pic_wework/23479275/c2c412ce272e99c76298696dd36f844f45a5c05a5cd1a229/0)

执行 **MessageTest.php** 即完成消息发送（调试期间建议在根目录的 config.php 文件里，配置DEBUG参数为true，如果有失败，会打印错误信息）。如果发送成功，接收者的应用里会收到一条消息。

![](https://p.qpic.cn/pic_wework/23479275/2224f22e658fd99381dab40a17e66eed3448b7bcbe2c857d/0)

### 使用应用菜单

要实现这样的简单功能：用户点击应用菜单后，展现一个静态网页，告知企业后台系统的内存使用情况。

需要使用Apache搭建企业后台服务，请自行配置好php和Apache环境。

#### 部署应用后台

我们需要在自己的服务器维护一个页面。以常用的 Apache + php 为例，搭建一个简单的企业后台。
在 Apache 的 website 目录下，创建一个页面 getmemoryusage.php，该文件内容如下：

```javascript
<?php
echo "system memory usage " . memory_get_usage() . "B\n";
?>
```

页面非常简单，通过系统函数获取内存使用情况，并打印出来。
这个页面的地址为 http://ip:port/getmemoryusage.php （如何得到ip、port，请参考Apache文献，此处不详述）
如果有正确配置 Apache 服务，在当前浏览器里输入 http://ip:port/getmemoryusage.php ，可以看到页面。
如何让这个页面在企业微信应用中展现呢？

#### 配置应用菜单

在管理端进入上面创建好的自建应用，点击“自定义菜单”项，菜单名自取，菜单内容选“跳转到网页”，URL填上 http://ip:port/getmemoryusage.php ，保存并发布，即完成了添加应用菜单的过程。

应用菜单发布后，从企业微信终端进入该应用，可以看到菜单已经更新。点击菜单即可看到企业后台系统的内存使用情况了！
注意，上面的URL在真实使用时必须外网可访问。如果只是用于测试，外网不可访问，那么测试的终端必须与页面所在服务器在同一个网段，或者使用内网穿透工具（如花生壳）。
 