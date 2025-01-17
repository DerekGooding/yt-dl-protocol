using Microsoft.Win32;
using System.IO;
using yt_dl_protocol;
using yt_dl_protocol.Properties;

namespace ytdlProtocol.Modern.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _ytDlDownloadPath;
    [ObservableProperty]
    private string _downloadPath;
    [ObservableProperty]
    private string _additionalArgs;
    [ObservableProperty]
    private bool _isAutoClose;

    public DashboardViewModel()
    {
        YtDlDownloadPath = Settings.Default.ytdl_path;
        DownloadPath = Settings.Default.download_path;
        AdditionalArgs = Settings.Default.additional_args;

        IsAutoClose = Settings.Default.autoclose_on_finish;
        UpdateCanRegisterState();
    }

    private void UpdateCanRegisterState()
    {
        //bool isRegistered = Utils.IsProtocolRegistered(Settings.Default.protocol_ytdl);
        //bool isValidYtDlPath = File.Exists(Settings.Default.ytdl_path) || File.Exists(YtDlDownloadPath);
        //bool isValidDlPath = Directory.Exists(Settings.Default.download_path) || Directory.Exists(DownloadPath);

        //bool canRegister = isValidYtDlPath && isValidDlPath;

        //TODO
        //UpdateButton.Enabled = isValidYtDlPath;

        //if (canRegister)
        //{
        //    UpdateButton.Visible = true;
        //    ProtocolStatusPictureBox.Image = isRegistered ? Resources.success : Resources.warning;
        //    ProtocolStatusPictureBox.Cursor = Cursors.Default;
        //    ProtocolToolTip.SetToolTip(ProtocolStatusPictureBox, isRegistered ? "The protocol has currently been registered." : "The protocol has currently not been registered.");
        //    ProtocolStatusLabel.Cursor = Cursors.Default;
        //}
        //else
        //{
        //    UpdateButton.Visible = false;
        //    ProtocolStatusPictureBox.Image = Resources.error;
        //    ProtocolStatusPictureBox.Cursor = Cursors.Help;
        //    ProtocolToolTip.SetToolTip(ProtocolStatusPictureBox, "You need to set up the required paths below before you can register the protocol.");
        //    ProtocolStatusLabel.Cursor = Cursors.Help;
        //}

        RegisterCommand.NotifyCanExecuteChanged();
        UnregisterCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void Browse()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Executable Files|*.exe",
            Title = "Select youtube-dl or yt-dlp executable",
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true
        };

        if (openFileDialog.ShowDialog() == true
            && File.Exists(openFileDialog.FileName)
            && Utils.ValidateExecutableIsYtDl(openFileDialog.FileName))
        {
            YtDlDownloadPath = openFileDialog.FileName;
            Settings.Default.ytdl_path = YtDlDownloadPath;
        }
        else
        {
            MessageBox.Show("The provided file is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        UpdateCanRegisterState();
    }
    [RelayCommand(CanExecute = nameof(IsNotRegistered))]
    public void Register() => HandleProtocolRegistering(true);
    [RelayCommand(CanExecute = nameof(IsRegistered))]
    public void Unregister() => HandleProtocolRegistering(false);

    private void HandleProtocolRegistering(bool register)
    {
        Settings.Default.protocol_registered = register
            ? Utils.RegisterURLProtocol("ytdl", System.Reflection.Assembly.GetExecutingAssembly().Location, "yt-dl-protocol-handler")
            : Utils.UnregisterURLProtocol("ytdl");

        UpdateCanRegisterState();
        SaveSettings();
    }

    private void SaveSettings()
    {
        Settings.Default.autoclose_on_finish = IsAutoClose;
        Settings.Default.additional_args = AdditionalArgs;
        Settings.Default.Save();
        Settings.Default.Reload();
        UpdateCanRegisterState();
    }
    [RelayCommand(CanExecute = nameof(CanRegister))]
    private void Save()
    {
        if (!File.Exists(YtDlDownloadPath) || !Directory.Exists(DownloadPath))
        {
            MessageBox.Show("The settings could not be saved due to one or more invalid paths.",
                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            SaveSettings();
            MessageBox.Show("The settings have been saved successfully.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        UpdateCanRegisterState();
    }

    [RelayCommand]
    private void CheckForUpdates() => new CommandForm("-U").ShowDialog();

    [RelayCommand]
    private void BrowseDownloadPath()
    {
        OpenFolderDialog selectFolderDialog = new()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Title = "Select a destination",
        };
        if (selectFolderDialog.ShowDialog() == true && Directory.Exists(selectFolderDialog.FolderName))
        {
            DownloadPath = selectFolderDialog.FolderName;
            Settings.Default.download_path = DownloadPath;
        }
        else
        {
            MessageBox.Show("The provided directory is invalid.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        UpdateCanRegisterState();
    }
    [RelayCommand]
    public void Guide() => new BookmarkForm().ShowDialog();

    private void ProtocolStatusPictureBox_BackgroundImageChanged(object sender, EventArgs e)
    {
        //TODO
        //if (ProtocolStatusPictureBox.BackgroundImage == Properties.Resources.error)
        //{
        //    if (File.Exists(Settings.Default.ytdl_path))
        //    {
        //        ProtocolStatusPictureBox.Cursor = Cursors.Default;
        //        ProtocolStatusLabel.Cursor = Cursors.Default;
        //    }
        //    else
        //    {
        //        ProtocolStatusPictureBox.Cursor = Cursors.Help;
        //        ProtocolStatusLabel.Cursor = Cursors.Help;
        //    }
        //}
        //else
        //{
        //    ProtocolStatusPictureBox.Cursor = Cursors.Default;
        //    ProtocolStatusLabel.Cursor = Cursors.Default;
        //}
    }

    private bool IsRegistered() => Utils.IsProtocolRegistered(Settings.Default.protocol_ytdl);
    private bool IsNotRegistered() => !IsRegistered();
    private bool CanRegister()
        => File.Exists(Settings.Default.ytdl_path) || File.Exists(YtDlDownloadPath)
        && Directory.Exists(Settings.Default.download_path) || Directory.Exists(DownloadPath);
}
