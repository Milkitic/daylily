# Daylily 插件开发指南
Daylily 的大部分功能基于插件模式，其中包括了框架级功能（例如插件过滤、命令处理、消息过滤等）、应用级功能（与用户互动）。

插件模式的优点在于：使代码的可维护性、可移植度上升，更易配置，并且为横纵向管理提供基石。

## 快速开始
```csharp
namespace QuickStart
{
    [Command("hello", "hi")] // 激活插件的命令，支持多字符串
    [Name("Quick Start插件")] // 插件名
    [Author("yf_extension")] // 插件作者
    [Version(0, 0, 1, PluginVersion.Stable)] // 插件版本
    [Help("这是第一行。", "这是第二行。")] // 插件说明，用于Help查询
    class QuickStartPlugin : CoolQCommandPlugin // 继承CommandPlugin
    {
        public override Guid Guid => new Guid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"); // 每个插件须有独立的Guid，且后期不建议更改

        [FreeArg(Default = "world")]
        public string Reply { get; set; } // 设置Reply自由参数

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            CoolQRouteMessage routeMsg = scope.RouteMessage; // 消息上下文的消息对象
            string rawMessage = routeMsg.RawMessage; // 接收的原始信息
            CoolQIdentity identity = routeMsg.CoolQIdentity; // 所在会话的标识

            MessageType type = identity.Type;

            if (type == MessageType.Private)
            {
                Logger.Info($"这是一条私聊会话，发送者为：{identity.Id}。");
            }
            else if (type == MessageType.Group || type == MessageType.Discuss)
            {
                string sender = routeMsg.UserId;
                Logger.Info($"这是一条群聊会话，发送者为：{sender}，所在群为：{identity.Id}。");
                return null; // 不理会群聊消息，返回null
            }

            return routeMsg.ToSource($"Hello {Reply.Trim()}!"); // ToSource为原路返回至发送者的会话中
        }
    }
}
```
当用户输入 `/hello` 时，会返回 `Hello world!`。
当用户输入 `/hello friend` 时，会返回 `Hello friend!`。

### 请参阅：
* [插件类型](./README_PLUGIN.md##插件类型)
* 插件特性标识、命令特性标识
* 消息上下文


## 插件类型
插件分为三种：`ServicePlugin`、`ApplicationPlugin` 和 `CommandPlugin`，拥有共同基类 `PluginBase`。其中 `ApplicationPlugin` 与 `CommandPlugin` 继承自 `MessagePlugin`，他们仅通过 `Dispatcher` 调用，作用于消息接收时。

* Service Plugin：Bot 初始化后自动按一定顺序运行，且与其他插件互不影响，作为后台服务使用。
* Application Plugin：消息处理的主体，接收消息时以优先级顺序调用。可以对消息上下文进行分析、处理和修改，以及终止调用后续插件。并且可增加自定义 `Tag` ，以在各个 `ApplicationPlugin` 中传递自定义信息。其也可同时配合某些条件或者命令分析器，作为类似 `CommandPlugin` 的插件使用，与用户进行互动。
* CommandPlugin：命令式消息处理的主体，在用户输入符合互动，在 `ApplicationPlugin` 调用完毕后执行。命令会自动由核心插件命令分析器解释。并且对正常使用的属性进行简单的特性标注（Attribute）配置，即可被注入器自动赋值，使用时极方便。
