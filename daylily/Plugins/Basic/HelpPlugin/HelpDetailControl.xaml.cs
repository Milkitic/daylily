using System;
using System.Collections.Generic;
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
using MilkiBotFramework.Imaging.Wpf;
using Image = SixLabors.ImageSharp.Image;

namespace daylily.Plugins.Basic.HelpPlugin
{
    /// <summary>
    /// HelpDetailControl.xaml 的交互逻辑
    /// </summary>
    public sealed partial class HelpDetailControl : WpfDrawingControl
    {
        private readonly HelpDetailVm _viewModel;

        public HelpDetailControl(object viewModel, Image? sourceImage = null)
            : base(viewModel, sourceImage)
        {
            InitializeComponent();
            Loaded += async (_, _) => await FinishDrawing();
            _viewModel = (HelpDetailVm)ViewModel;
        }
    }
}
