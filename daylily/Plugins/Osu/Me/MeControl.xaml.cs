using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coosu.Api.V2.ResponseModels;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Imaging.Wpf;
using Image = SixLabors.ImageSharp.Image;

namespace daylily.Plugins.Osu.Me;

public class MeOsuControlVm : INotifyPropertyChanged
{
    private User? _user;
    private ImageSource? _avatar;
    private ImageSource? _cover;

    public User? User
    {
        get => _user;
        set => SetField(ref _user, value);
    }

    public ImageSource? Avatar
    {
        get => _avatar;
        set => SetField(ref _avatar, value);
    }

    public ObservableCollection<ImageSource> Badges { get; } = new();

    public ObservableCollection<GridModel> GridModels { get; } = new();

    public ImageSource? Cover
    {
        get => _cover;
        set => SetField(ref _cover, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class GridModel
{
    public string Name { get; set; }
    public object Content { get; set; }
    public GridModelType Type { get; set; }
}

public enum GridModelType
{
    String, ImageList,
    ImageList2,
    Boolean
}

/// <summary>
/// MeControl.xaml 的交互逻辑
/// </summary>
public partial class MeControl : WpfDrawingControl
{
    private readonly LightHttpClient _lightHttpClient;
    private readonly MeOsuControlVm _viewModel;

    public MeControl(LightHttpClient lightHttpClient,
        object viewModel, Image? sourceImage = null) : base(viewModel, sourceImage)
    {
        _lightHttpClient = lightHttpClient;
        _viewModel = (MeOsuControlVm)viewModel;
        InitializeComponent();
    }

    private async void MeOsuControl_OnInitialized(object? sender, EventArgs e)
    {
        var user = _viewModel.User;
        if (user == null) return;

        try
        {
            var image = await _lightHttpClient.GetImageBytesFromUrlAsync(user.AvatarUrl);
            var bitmapImage = ByteToImageSource(image.Item1);

            _viewModel.Avatar = bitmapImage;
        }
        catch (Exception exception)
        {
        }

        try
        {
            var image = await _lightHttpClient.GetImageBytesFromUrlAsync(user.CoverUrl);
            var bitmapImage = ByteToImageSource(image.Item1);

            _viewModel.Cover = bitmapImage;
        }
        catch (Exception exception)
        {
        }

        if (user.Badges != null)
        {
            foreach (var userBadge in user.Badges)
            {
                try
                {
                    var image = await _lightHttpClient.GetImageBytesFromUrlAsync(userBadge.ImageUrl);
                    var bitmapImage = ByteToImageSource(image.Item1);
                    _viewModel.Badges.Add(bitmapImage);
                }
                catch (Exception exception)
                {
                }
            }
        }
        if (_viewModel.Cover != null)
            _viewModel.GridModels.Add(new GridModel { Name = "封面", Content = new[] { _viewModel.Cover }, Type = GridModelType.ImageList });
        _viewModel.GridModels.Add(new GridModel { Name = "头像", Content = new[] { _viewModel.Avatar }, Type = GridModelType.ImageList });
        _viewModel.GridModels.Add(new GridModel
        {
            Name = "户口",
            Content = (string.IsNullOrWhiteSpace(user.Location) ? "" : user.Location + ", ") + user.Country?.Name
        });
        _viewModel.GridModels.Add(new GridModel { Name = "用户名", Content = user.Username });
        _viewModel.GridModels.Add(new GridModel { Name = "氪金状态", Content = user.HasSupported, Type = GridModelType.Boolean });
        _viewModel.GridModels.Add(new GridModel { Name = "主玩模式", Content = user.Playmode });
        if (!string.IsNullOrWhiteSpace(user.Discord))
            _viewModel.GridModels.Add(new GridModel { Name = "Discord", Content = user.Discord });
        if (!string.IsNullOrWhiteSpace(user.Twitter))
            _viewModel.GridModels.Add(new GridModel { Name = "Twitter", Content = user.Twitter });
        if (!string.IsNullOrWhiteSpace(user.Website))
            _viewModel.GridModels.Add(new GridModel { Name = "网址", Content = user.Website });
        if (!string.IsNullOrWhiteSpace(user.Interests))
            _viewModel.GridModels.Add(new GridModel { Name = "兴趣", Content = user.Interests });
        if (!string.IsNullOrWhiteSpace(user.Occupation))
            _viewModel.GridModels.Add(new GridModel { Name = "职业", Content = user.Occupation });
        _viewModel.GridModels.Add(new GridModel { Name = "好友关注", Content = user.FollowerCount });
        _viewModel.GridModels.Add(new GridModel { Name = "Rank图", Content = user.RankedAndApprovedBeatmapsetCount });
        _viewModel.GridModels.Add(new GridModel { Name = "Pending图", Content = user.UnrankedBeatmapsetCount });
        _viewModel.GridModels.Add(new GridModel { Name = "坟图", Content = user.GraveyardBeatmapsetCount });
        if (!string.IsNullOrWhiteSpace(user.Title))
            _viewModel.GridModels.Add(new GridModel { Name = "头衔", Content = user.Title });
        if (_viewModel.Badges.Count > 0)
            _viewModel.GridModels.Add(new GridModel { Name = "狗牌", Content = _viewModel.Badges, Type = GridModelType.ImageList2 });

        await Task.Delay(50);
        await FinishDrawing();
    }

    private static BitmapImage ByteToImageSource(byte[] bytes)
    {
        var memoryStream = new MemoryStream(bytes);
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memoryStream;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        if (e.Row.DataContext is GridModel gridModel)
        {
            if (gridModel.Type == GridModelType.String)
            {
                e.Row.Height = 20;
            }
            //e.Row.Height = 32;
        }
    }
}