using System.Windows.Media;
using System.Windows.Controls;
using UCCBCTToolBox.ViewModels;
using System.Windows;
using UCCBCTToolBox.Classes;
using UCCBCTToolBox.Helpers;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for MainCalcsPanel.xaml
    /// </summary>
    public partial class InstructionView : Window
    {
        private readonly InstructionViewModel model;
        public InstructionView()
        {
            InitializeComponent();
            model = (InstructionViewModel)DataContext;
        }
    }
}
