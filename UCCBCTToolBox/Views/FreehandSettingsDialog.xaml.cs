using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for FreehandSettingsDialog.xaml
    /// </summary>
    public partial class FreehandSettingsDialog : Window
    {
        private int _strokeThickness;
        private string _fillColor;
        public string FillColor
        { get { return _fillColor; } set { _fillColor = value; } }
        public int StrokeThickness
        { get { return _strokeThickness; } set { _strokeThickness = value; } }

        private IEnumerable<string> _brushes;
        public IEnumerable<string> AvailableBrushes => _brushes;
        public FreehandSettingsDialog(string color = "Red", int thick = 1)
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
            _strokeThickness = thick;
            _fillColor = color;
            DataContext = this;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
