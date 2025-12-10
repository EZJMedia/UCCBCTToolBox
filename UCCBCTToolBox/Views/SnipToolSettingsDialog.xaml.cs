using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for CenterPointSettingsDialog.xaml
    /// </summary>
    public partial class SnipToolSettingsDialog : Window
    {
        private string _imageType;

        public string ImageType { get => _imageType; set => _imageType = value; }

        private IEnumerable<string> _imageTypes;
        public IEnumerable<string> AvailableImageTypes => _imageTypes;
        public SnipToolSettingsDialog(string imageType)
        {
            _imageTypes = new List<string>()
            {
                "PNG",
                "JPG",
            };
            _imageType = imageType;
            DataContext = this;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
