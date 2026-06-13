using System.ComponentModel;
using GenshinMacro.Input;
using GenshinMacro.Services;
using GenshinMacro.ViewModels;
using GenshinMacro.Views;
using Wpf.Ui.Controls;

namespace GenshinMacro;

public partial class MainWindow : FluentWindow
{
    private readonly AutoRotationView _rotationView;
    private readonly DoubleMacroView _doubleMacroView;
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(SettingsService settingsService)
    {
        var keyState = new Win32KeyStateProvider();
        _viewModel = new MainWindowViewModel(settingsService, keyState);
        DataContext = _viewModel;

        InitializeComponent();

        // Create page views
        _rotationView = new AutoRotationView(_viewModel.RotationVM);
        _doubleMacroView = new DoubleMacroView(_viewModel.DoubleMacroVM);

        // Show initial page
        ContentArea.Content = _rotationView;

        // Listen for navigation changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.SelectedSection))
        {
            ContentArea.Content = _viewModel.SelectedSection switch
            {
                SelectedSection.AutoRotation => _rotationView,
                SelectedSection.DoubleMacro => _doubleMacroView,
                _ => _rotationView
            };
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel.Shutdown();
        base.OnClosed(e);
    }
}
