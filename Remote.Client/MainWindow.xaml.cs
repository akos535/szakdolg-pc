using System.ComponentModel;
using System.Windows;
using Remote.Server;

namespace Remote.Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ServerRunner _serverRunner = new();
    private bool _isClosing;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartButton.IsEnabled = false;
        StatusText.Text = "Starting server...";

        try
        {
            await _serverRunner.StartAsync();

            StatusText.Text = "Server running on ports 5000 and 5001";
            StopButton.IsEnabled = true;
        }
        catch(Exception ex)
        {
            StatusText.Text = $"Failed to start: {ex.Message}";
            StartButton.IsEnabled = true;
        }
    }

    private async void StopButton_Click(object sender, RoutedEventArgs e)
    {
        await StopServerAsync();
    }

    private async void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_isClosing) return;
        e.Cancel = true;
        _isClosing = true;

        await StopServerAsync();
    }

    private async Task StopServerAsync()
    {
        if (!_serverRunner.IsRunning) return;

        StatusText.Text = "Stopping server...";

        try
        {
            await _serverRunner.DisposeAsync();

            StatusText.Text = "Server not running";
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

        }
        catch (Exception ex)
        {
            StatusText.Text = $"Failed to stop server: {ex.Message}";
        }
    }
}