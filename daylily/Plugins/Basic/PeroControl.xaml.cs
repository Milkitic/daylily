using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Data;
using MilkiBotFramework.Imaging.Wpf;
using MilkiBotFramework.Imaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining;
using Image = SixLabors.ImageSharp.Image;
using Path = System.IO.Path;

namespace daylily.Plugins.Basic;

[PluginIdentifier("411dd6b0-5557-4255-94ba-d31dced4a89e", "舔", Scope = "daylily",
    Authors = "milkiticyf")]
public class Pero : BasicPlugin
{
    private readonly LightHttpClient _lightHttpClient;

    public Pero(LightHttpClient lightHttpClient)
    {
        _lightHttpClient = lightHttpClient;
    }

    [CommandHandler("pero")]
    public async Task<IResponse> PeroCore(MessageContext context)
    {
        var richMsg = context.GetRichMessage();
        var userId = richMsg.OfType<At>().FirstOrDefault()?.UserId;
        byte[]? avatarBytes;
        if (userId != null)
        {
            var uri = $"http://q1.qlogo.cn/g?b=qq&nk={userId}&s=640";
            (avatarBytes, _) = await _lightHttpClient.GetImageBytesFromUrlAsync(uri);
        }
        else
        {
            var firstImage = richMsg.OfType<LinkImage>().FirstOrDefault();
            if (firstImage != null)
            {
                (avatarBytes, _) = await _lightHttpClient.GetImageBytesFromUrlAsync(firstImage.Uri);
            }
            else
            {
                var qq = context.MessageUserIdentity?.UserId;
                var uri = $"http://q1.qlogo.cn/g?b=qq&nk={qq}&s=640";
                (avatarBytes, _) = await _lightHttpClient.GetImageBytesFromUrlAsync(uri);
            }
        }

        var renderer = new WpfDrawingProcessor<PeroVm, PeroControl>(true);

        var peroVm = new PeroVm(avatarBytes, Path.Combine(PluginHome, "base.png"));
        var image = await renderer.ProcessAsync(peroVm);
        return Reply(new MemoryImage(image, ImageType.Png));
    }
}

public class PeroVm : ViewModelBase
{
    private ImageSource? _avatar;

    public PeroVm(byte[] avatarBytes, string baseImagePath)
    {
        AvatarBytes = avatarBytes;
        BaseImagePath = Path.GetFullPath(baseImagePath);
    }

    public string BaseImagePath { get; }
    public byte[] AvatarBytes { get; }
    public ImageSource? Avatar
    {
        get => _avatar;
        set => SetField(ref _avatar, value);
    }
}

/// <summary>
/// PeroControl.xaml 的交互逻辑
/// </summary>
public partial class PeroControl : WpfDrawingControl
{
    private readonly PeroVm _viewModel;

    public PeroControl(object viewModel, Image? sourceImage = null)
        : base(viewModel, sourceImage)
    {
        InitializeComponent();
        Loaded += async (_, _) => await FinishDrawing();
        _viewModel = (PeroVm)ViewModel;

        var memoryStream = new MemoryStream(_viewModel.AvatarBytes);
        var bitmapSource = new BitmapImage();

        bitmapSource.BeginInit();
        bitmapSource.StreamSource = memoryStream;
        bitmapSource.CacheOption = BitmapCacheOption.None;
        bitmapSource.EndInit();

        _viewModel.Avatar = bitmapSource;
    }
}