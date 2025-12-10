using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UCCBCTToolBox.Classes;
using UCCBCTToolBox.ViewModels;
using UCCBCTToolBox.Views;

namespace UCCBCTToolBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand routedCommand = new();

        private readonly MainWindowViewModel model;

        public static DpiScale ScreenDPI { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = model = (MainWindowViewModel)OuterCanvas.DataContext;

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            routedCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            routedCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

            ScreenDPI = VisualTreeHelper.GetDpi(this);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            InnerCanvas.Children.Clear();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RoutedCommands_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                Global.CurrentTool = new Tool(InnerCanvas);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.Z))
                {
                    if (Global.CurrentTool.TheTool.GetType() != typeof(NullTool))
                    {
                        Global.CurrentTool.TheTool.DoAction(Tool.ActionUndo, this);
                    }
                }
            }
        }

        private void CenterPointButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new(InnerCanvas, CenterPoint.ToolName);
        }

        private void FreehandButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, Freehand.ToolName);
        }

        private void SnipToolButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, SnipTool.ToolName);
        }

        private void FreehandSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FreehandSettingsDialog(Freehand.FillColor, Freehand.StrokeThickness)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                Freehand.StrokeThickness = dialog.StrokeThickness;
                Freehand.FillColor = dialog.FillColor;
            }
        }

        private void Transparent50_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement child in InnerCanvas.Children)
            {
                child.Opacity = child.Opacity == 1 ? 0.5 : 1;
            }
        }

        private int _mainCalcsIdx = -1;
        private void MainCalcsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainCalcsIdx >= 0)
            {
                OuterCanvas.Children[_mainCalcsIdx].Visibility = OuterCanvas.Children[_mainCalcsIdx].Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                return;
            }
            //Debug.WriteLine("MainCalcs");
            WrapPanel control = new MainCalcsPanel();
            Canvas.SetLeft(control, 20);
            Canvas.SetTop(control, 60);

            _mainCalcsIdx = OuterCanvas.Children.Add(control);
        }

        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            ThreePointCircle.ToolType = ThreePointCircle.CTool;
            Global.CurrentTool = new Tool(InnerCanvas, ThreePointCircle.ToolName);
        }

        private void AButton_Click(object sender, RoutedEventArgs e)
        {
            ThreePointCircle.ToolType = ThreePointCircle.ATool;
            Global.CurrentTool = new Tool(InnerCanvas, ThreePointCircle.ToolName);
        }

        private void Calibrate_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, CalibrateTool.ToolName);
        }

        private void ProtractorBtn_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, Protractor.ToolName);
        }

        private void HAButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, HAVATool.ToolName);
            Global.CurrentTool.TheTool.DoAction(HAVATool.ActionSetOrientation, HAVATool.OrientationHorizontal);
        }

        private void VAButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, HAVATool.ToolName);
            Global.CurrentTool.TheTool.DoAction(HAVATool.ActionSetOrientation, HAVATool.OrientationVertical);
        }

        private void Misalignment_Click(object sender, RoutedEventArgs e)
        {
            if (_mainCalcsIdx == -1)
            {
                System.Windows.Forms.MessageBox.Show("Please input values into Main Calcs first");
                return;
            }
            MainCalcsPanel panel = (MainCalcsPanel)OuterCanvas.Children[_mainCalcsIdx];

            Global.CurrentTool = new Tool(InnerCanvas, MisalignmentTool.ToolName);
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetHeadTilt, panel.GetHeadTilt());
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetSpineTilt, panel.GetSpineTilt());
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetFA, panel.GetFA());
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetHTRight, panel.IsHeadTiltRight);
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetSTRight, panel.IsSpineTiltRight);
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetFACRight, panel.IsFACRight);
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionSetFAHigh, panel.IsFATiltHigh);
            Global.CurrentTool.TheTool.DoAction(MisalignmentTool.ActionDrawMisalignment, 1);
        }

        private void InsertText_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, TextTool.ToolName);
        }

        private void CenterPointSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new(InnerCanvas, CenterPoint.ToolName);

            Global.CurrentTool.TheTool.DoAction(CenterPoint.ActionSetSettings, this);
        }

        private void StraightLineButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new(InnerCanvas, StraightLine.ToolName);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new(InnerCanvas, SnipImport.ToolName);

            Global.CurrentTool.TheTool.DoAction(SnipImport.ActionImport, this);
        }

        private void DeactivateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DeactivateDialog()
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                Global.DeleteValueFromRegistry("Serial");
                Close();
            }
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void InstructionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var inst = new InstructionView();
            inst.Show();
        }

        private void CephaloBtn_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentTool = new Tool(InnerCanvas, CephaloTool.ToolName);
        }

        private void SnipToolSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if current tool is snip tool
            if (typeof(SnipTool) != Global.CurrentTool.TheTool.GetType())
            {
                System.Windows.Forms.MessageBox.Show("Please select Snip Tool first");
                return;
            }

            Global.CurrentTool.TheTool.DoAction(SnipTool.ActionSettingsDialog, this);
        }
    }
}
