using Wpf.Ui.Controls;
using ytdlProtocol.Modern.ViewModels.Pages;

namespace ytdlProtocol.Modern.Views.Pages;
public partial class DashboardPage : INavigableView<DashboardViewModel>
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
