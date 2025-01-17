using Wpf.Ui.Controls;
using ytdlProtocol.Modern.ViewModels.Pages;

namespace ytdlProtocol.Modern.Views.Pages;
public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
