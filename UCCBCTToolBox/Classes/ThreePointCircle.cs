using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;
using UCCBCTToolBox.ViewModels;

namespace UCCBCTToolBox.Classes
{
    internal class ThreePointCircle : ITool
    {
        public const string ToolName = "ThreePointCirlce";
        public const string CTool = "Condylar Circle";
        public const string ATool = "Axial Circle";
        private const string Inch = "in";
        private const string Centimeter = "cm";
        public static string CalibrateUnit { get; set; } = Centimeter;
        public static string ToolType { get; set; } = CTool;
        public static double LengthPerUnit => CalibrateUnit == Inch ? CalibrateLengthPerUnit : CalibrateLengthPerUnit * 2.54;
        public static double CalibrateLengthPerUnit => CalibratedWidth / CalibratedValue;
        public static double CalibratedWidth { get; set; } = 400;
        public static double CalibratedValue { get; set; } = 4;

        private double _circleArea => Math.Round(Math.PI * (CircleRadius / LengthPerUnit) * (CircleRadius / LengthPerUnit), 2);
        private double _circlePerimeter => Math.Round(2 * Math.PI * (CircleRadius / LengthPerUnit), 2);
        private double _circleDiameter => Math.Round(2 * (CircleRadius / LengthPerUnit), 2);

        private Canvas _board;

        private Path _circlePath, _circle1, _circle2;
        private List<ContentControl> _contentControlDots;
        private List<Point> _points;
        private double c, f, g;
        private bool _isFinished;

        private TextBlock _rTitle, _rArea, _rPerimeter, _rDiameter;
        private StackPanel _resultPanel;
        public ThreePointCircle(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _circlePath = new Path();
            _circle1 = new Path();
            _circle2 = new Path();
            _contentControlDots = new List<ContentControl>();
            _points = new List<Point>();

            InitResultPanel();
            InitCalibrationRef();

            _isFinished = false;
        }

        private void InitCalibrationRef()
        {
            Point _pointA = new(0, 0);
            Point _pointB = new(_pointA.X + _board.ActualWidth, 0);
            Path myPath = new()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = Brushes.Red,
                StrokeThickness = 5
            };
            LineGeometry lineGeometry = new()
            {
                StartPoint = _pointA,
                EndPoint = _pointB
            };

            myPath.Data = lineGeometry;
            ContentControl pathPanel = new()
            {
                Height = myPath.StrokeThickness,
                Width = CalibratedWidth,
                Content = myPath,
                Margin = new Thickness(0, 0, 0, 16),
            };

            SolidColorBrush panelBg = new()
            {
                Color = Colors.Black,
                Opacity = 0.5
            };
            StackPanel panel = new()
            {
                Margin = new Thickness(4),
                Width = CalibratedWidth + 16,
                Background = panelBg,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            TextBlock referenceLineInfo = new()
            {
                Text = "Calibration: " + CalibratedValue + " " + CalibrateUnit,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(4, 8, 4, 8),
                TextWrapping = TextWrapping.Wrap,
            };
            if (CalibrateUnit == Centimeter)
            {
                referenceLineInfo.Text += " / " + Math.Round(CalibratedValue / 2.54, 2) + " " + Inch;
            }
            panel.Children.Add(referenceLineInfo);
            panel.Children.Add(pathPanel);

            ContentControl contentControl = new()
            {
                Width = panel.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = panel
            };

            Canvas.SetTop(contentControl, 20);
            Canvas.SetLeft(contentControl, _board.ActualWidth - contentControl.Width - 20);

            _board.Children.Add(contentControl);
        }

        public void InitResultPanel()
        {
            _rTitle = new()
            {
                Text = ToolType,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };

            _rArea = new()
            {
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };

            _rPerimeter = new()
            {
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };

            _rDiameter = new()
            {
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };

            SolidColorBrush panelBg = new()
            {
                Color = Colors.Black,
                Opacity = 0.5
            };
            _resultPanel = new()
            {
                Margin = new Thickness(4),
                Height = 80,
                Width = 160,
                Background = panelBg,
            };
            _resultPanel.Children.Add(_rTitle);
            _resultPanel.Children.Add(_rArea);
            _resultPanel.Children.Add(_rPerimeter);
            _resultPanel.Children.Add(_rDiameter);
        }

        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
            //System.Windows.Forms.MessageBox.Show("TestFrom3");
            e.Handled = true;
        }

        private void Board_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Test");
            if (Global.CurrentTool.Name != ToolName || _isFinished)
            {
                e.Handled = true;
                return;
            }
            //System.Windows.Forms.MessageBox.Show("Test");
            Point clk = e.GetPosition(_board);
            _points.Add(clk);

            Ellipse ellipse = DrawEllipse();
            ContentControl contentControl = new()
            {
                Height = ellipse.Height,
                Width = ellipse.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = ellipse,
                Tag = _contentControlDots.Count
            };



            Canvas.SetTop(contentControl, clk.Y - ellipse.Height / 2);
            Canvas.SetLeft(contentControl, clk.X - ellipse.Width / 2);
            contentControl.PreviewMouseUp += ContentControl_PreviewMouseUp;
            _contentControlDots.Add(contentControl);
            _board.Children.Add(contentControl);

            if (_points.Count >= 4)
            {
                CalculateCenterOfCirclePassingThroughThreePoints(_points[0], _points[1], _points[3]);
                double r1 = CircleRadius;
                _circle1 = DrawCircle(Brushes.Red);

                _board.Children.Add(_circle1);

                CalculateCenterOfCirclePassingThroughThreePoints(_points[0], _points[2], _points[3]);
                double r2 = CircleRadius;
                _circle2 = DrawCircle(Brushes.Red);

                _board.Children.Add(_circle2);

                CalculateCenterOfCirclePassingThroughTwoPointsHavingARadius((r1 + r2) / 2);
                _circlePath = DrawCircle(Brushes.Green);

                _board.Children.Add(_circlePath);

                AddCircleInfo();

                _isFinished = true;
            }
        }

        private void AddCircleInfo()
        {
            _rArea.Text = "Area: " + _circleArea + " " + Inch + "\xB2";

            _rPerimeter.Text = "Perimeter: " + _circlePerimeter + " " + Inch;

            _rDiameter.Text = "Diameter: " + _circleDiameter + " " + Inch;

            ContentControl contentControl = new()
            {
                Height = _resultPanel.Height,
                Width = _resultPanel.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = _resultPanel,
            };

            Canvas.SetTop(contentControl, _points[3].Y - contentControl.Height / 2);
            Canvas.SetLeft(contentControl, _points[3].X + 8);

            _board.Children.Add(contentControl);

            if (ToolType == CTool)
            {
                MainCalcsPanelViewModel.CValue = _circleDiameter;
            }
            else if (ToolType == ATool)
            {
                MainCalcsPanelViewModel.AValue = _circleDiameter;
            }
        }

        private void CalculateCenterOfCirclePassingThroughTwoPointsHavingARadius(double r)
        {
            Point p = _points[0], q = _points[3];
            double d = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
            double D = Math.Sqrt(r * r - 0.25 * d * d);

            Point m = new Point((p.X + q.X) / 2, (p.Y + q.Y) / 2);
            double mul = 2 * D / d;
            Point c1 = new Point(m.X + mul * (p.Y - m.Y), m.Y + mul * (m.X - p.X));
            Point c2 = new Point(m.X - mul * (p.Y - m.Y), m.Y - mul * (m.X - p.X));
            p = c1; q = new(-g, -f);
            double d1 = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
            p = c2;
            double d2 = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
            if (d1 > d2)
            {
                g = -c2.X; f = -c2.Y; c = g * g + f * f - r * r;
            }
            else
            {
                g = -c1.X; f = -c1.Y; c = g * g + f * f - r * r;
            }
        }

        private void ContentControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Global.CurrentTool.Name);
            Point p = e.GetPosition(_board);

            int idx = int.Parse((sender as ContentControl).Tag.ToString());
            _points[idx] = p;

            if (_points.Count < 4) return;

            CalculateCenterOfCirclePassingThroughThreePoints(_points[0], _points[1], _points[3]);
            EllipseGeometry ellipseGeometry1 = new()
            {
                Center = new Point(-g, -f),
                RadiusX = CircleRadius,
                RadiusY = CircleRadius,
            };

            _circle1.Data = ellipseGeometry1;
            double r1 = CircleRadius;

            CalculateCenterOfCirclePassingThroughThreePoints(_points[0], _points[2], _points[3]);
            EllipseGeometry ellipseGeometry2 = new()
            {
                Center = new Point(-g, -f),
                RadiusX = CircleRadius,
                RadiusY = CircleRadius,
            };

            _circle2.Data = ellipseGeometry2;
            double r2 = CircleRadius;

            CalculateCenterOfCirclePassingThroughTwoPointsHavingARadius((r1 + r2) / 2);

            EllipseGeometry ellipseGeometry = new()
            {
                Center = new Point(-g, -f),
                RadiusX = CircleRadius,
                RadiusY = CircleRadius,
            };

            _circlePath.Data = ellipseGeometry;
            UpdateCircleInfo();
        }

        private void UpdateCircleInfo()
        {
            _rArea.Text = "Area: " + _circleArea + " " + Inch + "\xB2";

            _rPerimeter.Text = "Perimeter: " + _circlePerimeter + " " + Inch;

            _rDiameter.Text = "Diameter: " + _circleDiameter + " " + Inch;

            if (ToolType == CTool)
            {
                MainCalcsPanelViewModel.CValue = _circleDiameter;
            }
            else if (ToolType == ATool)
            {
                MainCalcsPanelViewModel.AValue = _circleDiameter;
            }
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
        }

        private Path DrawCircle(SolidColorBrush brush)
        {
            Path myPath = new()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = brush,
                StrokeThickness = 1
            };
            EllipseGeometry ellipseGeometry = new()
            {
                Center = new Point(-g, -f),
                RadiusX = CircleRadius,
                RadiusY = CircleRadius,

            };

            myPath.Data = ellipseGeometry;

            return myPath;
        }

        private void CalculateCenterOfCirclePassingThroughThreePoints(Point _pointA, Point _pointB, Point _pointC)
        {
            double[,] mat = {
                {2 * _pointA.X, 2 * _pointA.Y, 1, - (_pointA.X * _pointA.X) - (_pointA.Y * _pointA.Y) },
                {2 * _pointB.X, 2 * _pointB.Y, 1, - (_pointB.X * _pointB.X) - (_pointB.Y * _pointB.Y) },
                {2 * _pointC.X, 2 * _pointC.Y, 1, - (_pointC.X * _pointC.X) - (_pointC.Y * _pointC.Y) },
            };

            double mul = - mat[1, 0] / mat[0, 0];
            mat[1, 0] += mul * mat[0, 0];
            mat[1, 1] += mul * mat[0, 1];
            mat[1, 2] += mul * mat[0, 2];
            mat[1, 3] += mul * mat[0, 3];

            mul = - mat[2, 0] / mat[0, 0];
            mat[2, 0] += mul * mat[0, 0];
            mat[2, 1] += mul * mat[0, 1];
            mat[2, 2] += mul * mat[0, 2];
            mat[2, 3] += mul * mat[0, 3];

            mul = -mat[2, 1] / mat[1, 1];
            mat[2, 1] += mul * mat[1, 1];
            mat[2, 2] += mul * mat[1, 2];
            mat[2, 3] += mul * mat[1, 3];

            c = mat[2, 3] / mat[2, 2];
            f = (mat[1, 3] - mat[1, 2] * c) / mat[1, 1];
            g = (mat[0, 3] - mat[0, 2] * c - mat[0, 1] * f) / mat[0, 0];
        }

        private double CircleRadius => Math.Sqrt(g * g + f * f - c);

        private Ellipse DrawEllipse()
        {
            Point center = _points[_points.Count - 1];
            Ellipse ellipse = new()
            {
                Height = 6,
                Width = 6,
                Stroke = Brushes.Green,
                Fill = Brushes.Green,
            };

            Canvas.SetLeft(ellipse, center.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, center.Y - ellipse.Height / 2);

            return ellipse;
        }

        public void DestroyTool()
        {
            _board.MouseDown -= Board_MouseDown;
            _board.MouseUp -= Board_MouseUp;
            _board.MouseMove -= Board_MouseMove;
        }

        public void DoAction(string actName, object value)
        {
            System.Windows.Forms.MessageBox.Show(actName);
        }
    }
}
