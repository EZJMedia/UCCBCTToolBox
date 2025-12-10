using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for CenterPointSettingsDialog.xaml
    /// </summary>
    public partial class CenterPointSettingsDialog : Window
    {
        private string _dot, _cross;

        public string DotColor
        {
            get { return _dot; }
            set { _dot = value; }
        }
        public string CrossColor
        {
            get { return _cross; }
            set { _cross = value; }
        }

        private IEnumerable<string> _brushes;
        public IEnumerable<string> AvailableBrushes => _brushes;
        public CenterPointSettingsDialog(string dot, string cross)
        {
            _brushes = new List<string>()
            {
                "Red",
                "Green",
                "Blue",
                "Yellow",
                "Purple",
                "Orange",
                "Pink",
                "Black",
                "White",
            };
            _dot = dot;
            _cross = cross;
            DataContext = this;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
