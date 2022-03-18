# daylily
我叫黄花菜w，是基于[MilkiBotFramework](https://github.com/Milkitic/MilkiBotFramework)的机器人。
这是我的开源仓库，包含了大部分的可开源的逻辑。
> 请注意，实际运营的我包含着一些非公开插件。

本项目亦可作为MilkiBotFramework示例仓库，这将教你如何优雅地使用该框架编写机器人。

## 指引
不同插件使用了MilkiBotFramework的框架的不同处理方法，请根据实际需求参考。

### 基础互动插件
* 【@检测】`CheckCqAt`：解析富文本、发送富文本，获取机器人账号信息
* 【缩写查询】`CsxPlugin`：命令处理、调用HTTP请求
* 【龙语识别】`DragonLanguage`：复杂逻辑相关
* 【Ping-pong】`Echo`
* 【关键词触发】`KeywordTrigger`
* 【konachan】`Konachan`
* 【获取随机数】`Roll`
* 【自助禁言】`Smoke`：使用专用API
* 【图灵】`Tuling`
* 【洗衣机插件】`WashingMachine`：图片处理示例（包括gif）、上下文对话
* 【今天吃什么】`WhatToEat`：托管配置处理，获取Bot配置文件
* 【Bot帮助】`Help`：使用UI框架绘图，获取所有插件信息

### osu!插件
* `AuthenticationController`：ASP.NET Core Web API的控制器声明
* `OsuDbContext`：托管的数据库上下文声明
* `ApiService`
* 【绑定osu!账号】`SetId`：与Controller的互动、EventBus的使用、插件之间注入
* 【成绩查询】`RecentPlayPlugin`：参数级权限声明
* 【个人名片】`MePlugin`：使用托管的数据库
* 【个人介绍页面】`UserPagePlugin`
* 【BN信息搜索】`SearchBn`
* 【地图数据订阅】`BeatmapStatistics`

### 基础处理/控制插件
* 【控制台消息输出】`CliPrint`
* 【命令热度分析】`CommandRate`
* 【聊天记录（非持久化）】`MessageLogging`
* 【插件管理】`PluginFilter`：控制插件行为、获取用户输入解析结果
* 【睡眠】`PowerOff`：任务计划程序、原生MessageApi
* 【睡眠控制消息输出】`PowerOffFilterService`：`ServicePlugin`的`BeforeSend`的使用
* 【远程重启】`Reboot`：任务计划程序、原生MessageApi
* 【发送自定义消息】`SendMessage`：限制命令的权限、群私聊

### 服务插件
* 【缓存定时清理】`CacheManagerService`
* 【控制台消息输出（Bot）】`CliPrintMe`
* `FailBindingReplyService`：`ServicePlugin`的`OnBindingFailed`的使用
* `FailExecuteReplyService`：`ServicePlugin`的`OnPluginException`的使用
* 【内存溢出检测】`MemoryCheckService`
* 【发言重复过滤】`MessageAvoidRepeatService`
* 【敏感词屏蔽】`SensitiveMatchService`

## 许可声明
本项目代码部分采用MIT许可。

其中包含的Resources/Fonts的字体文件为 Google 的 Source Sans Pro，其采用 [Open Font License](https://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL)。