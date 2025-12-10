using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UCCBCTToolBox.Notifiers;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for CalibrationDialog.xaml
    /// </summary>
    public partial class CalibrationDialog : Window
    {
        public bool IsCancel = true;
        public ClientStateManager ClientStateManager { get; private set; }
        public CalibrationDialog()
        {
            ClientStateManager = new ClientStateManager();
            DataContext = this;
            InitializeComponent();
            ClientStateManager.SetTop();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = false;
            Close();
        }
    }
}
