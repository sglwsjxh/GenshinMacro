using System.Windows.Controls;
using GenshinMacro.ViewModels;

namespace GenshinMacro.Views;

public partial class AutoRotationView : UserControl
{
    public AutoRotationView(AutoRotationViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
