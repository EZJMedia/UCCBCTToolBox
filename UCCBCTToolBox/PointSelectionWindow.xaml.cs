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

namespace UCCBCTToolBox
{
    /// <summary>
    /// Interaction logic for PointSelectionWindow.xaml
    /// </summary>
    public partial class PointSelectionWindow : Window
    {
        public static RoutedCommand routedCommand = new();
        private List<Point> points = new();
        private bool canMakeNewPoints = true;
        public PointSelectionWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            routedCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            routedCommand.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            routedCommand.InputGestures.Add(new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control));
        }

        private Ellipse GetDrawablePoint(double cx, double cy, SolidColorBrush stroke, double w = 10, double h = 10)
        {
            Ellipse ellipse = new()
            {
                Height = h,
                Width = w,
                Stroke = stroke,
                Fill = stroke,
            };

            Canvas.SetLeft(ellipse, (cx - ellipse.Width / 2));
            Canvas.SetTop(ellipse, (cy - ellipse.Height / 2));

            return ellipse;
        }

        private Path GetDrawableCross(double cx, double cy, SolidColorBrush stroke, double side = 8)
        {
            string geometry = "M0, 0L" + side + ", " + side + "M" + side + ", 0L0, " + side;
            Path path = new()
            {
                Data = Geometry.Parse(geometry),
                Stroke = stroke,
                Width = side,
                Height = side,
            };
            Canvas.SetLeft(path, (cx - path.Width / 2));
            Canvas.SetTop(path, (cy - path.Height / 2));

            return path;
        }

        private void DrawCenterPoint()
        {
            Point mid = GetMidPoint(points[points.Count - 1], points[points.Count - 2]);
            //MessageBox.Show(mid.ToString(), "PreviewMouseUp");
            
            innerCanvas.Children.Add(GetDrawableCross(mid.X, mid.Y, Brushes.Yellow));
        }

        private Point GetMidPoint(Point point1, Point point2)
        {
            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        private void CTRL_C_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                canMakeNewPoints = !canMakeNewPoints;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {

            }
        }

        private void logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }

        private void innerCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!canMakeNewPoints)
                return;
            Point p = e.GetPosition(this);
            p.Y -= actionBar.ActualHeight;
            points.Add(p);
            if (points.Count > 0 && points.Count % 2 == 0)
                DrawCenterPoint();

            innerCanvas.Children.Add(GetDrawablePoint(p.X, p.Y, Brushes.Red));
        }

        private void MakePointsTransparent_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            points.Clear();
            innerCanvas.Children.Clear();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private bool _isMoving;
        private Point? _buttonPosition;
        private double deltaX;
        private double deltaY;
        private TranslateTransform _currentTT;

        private void innerButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            canMakeNewPoints = false;
            if (_buttonPosition == null)
                _buttonPosition = innerButton.TransformToAncestor(innerCanvas).Transform(new Point(0, 0));
            var mousePosition = Mouse.GetPosition(innerCanvas);
            deltaX = mousePosition.X - _buttonPosition.Value.X;
            deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;
        }

        private void innerButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _currentTT = innerButton.RenderTransform as TranslateTransform;
            _isMoving = false;
            canMakeNewPoints = true;
        }

        private void innerButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition(innerCanvas);

            var offsetX = (_currentTT == null ? _buttonPosition.Value.X : _buttonPosition.Value.X - _currentTT.X) + deltaX - mousePoint.X;
            var offsetY = (_currentTT == null ? _buttonPosition.Value.Y : _buttonPosition.Value.Y - _currentTT.Y) + deltaY - mousePoint.Y;

            this.innerButton.RenderTransform = new TranslateTransform(-offsetX, -offsetY);
        }
    }
}
