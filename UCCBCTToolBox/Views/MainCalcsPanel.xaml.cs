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
    public partial class MainCalcsPanel : WrapPanel
    {
        private readonly MainCalcsPanelViewModel model;
        public MainCalcsPanel()
        {
            InitializeComponent();
            model = (MainCalcsPanelViewModel)DataContext;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            model.ResetAllInput();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Canvas canvas = Parent as Canvas;
            double left = Canvas.GetLeft(this), top = Canvas.GetTop(this);

            Point scs = canvas.PointToScreen(new Point(left, top));
            Point sce = canvas.PointToScreen(new Point(left + ActualWidth, top + SaveArea.ActualHeight));
            SnipTool.SaveSnip(ScreenSnip.CopyScreen((int)scs.X, (int)scs.Y, (int)sce.X, (int)sce.Y));
        }

        private void Autoload_Click(object sender, RoutedEventArgs e)
        {
            model.Autoload();
        }

        public double GetHeadTilt()
        {
            return model.HeadTiltInput;
        }
        public bool IsHeadTiltRight => model.HTModeR;
        public double GetSpineTilt()
        {
            return model.CSTInput;
        }
        public bool IsSpineTiltRight => model.CSTModeR;
        public double GetFA()
        {
            return model.FACalc;
        }
        public bool IsFATiltHigh => model.FAText == "+";
        public bool IsFACRight => model.FACText == "R";

        private void CopyToClipBoard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(
            model.FACText + "\t" +
            model.FACCalc + "\t" +
            0 + "\t" +
            model.FASText + "\t" +
            model.FASCalc + "\t" +
            model.HACText + "\t" +
            model.HACCalc + "\t" +
            model.HAxText + "\t" +
            model.HAxCalc / 3 + "\t" +
            (model.FAText == "-" ? model.FACalc * -1 : model.FACalc) + "\t" + 
            model.CInput + "\t" +
            model.AInput
            );
        }

        private void CalculateOrthospinology_Click(object sender, RoutedEventArgs e)
        {
            model.OnOrthospinologyCalculateClick();
        }
    }
}
