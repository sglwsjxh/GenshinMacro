using System.Windows.Controls;
using GenshinMacro.ViewModels;

namespace GenshinMacro.Views;

public partial class DoubleMacroView : UserControl
{
    public DoubleMacroView(DoubleMacroViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
