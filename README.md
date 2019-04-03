# Daylily
一款面向C#语言，基于.NET Core的机器人框架。
> 包含通用可扩展框架、CoolQ HTTP API插件的SDK、Daylily Bot（黄花菜）、已有CoolQ插件

## 指引
本项目分为以下几个部分：框架核心、SDK、插件、测试。

> * 框架核心请至：[Kernal](./Kernal)
> * SDK请至：[ExternalInterfaces](./ExternalInterfaces) 【**CoolQ HTTP API SDK:** [Daylily.CoolQ](./ExternalInterfaces/Daylily.CoolQ)】
> * 黄花菜已有插件、应用请至：[Plugins](./Plugins)

## 如果……
### 你想增加或改进黄花菜机器人的互动功能，你需要：
* 了解Daylily框架的[插件开发](./README_PLUGIN.md)
* 了解[CoolQ HTTP API](https://github.com/richardchien/coolq-http-api)的相关接口，并会使用[CoolQ HTTP API SDK](./ExternalInterfaces/Daylily.CoolQ/README.md)
* 了解[如何测试](./README_TEST.md)自己的插件
* 提交插件项目的Pull Request

### 你想使用框架，打造自己的基于CoolQ的QQ机器人，你需要：
* Fork本项目修改（稳定），或使用Submodule添加到自己的项目（方便框架的后续功能更新）
* 了解如何基于ASP.NET Core[配置Daylily](./Daylily.AspNetCore/README.md)
* 了解Daylily框架的[插件开发](./README_PLUGIN.md)
* 了解CoolQ基础知识、[CoolQ HTTP API](https://github.com/richardchien/coolq-http-api)的相关接口，并会使用[CoolQ HTTP API SDK](./ExternalInterfaces/Daylily.CoolQ/README.md)
