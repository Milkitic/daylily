using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Osu;

[PluginIdentifier("8c138573-60fb-44ec-9f2d-b2077d1e757a", "大丘丘病了二丘丘瞧", Scope = "osu!")]
[Description("三丘丘采药四丘丘熬")]
public class Qqm : BasicPlugin
{
    private string StickerDir { get; set; }

    [CommandHandler("丘丘萌")]
    public async Task<IResponse> QqmImpl(MessageContext messageContext)
    {
        var files = Directory.EnumerateFiles(StickerDir, "*.jpg")
            .Concat(Directory.EnumerateFiles(StickerDir, "*.png"))
            .Concat(Directory.EnumerateFiles(StickerDir, "*.gif"))
            .ToArray();
        if (files.Length > 0)
        {
            var random = Random.Shared.Next(0, files.Length);
            var path = files[random];
            return Reply(new FileImage(path));
        }

        return Reply("未找到图片");
    }

    protected override Task OnInitialized()
    {
        StickerDir = Path.Combine(PluginHome, "stickers");
        Directory.CreateDirectory(StickerDir);
        return Task.CompletedTask;
    }
}