using Wpf.Ui.Controls;
using ytdlProtocol.Modern.ViewModels.Pages;

namespace ytdlProtocol.Modern.Views.Pages;
public partial class DataPage : INavigableView<DataViewModel>
{
    public DataViewModel ViewModel { get; }

    public DataPage(DataViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
