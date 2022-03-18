using System.ComponentModel;
using System.IO;
using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Imaging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace daylily.Plugins.Basic;

[PluginIdentifier("A520D94D-B9EA-46BD-9D74-EB50FFC6443C", "洗衣机插件", Scope = "mjbot")]
[Description("图片处理集合")]
public class WashingMachine : BasicPlugin
{
    private readonly ILogger<WashingMachine> _logger;
    private readonly BotOptions _botOptions;

    private readonly LightHttpClient _httpClient;
    //private const int MaxWidthOrHeight = 256;
    private const int MaxWidthOrHeight2X = 512;

    public WashingMachine(ILogger<WashingMachine> logger, BotOptions botOptions, LightHttpClient httpClient)
    {
        _logger = logger;
        _botOptions = botOptions;
        _httpClient = httpClient;
    }

    [CommandHandler("baojiang")]
    [Description("深度烘焙")]
    public IAsyncEnumerable<IResponse?> Baojiang(
        [Argument, Description("图片")] LinkImage? linkImage = null)
    {
        return Green(68, 24, linkImage);
    }

    [CommandHandler("lightgreen")]
    [Description("轻度烘焙")]
    public IAsyncEnumerable<IResponse?> LightGreen(
        [Argument, Description("图片")] LinkImage? linkImage = null)
    {
        return Green(75, 8, linkImage);
    }

    [CommandHandler("green")]
    [Description("包浆图片")]
    public async IAsyncEnumerable<IResponse?> Green(
        [Description("质量"), Option("q")] int quality = 75,
        [Description("循环次数"), Option("i")] int iterations = 16,
        [Description("图片"), Argument] LinkImage? linkImage = null)
    {
        if (linkImage == null)
        {
            await foreach (var (response, image) in EnsureImage(linkImage))
            {
                if (image != null)
                {
                    linkImage = image;
                    break;
                }

                yield return response;
            }
        }

        if (iterations > 300) iterations = 300;
        else if (iterations < 1) iterations = 1;

        if (quality > 100) quality = 100;
        else if (quality < 0) quality = 0;

        using var memoryImage = (await _httpClient.GetImageFromUrlAsync(linkImage!.Uri)).InMemoryImage;
        var max = Math.Max(memoryImage.Width, memoryImage.Height);
        var ratio = max <= MaxWidthOrHeight2X ? 1 : (float)MaxWidthOrHeight2X / max;

        using var resizedImage = ImageHelper.GetResizedImage(memoryImage, ratio);
        var processedImage = await Processing.ProcessGreenAsync(resizedImage, quality, iterations);

        yield return Reply(new MemoryImage(processedImage, ImageType.Jpeg));
    }

    [CommandHandler("rotate")]
    [Description("转动图片")]
    public async IAsyncEnumerable<IResponse?> Rotate(
        [Description("帧总数"), Option("f")] uint frames = 4,
        [Description("帧间延时（毫秒）"), Option("d")] uint delay = 800,
        [Description("是否逆时针转"), Option("rev")] bool reverse = false,
        [Description("图片"), Argument] LinkImage? linkImage = null)
    {
        if (frames < 4)
        {
            yield return Reply("帧数最低需包含4帧..");
            yield break;
        }

        if (frames > 32)
        {
            yield return Reply("帧数最高不得超过64帧");
            yield break;
        }

        if (linkImage == null)
        {
            await foreach (var (response, image) in EnsureImage(linkImage))
            {
                if (image != null)
                {
                    linkImage = image;
                    break;
                }

                yield return response;
            }
        }

        using var memoryImage = (await _httpClient.GetImageFromUrlAsync(linkImage!.Uri)).InMemoryImage;
        var max = Math.Max(memoryImage.Width, memoryImage.Height);
        var ratio = max <= MaxWidthOrHeight2X ? 1 : (float)MaxWidthOrHeight2X / max;

        using var resizedImage = ImageHelper.GetResizedImage(memoryImage, ratio);
        var processedImage = await Processing.ProcessRotateAsync(resizedImage, reverse, (int)frames,
            TimeSpan.FromMilliseconds(delay), _logger);

        var processedPath = await ImageHelper.CompressToFileAsync
            (_botOptions.GifSiclePath, _botOptions.CacheImageDir, processedImage);
        yield return Reply(new FileImage(processedPath));
    }

    [CommandHandler("shake")]
    [Description("抖动图片")]
    public async IAsyncEnumerable<IResponse?> Shake(
        [Description("抖动X范围"), Option("x")] uint xRange = 2,
        [Description("抖动Y范围"), Option("y")] uint yRange = 2,
        [Description("帧总数"), Option("f")] uint frames = 5,
        [Description("帧间延时（毫秒）"), Option("d")] uint delay = 50,
        [Description("图片"), Argument] LinkImage? linkImage = null)
    {
        if (frames < 2)
        {
            yield return Reply("帧数最低需包含2帧..");
            yield break;
        }

        if (frames > 25)
        {
            yield return Reply("帧数最高不得超过25帧");
            yield break;
        }

        if (linkImage == null)
        {
            await foreach (var (response, image) in EnsureImage(linkImage))
            {
                if (image != null)
                {
                    linkImage = image;
                    break;
                }

                yield return response;
            }
        }

        using var memoryImage = (await _httpClient.GetImageFromUrlAsync(linkImage!.Uri)).InMemoryImage;
        var max = Math.Max(memoryImage.Width, memoryImage.Height);
        var ratio = max <= MaxWidthOrHeight2X ? 1 : (float)MaxWidthOrHeight2X / max;

        using var resizedImage = ImageHelper.GetResizedImage(memoryImage, ratio);
        var processedImage = await Processing.ProcessShakeAsync(resizedImage, (int)xRange, (int)yRange, (int)frames,
            TimeSpan.FromMilliseconds(delay), _logger);

        var processedPath = await ImageHelper.CompressToFileAsync
            (_botOptions.GifSiclePath, _botOptions.CacheImageDir, processedImage);
        yield return Reply(new FileImage(processedPath));
    }

    [CommandHandler("reverse")]
    [Description("Gif倒放")]
    public async IAsyncEnumerable<IResponse?> Reverse(
        [Description("图片"), Argument] LinkImage? linkImage = null)
    {
        if (linkImage == null)
        {
            await foreach (var (response, image) in EnsureImage(linkImage))
            {
                if (image != null)
                {
                    linkImage = image;
                    break;
                }

                yield return response;
            }
        }

        using var memoryImage = (await _httpClient.GetImageFromUrlAsync(linkImage!.Uri)).InMemoryImage;
        var max = Math.Max(memoryImage.Width, memoryImage.Height);
        var ratio = max <= MaxWidthOrHeight2X ? 1 : (float)MaxWidthOrHeight2X / max;

        using var resizedImage = ImageHelper.GetResizedImage(memoryImage, ratio);
        var processedImage = await Processing.ProcessReverseAsync(resizedImage,
            new Size((int)(memoryImage.Width * ratio), (int)(memoryImage.Height * ratio)),
            _logger);

        var processedPath = await ImageHelper.CompressToFileAsync
            (_botOptions.GifSiclePath, _botOptions.CacheImageDir, processedImage);
        yield return Reply(new FileImage(processedPath));
    }

    private static async IAsyncEnumerable<(IResponse? response, LinkImage? linkImage)> EnsureImage(LinkImage? linkImage)
    {
        yield return (Reply("请发送一张图片", out var nextMessage), linkImage);
        while (linkImage == null)
        {
            var message = await nextMessage.GetNextMessageAsync(20);
            linkImage = (LinkImage?)message.GetRichMessage().FirstOrDefault(k => k is LinkImage);

            if (linkImage == null)
            {
                yield return (Reply("找不到图片或者有多个图片，请重发..", out nextMessage), null);
            }
            else
            {
                yield return (null, linkImage);
            }
        }
    }

    private static class Processing
    {
        public static async Task<Image> ProcessReverseAsync(Image source, Size? size, ILogger logger)
        {
            if (size != null)
                source.Mutate(k => k.Resize(size.Value));
            var list = new List<GifFrame>();
            try
            {
                list = await ImageHelper.CloneImagesFromFramesAsync(source.Frames);
                if (list.Count > 1)
                {
                    var lastDelay = list[^1].Delay;
                    for (var i = list.Count - 1; i >= 1; i--)
                    {
                        var preKvp = list[i - 1];
                        var kvp = list[i];
                        kvp.Delay = preKvp.Delay;
                    }

                    list[0].Delay = lastDelay;
                    list.Reverse();
                }

                //list = ImageHelper.CompressSerial(list);
                return await ImageHelper.CreateGifByImagesAsync(list, source.Size(), true);
            }
            finally
            {
                foreach (var (image, _) in list)
                {
                    try
                    {
                        image.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }
            }
        }

        public static async Task<Image> ProcessShakeAsync(Image source, int xRange, int yRange, int frames,
            TimeSpan interval, ILogger logger)
        {
            var arr = Array.Empty<Image>();
            try
            {
                arr = Enumerable.Range(0, frames)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                    .Select(_ =>
                    {
                        int x = Random.Shared.Next(xRange * 2 + 1) - xRange;
                        int y = Random.Shared.Next(yRange * 2 + 1) - yRange;
                        return ImageHelper.GetTranslatedBitmap(source, x, y);
                    })
                    .ToArray();

                //list = ImageHelper.CompressSerial(list);
                return await ImageHelper.CreateGifByImagesAsync(arr, interval, source.Size());
            }
            finally
            {
                foreach (var bitmap in arr)
                {
                    try
                    {
                        bitmap.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Error while disposing bitmap:" + ex.Message);
                    }
                }
            }
        }

        public static async Task<Image> ProcessRotateAsync(Image source, bool reverse, int frames, TimeSpan interval,
            ILogger logger)
        {
            var actualInterval = interval.TotalMilliseconds / frames;
            var arr = Array.Empty<Image>();
            try
            {
                arr = Enumerable.Range(0, frames)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                    .Select(i =>
                    {
                        var angle = !reverse ? 360 * i / frames : 360 * (frames - i) / frames;
                        return ImageHelper.GetRotatedImage(source, angle);
                    })
                    .ToArray();

                //list = ImageHelper.CompressSerial(list);
                return await ImageHelper.CreateGifByImagesAsync(arr, TimeSpan.FromMilliseconds(actualInterval),
                    source.Size());
            }
            finally
            {
                foreach (var bitmap in arr)
                {
                    try
                    {
                        bitmap.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Error while disposing bitmap:" + ex.Message);
                    }
                }
            }
        }

        public static async Task<Image> ProcessGreenAsync(Image image, int quality, int iterations)
        {
            var ms = new MemoryStream();
            using (var copy = image.Clone(_ => { }))
            {
                await copy.SaveAsJpegAsync(ms);
            }

            for (int k = 0; k < iterations; k++)
            {
                ms.Position = 0;
                using var imageI = (Image<Rgb24>)await Image.LoadAsync(ms);
                var height = imageI.Height;

                imageI.ProcessPixelRows(pixelAccessor =>
                {
                    for (int j = 0; j < height; j++)
                    {
                        Span<Rgb24> row = pixelAccessor.GetRowSpan(j);

                        // Using row.Length helps JIT to eliminate bounds checks when accessing row[x].
                        for (var i = 0; i < row.Length; i++)
                        {
                            var color = row[i];
                            byte r, g, b;
                            r = color.R;
                            g = color.G;
                            b = color.B;

                            var y = Clamp((77 * r + 150 * g + 29 * b) >> 8);
                            var u = ClampUv(((-43 * r - 85 * g + 128 * b) >> 8) - 1);
                            var v = ClampUv(((128 * r - 107 * g - 21 * b) >> 8) - 1);

                            var r1 = Clamp((65536 * y + 91881 * v) >> 16);
                            var g1 = Clamp((65536 * y - 22553 * u - 46802 * v) >> 16);
                            var b1 = Clamp((65536 * y + 116130 * u) >> 16);

                            row[i] = new Rgb24(r1, g1, b1 /*, color.A*/);
                        }
                    }
                });

                await ms.DisposeAsync();
                ms = new MemoryStream();
                await imageI.SaveAsJpegAsync(ms, new JpegEncoder
                {
                    Quality = quality,
                    ColorType = JpegColorType.Rgb
                });
            }

            ms.Position = 0;
            var outputBitmap = await Image.LoadAsync(ms);
            await ms.DisposeAsync();
            return outputBitmap;
        }

        private static byte Clamp(int x) => (byte)(x >= 0 ? (x <= 255 ? x : 255) : 0);

        private static sbyte ClampUv(int x) => (sbyte)(x >= -128 ? (x <= 127 ? x : 127) : -128);
    }
}