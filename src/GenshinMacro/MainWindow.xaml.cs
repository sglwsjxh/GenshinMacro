global using System.Windows;
global using System.Windows.Input;

namespace GenshinMacro;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is ViewModels.MainWindowViewModel viewModel)
            viewModel.Shutdown();

        base.OnClosed(e);
    }
}
