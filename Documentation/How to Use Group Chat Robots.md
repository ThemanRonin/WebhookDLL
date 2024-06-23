## 如何使用群机器人

* 在终端某个群组添加机器人之后，创建者可以在机器人详情页看到该机器人特有的webhookurl。开发者可以按以下说明向这个地址发起HTTP POST 请求，即可实现给该群组发送消息。下面举个简单的例子.
  假设webhook是：https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=693a91f6-7xxx-4bc4-97a0-0ec2sifa5aaa

> 特别特别要注意：一定要保护好机器人的webhook地址，避免泄漏！不要分享到github、博客等可被公开查阅的地方，否则坏人就可以用你的机器人来发垃圾消息了。

以下是用curl工具往群组推送文本消息的示例（注意要将url替换成你的机器人webhook地址，content必须是utf8编码）：

```javascript
curl 'https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=693axxx6-7aoc-4bc4-97a0-0ec2sifa5aaa' \
   -H 'Content-Type: application/json' \
   -d '
   {
    	"msgtype": "text",
    	"text": {
        	"content": "hello world"
    	}
   }'
```

* 当前自定义机器人支持文本（text）、markdown（markdown）、图片（image）、图文（news）、文件（file）、语音（voice）、模板卡片（template_card）七种消息类型。
* 机器人的text/markdown类型消息支持在content中使用<@userid>扩展语法来@群成员

## 消息类型及数据格式

### 文本类型

```javascript
{
    "msgtype": "text",
    "text": {
        "content": "广州今日天气：29度，大部分多云，降雨概率：60%",
		"mentioned_list":["wangqing","@all"],
		"mentioned_mobile_list":["13800001111","@all"]
    }
}
```

| 参数                          | 是否必填 | 说明                                                                                                                             |
| ------------------------------- | ---------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| msgtype                       | 是       | 消息类型，此时固定为text                                                                                                         |
| content                       | 是       | 文本内容，最长不超过2048个字节，必须是utf8编码                                                                                   |
| mentioned\_list            | 否       | userid的列表，提醒群中的指定成员(@某个成员)，@all表示提醒所有人，如果开发者获取不到userid，可以使用mentioned\_mobile\_list |
| mentioned\_mobile\_list | 否       | 手机号列表，提醒手机号对应的群成员(@某个成员)，@all表示提醒所有人                                                                |

![](https://p.qpic.cn/pic_wework/220922847/2325e72fe7c62f371e828acf452e08a60bd20cf4402516af/0)

### markdown类型

```json
{
    "msgtype": "markdown",
    "markdown": {
        "content": "实时新增用户反馈<font color=\"warning\">132例</font>，请相关同事注意。\n
         >类型:<font color=\"comment\">用户反馈</font>
         >普通用户反馈:<font color=\"comment\">117例</font>
         >VIP用户反馈:<font color=\"comment\">15例</font>"
    }
}
```

| 参数    | 是否必填 | 说明                                               |
| --------- | ---------- | ---------------------------------------------------- |
| msgtype | 是       | 消息类型，此时固定为markdown                       |
| content | 是       | markdown内容，最长不超过4096个字节，必须是utf8编码 |

![](https://p.qpic.cn/pic_wework/3887590517/1e18b50e1f1517a6328199454b42c9501fe22d7ddcc2457d/0)

目前支持的markdown语法是如下的子集：

1. 标题 （支持1至6级标题，注意#与文字中间要有空格）

    ```javascript
    # 标题一
    ## 标题二
    ### 标题三
    #### 标题四
    ##### 标题五
    ###### 标题六
    ```
2. 加粗

    ```javascript
    **bold**
    ```
3. 链接

    ```javascript
    [这是一个链接](http://work.weixin.qq.com/api/doc)
    ```
4. 行内代码段（暂不支持跨行）

    ```javascript
    `code`
    ```
5. 引用

    ```javascript
    > 引用文字
    ```
6. 字体颜色(只支持3种内置颜色)

    ```javascript
    <font color="info">绿色</font>
    <font color="comment">灰色</font>
    <font color="warning">橙红色</font>
    ```

### 图片类型

```javascript
{
    "msgtype": "image",
    "image": {
        "base64": "DATA",
		"md5": "MD5"
    }
}
```

| 参数    | 是否必填 | 说明                            |
| --------- | ---------- | --------------------------------- |
| msgtype | 是       | 消息类型，此时固定为image       |
| base64  | 是       | 图片内容的base64编码            |
| md5     | 是       | 图片内容（base64编码前）的md5值 |

> 注：图片（base64编码前）最大不能超过2M，支持JPG,PNG格式

![](https://p.qpic.cn/pic_wework/2882333867/6b6d7c66fb1b8cbecf9e3a89ad87b6ef135d3c140496be31/0)

### 图文类型

```javascript
{
    "msgtype": "news",
    "news": {
       "articles" : [
           {
               "title" : "中秋节礼品领取",
               "description" : "今年中秋节公司有豪礼相送",
               "url" : "www.qq.com",
               "picurl" : "http://res.mail.qq.com/node/ww/wwopenmng/images/independent/doc/test_pic_msg1.png"
           }
        ]
    }
}
```

| 参数        | 是否必填 | 说明                                                                                   |
| ------------- | ---------- | ---------------------------------------------------------------------------------------- |
| msgtype     | 是       | 消息类型，此时固定为news                                                               |
| articles    | 是       | 图文消息，一个图文消息支持1到8条图文                                                   |
| title       | 是       | 标题，不超过128个字节，超过会自动截断                                                  |
| description | 否       | 描述，不超过512个字节，超过会自动截断                                                  |
| url         | 是       | 点击后跳转的链接。                                                                     |
| picurl      | 否       | 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图 1068\*455，小图150\*150。 |

![](https://p.qpic.cn/pic_wework/3634058781/8187918cc7b9ba346403e2f42b1b7bbf9c5889067ac6b227/0)

### 文件类型

```javascript
{
    "msgtype": "file",
    "file": {
 		"media_id": "3a8asd892asd8asd"
    }
}
```

| 参数         | 是否必填 | 说明                               |
| -------------- | ---------- | ------------------------------------ |
| msgtype      | 是       | 消息类型，此时固定为file           |
| media\_id | 是       | 文件id，通过下文的文件上传接口获取 |

![](https://wwcdn.weixin.qq.com/node/wework/images/202005122030.3ce0bde714.png)

### 语音类型

```javascript
{
	"msgtype": "voice",
	"voice": {
		"media_id": "MEDIA_ID"
	}
}
```

| 参数         | 是否必填 | 说明                                   |
| -------------- | ---------- | ---------------------------------------- |
| msgtype      | 是       | 语音类型，此时固定为voice              |
| media\_id | 是       | 语音文件id，通过下文的文件上传接口获取 |

### 模版卡片类型

#### 文本通知模版卡片

![](https://wework.qpic.cn/wwpic/856731_brMLKQZDQrGVRob_1633937847/0)

```javascript
{
    "msgtype":"template_card",
    "template_card":{
        "card_type":"text_notice",
        "source":{
            "icon_url":"https://wework.qpic.cn/wwpic/252813_jOfDHtcISzuodLa_1629280209/0",
            "desc":"企业微信",
            "desc_color":0
        },
        "main_title":{
            "title":"欢迎使用企业微信",
            "desc":"您的好友正在邀请您加入企业微信"
        },
        "emphasis_content":{
            "title":"100",
            "desc":"数据含义"
        },
        "quote_area":{
            "type":1,
            "url":"https://work.weixin.qq.com/?from=openApi",
            "appid":"APPID",
            "pagepath":"PAGEPATH",
            "title":"引用文本标题",
            "quote_text":"Jack：企业微信真的很好用~\nBalian：超级好的一款软件！"
        },
        "sub_title_text":"下载企业微信还能抢红包！",
        "horizontal_content_list":[
            {
                "keyname":"邀请人",
                "value":"张三"
            },
            {
                "keyname":"企微官网",
                "value":"点击访问",
                "type":1,
                "url":"https://work.weixin.qq.com/?from=openApi"
            },
            {
                "keyname":"企微下载",
                "value":"企业微信.apk",
                "type":2,
                "media_id":"MEDIAID"
            }
        ],
        "jump_list":[
            {
                "type":1,
                "url":"https://work.weixin.qq.com/?from=openApi",
                "title":"企业微信官网"
            },
            {
                "type":2,
                "appid":"APPID",
                "pagepath":"PAGEPATH",
                "title":"跳转小程序"
            }
        ],
        "card_action":{
            "type":1,
            "url":"https://work.weixin.qq.com/?from=openApi",
            "appid":"APPID",
            "pagepath":"PAGEPATH"
        }
    }
}
```

请求参数

| 参数              | 类型   | 必须 | 说明                           |
| :------------------ | :------- | :----- | :------------------------------- |
| msgtype           | String | 是   | 消息类型，此时的消息类型固定为`template_card` |
| template\_card | Object | 是   | 具体的模版卡片参数             |

template_card的参数说明

| 参数                                         | 类型     | 必须 | 说明                                                                                                          |
| :--------------------------------------------- | :--------- | :----- | :-------------------------------------------------------------------------------------------------------------- |
| card\_type                                | String   | 是   | 模版卡片的模版类型，文本通知模版卡片的类型为`text_notice`                                                                  |
| source                                       | Object   | 否   | 卡片来源样式信息，不需要来源样式可不填写                                                                      |
| source.icon\_url                          | String   | 否   | 来源图片的url                                                                                                 |
| source.desc                                  | String   | 否   | 来源图片的描述，建议不超过13个字                                                                              |
| source.desc\_color                        | Int      | 否   | 来源文字的颜色，目前支持：0(默认) 灰色，1 黑色，2 红色，3 绿色                                                |
| main\_title                               | Object   | 是   | 模版卡片的主要内容，包括一级标题和标题辅助信息                                                                |
| main\_title.title                         | String   | 否   | 一级标题，建议不超过26个字。**模版卡片主要内容的一级标题maintitle.title和二级普通文本subtitletext必须有一项填写**                                                                                  |
| main\_title.desc                          | String   | 否   | 标题辅助信息，建议不超过30个字                                                                                |
| emphasis\_content                         | Object   | 否   | 关键数据样式                                                                                                  |
| emphasis\_content.title                   | String   | 否   | 关键数据样式的数据内容，建议不超过10个字                                                                      |
| emphasis\_content.desc                    | String   | 否   | 关键数据样式的数据描述内容，建议不超过15个字                                                                  |
| quote\_area                               | Object   | 否   | 引用文献样式，建议不与关键数据共用                                                                            |
| quote\_area.type                          | Int      | 否   | 引用文献样式区域点击事件，0或不填代表没有点击事件，1 代表跳转url，2 代表跳转小程序                            |
| quote\_area.url                           | String   | 否   | 点击跳转的url，quote\_area.type是1时必填                                                                   |
| quote\_area.appid                         | String   | 否   | 点击跳转的小程序的appid，quote\_area.type是2时必填                                                         |
| quote\_area.pagepath                      | String   | 否   | 点击跳转的小程序的pagepath，quote\_area.type是2时选填                                                      |
| quote\_area.title                         | String   | 否   | 引用文献样式的标题                                                                                            |
| quote\_area.quote\_text                | String   | 否   | 引用文献样式的引用文案                                                                                        |
| sub\_title\_text                       | String   | 否   | 二级普通文本，建议不超过112个字。**模版卡片主要内容的一级标题maintitle.title和二级普通文本subtitletext必须有一项填写**                                                                             |
| horizontal\_content\_list              | Object[] | 否   | 二级标题+文本列表，该字段可为空数组，但有数据的话需确认对应字段是否必填，列表长度不超过6                      |
| horizontal\_content\_list.type         | Int      | 否   | 模版卡片的二级标题信息内容支持的类型，1是url，2是文件附件，3 代表点击跳转成员详情                             |
| horizontal\_content\_list.keyname      | String   | 是   | 二级标题，建议不超过5个字                                                                                     |
| horizontal\_content\_list.value        | String   | 否   | 二级文本，如果horizontal\_content\_list.type是2，该字段代表文件名称（要包含文件类型），建议不超过26个字 |
| horizontal\_content\_list.url          | String   | 否   | 链接跳转的url，horizontal\_content\_list.type是1时必填                                                  |
| horizontal\_content\_list.media\_id | String   | 否   | 附件的[mediaid](https://developer.work.weixin.qq.com/document/path/91770#14404/%E6%96%87%E4%BB%B6%E4%B8%8A%E4%BC%A0%E6%8E%A5%E5%8F%A3)，horizontal\_content\_list.type是2时必填                                                         |
| horizontal\_content\_list.userid       | String   | 否   | 成员详情的userid，horizontal\_content\_list.type是3时必填                                               |
| jump\_list                                | Object[] | 否   | 跳转指引样式的列表，该字段可为空数组，但有数据的话需确认对应字段是否必填，列表长度不超过3                     |
| jump\_list.type                           | Int      | 否   | 跳转链接类型，0或不填代表不是链接，1 代表跳转url，2 代表跳转小程序                                            |
| jump\_list.title                          | String   | 是   | 跳转链接样式的文案内容，建议不超过13个字                                                                      |
| jump\_list.url                            | String   | 否   | 跳转链接的url，jump\_list.type是1时必填                                                                    |
| jump\_list.appid                          | String   | 否   | 跳转链接的小程序的appid，jump\_list.type是2时必填                                                          |
| jump\_list.pagepath                       | String   | 否   | 跳转链接的小程序的pagepath，jump\_list.type是2时选填                                                       |
| card\_action                              | Object   | 是   | 整体卡片的点击跳转事件，text\_notice模版卡片中该字段为必填项                                               |
| card\_action.type                         | Int      | 是   | 卡片跳转类型，1 代表跳转url，2 代表打开小程序。text\_notice模版卡片中该字段取值范围为[1,2]                 |
| card\_action.url                          | String   | 否   | 跳转事件的url，card\_action.type是1时必填                                                                  |
| card\_action.appid                        | String   | 否   | 跳转事件的小程序的appid，card\_action.type是2时必填                                                        |
| card\_action.pagepath                     | String   | 否   | 跳转事件的小程序的pagepath，card\_action.type是2时选填                                                     |

#### 图文展示模版卡片

![](https://wework.qpic.cn/wwpic/82758_ROBYYTD8RjGbGXY_1633937902/0)

```javascript
{
    "msgtype":"template_card",
    "template_card":{
        "card_type":"news_notice",
        "source":{
            "icon_url":"https://wework.qpic.cn/wwpic/252813_jOfDHtcISzuodLa_1629280209/0",
            "desc":"企业微信",
            "desc_color":0
        },
        "main_title":{
            "title":"欢迎使用企业微信",
            "desc":"您的好友正在邀请您加入企业微信"
        },
        "card_image":{
            "url":"https://wework.qpic.cn/wwpic/354393_4zpkKXd7SrGMvfg_1629280616/0",
            "aspect_ratio":2.25
        },
        "image_text_area":{
            "type":1,
            "url":"https://work.weixin.qq.com",
            "title":"欢迎使用企业微信",
            "desc":"您的好友正在邀请您加入企业微信",
            "image_url":"https://wework.qpic.cn/wwpic/354393_4zpkKXd7SrGMvfg_1629280616/0"
        },
        "quote_area":{
            "type":1,
            "url":"https://work.weixin.qq.com/?from=openApi",
            "appid":"APPID",
            "pagepath":"PAGEPATH",
            "title":"引用文本标题",
            "quote_text":"Jack：企业微信真的很好用~\nBalian：超级好的一款软件！"
        },
        "vertical_content_list":[
            {
                "title":"惊喜红包等你来拿",
                "desc":"下载企业微信还能抢红包！"
            }
        ],
        "horizontal_content_list":[
            {
                "keyname":"邀请人",
                "value":"张三"
            },
            {
                "keyname":"企微官网",
                "value":"点击访问",
                "type":1,
                "url":"https://work.weixin.qq.com/?from=openApi"
            },
            {
                "keyname":"企微下载",
                "value":"企业微信.apk",
                "type":2,
                "media_id":"MEDIAID"
            }
        ],
        "jump_list":[
            {
                "type":1,
                "url":"https://work.weixin.qq.com/?from=openApi",
                "title":"企业微信官网"
            },
            {
                "type":2,
                "appid":"APPID",
                "pagepath":"PAGEPATH",
                "title":"跳转小程序"
            }
        ],
        "card_action":{
            "type":1,
            "url":"https://work.weixin.qq.com/?from=openApi",
            "appid":"APPID",
            "pagepath":"PAGEPATH"
        }
    }
}
```

请求参数

| 参数              | 类型   | 必须 | 说明                 |
| :------------------ | :------- | :----- | :--------------------- |
| msgtype           | String | 是   | 模版卡片的消息类型为`template_card` |
| template\_card | Object | 是   | 具体的模版卡片参数   |

template_card的参数说明

| 参数                                         | 类型     | 必须 | 说明                                                                                                          |
| :--------------------------------------------- | :--------- | :----- | :-------------------------------------------------------------------------------------------------------------- |
| card\_type                                | String   | 是   | 模版卡片的模版类型，图文展示模版卡片的类型为`news_notice`                                                                  |
| source                                       | Object   | 否   | 卡片来源样式信息，不需要来源样式可不填写                                                                      |
| source.icon\_url                          | String   | 否   | 来源图片的url                                                                                                 |
| source.desc                                  | String   | 否   | 来源图片的描述，建议不超过13个字                                                                              |
| source.desc\_color                        | Int      | 否   | 来源文字的颜色，目前支持：0(默认) 灰色，1 黑色，2 红色，3 绿色                                                |
| main\_title                               | Object   | 是   | 模版卡片的主要内容，包括一级标题和标题辅助信息                                                                |
| main\_title.title                         | String   | 是   | 一级标题，建议不超过26个字                                                                                    |
| main\_title.desc                          | String   | 否   | 标题辅助信息，建议不超过30个字                                                                                |
| card\_image                               | Object   | 是   | 图片样式                                                                                                      |
| card\_image.url                           | String   | 是   | 图片的url                                                                                                     |
| card\_image.aspect\_ratio              | Float    | 否   | 图片的宽高比，宽高比要小于2.25，大于1.3，不填该参数默认1.3                                                    |
| image\_text\_area                      | Object   | 否   | 左图右文样式                                                                                                  |
| image\_text\_area.type                 | Int      | 否   | 左图右文样式区域点击事件，0或不填代表没有点击事件，1 代表跳转url，2 代表跳转小程序                            |
| image\_text\_area.url                  | String   | 否   | 点击跳转的url，image\_text\_area.type是1时必填                                                          |
| image\_text\_area.appid                | String   | 否   | 点击跳转的小程序的appid，必须是与当前应用关联的小程序，image\_text\_area.type是2时必填                  |
| image\_text\_area.pagepath             | String   | 否   | 点击跳转的小程序的pagepath，image\_text\_area.type是2时选填                                             |
| image\_text\_area.title                | String   | 否   | 左图右文样式的标题                                                                                            |
| image\_text\_area.desc                 | String   | 否   | 左图右文样式的描述                                                                                            |
| image\_text\_area.image\_url        | String   | 是   | 左图右文样式的图片url                                                                                         |
| quote\_area                               | Object   | 否   | 引用文献样式，建议不与关键数据共用                                                                            |
| quote\_area.type                          | Int      | 否   | 引用文献样式区域点击事件，0或不填代表没有点击事件，1 代表跳转url，2 代表跳转小程序                            |
| quote\_area.url                           | String   | 否   | 点击跳转的url，quote\_area.type是1时必填                                                                   |
| quote\_area.appid                         | String   | 否   | 点击跳转的小程序的appid，quote\_area.type是2时必填                                                         |
| quote\_area.pagepath                      | String   | 否   | 点击跳转的小程序的pagepath，quote\_area.type是2时选填                                                      |
| quote\_area.title                         | String   | 否   | 引用文献样式的标题                                                                                            |
| quote\_area.quote\_text                | String   | 否   | 引用文献样式的引用文案                                                                                        |
| vertical\_content\_list                | Object[] | 否   | 卡片二级垂直内容，该字段可为空数组，但有数据的话需确认对应字段是否必填，列表长度不超过4                       |
| vertical\_content\_list.title          | String   | 是   | 卡片二级标题，建议不超过26个字                                                                                |
| vertical\_content\_list.desc           | String   | 否   | 二级普通文本，建议不超过112个字                                                                               |
| horizontal\_content\_list              | Object[] | 否   | 二级标题+文本列表，该字段可为空数组，但有数据的话需确认对应字段是否必填，列表长度不超过6                      |
| horizontal\_content\_list.type         | Int      | 否   | 模版卡片的二级标题信息内容支持的类型，1是url，2是文件附件，3 代表点击跳转成员详情                             |
| horizontal\_content\_list.keyname      | String   | 是   | 二级标题，建议不超过5个字                                                                                     |
| horizontal\_content\_list.value        | String   | 否   | 二级文本，如果horizontal\_content\_list.type是2，该字段代表文件名称（要包含文件类型），建议不超过26个字 |
| horizontal\_content\_list.url          | String   | 否   | 链接跳转的url，horizontal\_content\_list.type是1时必填                                                  |
| horizontal\_content\_list.media\_id | String   | 否   | 附件的[mediaid](https://developer.work.weixin.qq.com/document/path/91770#14404/%E6%96%87%E4%BB%B6%E4%B8%8A%E4%BC%A0%E6%8E%A5%E5%8F%A3)，horizontal\_content\_list.type是2时必填                                                         |
| horizontal\_content\_list.userid       | String   | 否   | 成员详情的userid，horizontal\_content\_list.type是3时必填                                               |
| jump\_list                                | Object[] | 否   | 跳转指引样式的列表，该字段可为空数组，但有数据的话需确认对应字段是否必填，列表长度不超过3                     |
| jump\_list.type                           | Int      | 否   | 跳转链接类型，0或不填代表不是链接，1 代表跳转url，2 代表跳转小程序                                            |
| jump\_list.title                          | String   | 是   | 跳转链接样式的文案内容，建议不超过13个字                                                                      |
| jump\_list.url                            | String   | 否   | 跳转链接的url，jump\_list.type是1时必填                                                                    |
| jump\_list.appid                          | String   | 否   | 跳转链接的小程序的appid，jump\_list.type是2时必填                                                          |
| jump\_list.pagepath                       | String   | 否   | 跳转链接的小程序的pagepath，jump\_list.type是2时选填                                                       |
| card\_action                              | Object   | 是   | 整体卡片的点击跳转事件，news\_notice模版卡片中该字段为必填项                                               |
| card\_action.type                         | Int      | 是   | 卡片跳转类型，1 代表跳转url，2 代表打开小程序。news\_notice模版卡片中该字段取值范围为[1,2]                 |
| card\_action.url                          | String   | 否   | 跳转事件的url，card\_action.type是1时必填                                                                  |
| card\_action.appid                        | String   | 否   | 跳转事件的小程序的appid，card\_action.type是2时必填                                                        |
| card\_action.pagepath                     | String   | 否   | 跳转事件的小程序的pagepath，card\_action.type是2时选填                                                     |