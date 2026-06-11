global using System.Windows;

namespace GenshinMacro;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is ViewModels.MainWindowViewModel viewModel)
            viewModel.Shutdown();

        base.OnClosed(e);
    }
}
