using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Helpers;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class HAVATool : ITool
    {
        public const string ToolName = nameof(HAVATool);
        public const string ActionSetOrientation = nameof(ActionSetOrientation);
        public const string ActionSetAngle = nameof(ActionSetAngle);
        public const string ActionSetOrigin = nameof(ActionSetOrigin);
        public const string ActionInitPoints = nameof(ActionInitPoints);
        public const string OrientationHorizontal = "Horizontal Angle";
        public const string OrientationVertical = "Vertical Angle";

        private string _orientation;
        private Canvas _board;
        private List<Point> _points;
        private ContentControl _pointAControl, _pointBControl, _midPointControl, _resultControl;
        private Path _dottedLine, _mainLine;
        private Path _arcPath;
        private PathGeometry _arcGeometry;
        private PathFigure _arcFigure;
        private ArcSegment _arcSegment;
        private StackPanel _resultPanel;
        private TextBlock _rAngle, _rTitle;
        private double _rAngleValue;
        private bool _isFinished;

        public HAVATool(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;
            _board.PreviewKeyUp += Board_KeyUp;

            _orientation = OrientationHorizontal;
            _points = new();
        }

        private void Board_KeyUp(object sender, KeyEventArgs e)
        {
            // Listen for escape key
            if (e.Key == Key.Escape)
            {
                _isFinished = true;
            }
        }

        private void InitElements()
        {
            _arcPath = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            _arcGeometry = new PathGeometry();
            _arcFigure = new PathFigure();
            _arcSegment = new ArcSegment();
            Shape midPoint = DrawEllipse(_points[1]);
            midPoint.Height = 8;
            midPoint.Width = 8;
            _midPointControl = new()
            {
                Height = midPoint.Height,
                Width = midPoint.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = midPoint,
                Focusable = true
            };
            _midPointControl.PreviewMouseMove += MidPoint_PreviewMouseMove;
            _midPointControl.PreviewMouseUp += MidPoint_PreviewMouseUp;
            _midPointControl.PreviewKeyDown += MidPoint_PreviewKeyDown;
            _midPointControl.PreviewKeyUp += MidPointControl_PreviewKeyUp;

            

            _dottedLine = DrawDottedLine();
            _mainLine = DrawLine();

            _rTitle = new()
            {
                Text = _orientation,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };
            _rAngle = new()
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
                Height = 50,
                Width = 160,
                Background = panelBg,
            };
            _resultPanel.Children.Add(_rTitle);
            _resultPanel.Children.Add(_rAngle);

            _resultControl = new()
            {
                Height = _resultPanel.Height,
                Width = _resultPanel.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = _resultPanel,
            };
        }

        private void MidPointControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            UpdateElements();
            UpdateArc();
            e.Handled = true;
        }

        private void MidPoint_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            double angleToChange = 0.1;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                angleToChange = 0.01;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                angleToChange = 1;
            }
            if (e.Key == Key.Right)
            {
                if (_orientation == OrientationHorizontal)
                {
                    double y = _points[1].Y - (_points[2].X - _points[1].X) * Math.Tan((_rAngleValue - angleToChange) * Math.PI / 180.0);
                    _points[2] = new(_points[2].X, y);
                }
                if (_orientation == OrientationVertical)
                {
                    _points[2] = new(_points[2].X + 1, _points[2].Y);
                    _points[1] = new(_points[1].X + 1, _points[1].Y);
                }
            }
            if (e.Key == Key.Left)
            {
                if (_orientation == OrientationHorizontal)
                {
                    double y = _points[1].Y - (_points[2].X - _points[1].X) * Math.Tan((_rAngleValue + angleToChange) * Math.PI / 180.0);
                    _points[2] = new(_points[2].X, y);
                }
                if (_orientation == OrientationVertical)
                {
                    _points[2] = new(_points[2].X - 1, _points[2].Y);
                    _points[1] = new(_points[1].X - 1, _points[1].Y);
                }
            }
            
            if (e.Key == Key.Down)
            {
                if (_orientation == OrientationHorizontal)
                {
                    _points[2] = new(_points[2].X, _points[2].Y + 1);
                    _points[1] = new(_points[1].X, _points[1].Y + 1);
                }
                if (_orientation == OrientationVertical)
                {
                    double x = _points[1].X + (_points[2].Y - _points[1].Y) * Math.Tan((_rAngleValue - angleToChange) * Math.PI / 180.0);
                    _points[2] = new(x, _points[2].Y);
                }
            }
            if (e.Key == Key.Up)
            {
                if (_orientation == OrientationHorizontal)
                {
                    _points[2] = new(_points[2].X, _points[2].Y - 1);
                    _points[1] = new(_points[1].X, _points[1].Y - 1);
                }
                if (_orientation == OrientationVertical)
                {
                    double x = _points[1].X + (_points[2].Y - _points[1].Y) * Math.Tan((_rAngleValue + angleToChange) * Math.PI / 180.0);
                    _points[2] = new(x, _points[2].Y);
                }
            }
            _points[0] = new(2 * _points[1].X - _points[2].X, 2 * _points[1].Y - _points[2].Y);
            _midPointControl.Focus();
            e.Handled = true;
        }

        private void MidPoint_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(_board);
            _points[0] = new(_points[0].X + p.X - _points[1].X, _points[0].Y + p.Y - _points[1].Y);
            _points[2] = new(_points[2].X + p.X - _points[1].X, _points[2].Y + p.Y - _points[1].Y);
            _points[1] = new(p.X, p.Y);
            UpdateElements(true);
            UpdateArc();
        }

        private void MidPoint_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(_board);
                _points[0] = new(_points[0].X + p.X - _points[1].X, _points[0].Y + p.Y - _points[1].Y);
                _points[2] = new(_points[2].X + p.X - _points[1].X, _points[2].Y + p.Y - _points[1].Y);
                _points[1] = new(p.X, p.Y);
                UpdateElements(true);
            }
        }

        private void PointBControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_points.Count < 3)
            {
                return;
            }
            UpdateArc();
            _midPointControl.Focus();
        }

        private void PointAControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_points.Count < 3)
            {
                return;
            }
            UpdateArc();
            _midPointControl.Focus();
        }

        private void PointBControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
           if (e.LeftButton == MouseButtonState.Pressed)
            {
                _points[2] = e.GetPosition(_board);
                _points[1] = new((_points[0].X + _points[2].X) / 2, (_points[0].Y + _points[2].Y) / 2);
                //_points[0] = new(2 * _points[1].X - _points[2].X, 2 * _points[1].Y - _points[2].Y);
                UpdateElements();
            }
        }

        private void PointAControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _points[0] = e.GetPosition(_board);
            if (e.LeftButton == MouseButtonState.Pressed && _points.Count == 3)
            {
                _points[1] = new((_points[0].X + _points[2].X) / 2, (_points[0].Y + _points[2].Y) / 2);
                UpdateElements();
            }
        }

        private void ShowElements()
        {
            _board.Children.Add(_midPointControl);

            _board.Children.Add(_dottedLine);

            _board.Children.Add(_mainLine);

            _board.Children.Add(_arcPath);

            _board.Children.Add(_resultControl);

            _midPointControl.Focus();
        }

        private void UpdateArc()
        {
            if (_points.Count < 3)
            {
                return;
            }
            Vector center = new(_points[1].X, _points[1].Y), a = new(_points[2].X, _points[2].Y), b = new(_points[2].X, _points[1].Y);
            if (_orientation == OrientationVertical)
            {
                b = new(_points[1].X, _points[2].Y);
            }
            Vector ca = a - center, cb = b - center;

            _rAngleValue = Vector.AngleBetween(ca, cb);
            _rAngle.Text = "Angle: " + Math.Round(Math.Abs(_rAngleValue), 2) + " degrees";
            Vector cd = 95 * ca / 100, ce = 95 * cb / 100;
            double radius = Math.Max(cd.Length, ce.Length);

            Vector start = center + radius * ca / ca.Length;
            _arcFigure.StartPoint = new(start.X, start.Y);

            //set end point of arc.
            Vector end = center + radius * cb / cb.Length;
            _arcSegment.Point = new(end.X, end.Y);
            _arcSegment.Size = new Size(radius, radius);
            _arcSegment.SweepDirection = _rAngleValue < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

            _arcFigure.Segments.Add(_arcSegment);
            _arcGeometry.Figures.Add(_arcFigure);
            _arcPath.Data = _arcGeometry;
        }

        private void UpdateElements(bool updateBase = false)
        {
            Canvas.SetLeft(_midPointControl, _points[1].X - _midPointControl.Width / 2);
            Canvas.SetTop(_midPointControl, _points[1].Y - _midPointControl.Height / 2);

            Canvas.SetLeft(_pointAControl, _points[0].X - _pointAControl.Width / 2);
            Canvas.SetTop(_pointAControl, _points[0].Y - _pointAControl.Height / 2);

            Canvas.SetLeft(_pointBControl, _points[2].X - _pointBControl.Width / 2);
            Canvas.SetTop(_pointBControl, _points[2].Y - _pointBControl.Height / 2);

            _rTitle.Text = _orientation;
            if (_orientation == OrientationHorizontal)
            {
                Canvas.SetTop(_resultControl, _points[1].Y - 1.5 * _resultControl.Height);
                Canvas.SetLeft(_resultControl, _points[1].X - _resultControl.Width / 2);
            }
            else if (_orientation == OrientationVertical)
            {
                Canvas.SetLeft(_resultControl, _points[1].X + 0.5 * _resultControl.Width);
                Canvas.SetTop(_resultControl, _points[1].Y - _resultControl.Height / 2);
            }

            Point s = new(_points[0].X, _points[0].Y);
            Point e = new(_points[2].X, _points[2].Y);
            LineGeometry lineGeometry = new()
            {
                StartPoint = s,
                EndPoint = e,
            };
            _mainLine.Data = lineGeometry;
            
                if (_orientation == OrientationHorizontal)
                {
                    lineGeometry = new()
                    {
                        StartPoint = new(0, _points[1].Y),
                        EndPoint = new(_board.ActualWidth, _points[1].Y),
                    };
                }
                else if (_orientation == OrientationVertical)
                {
                    lineGeometry = new()
                    {
                        StartPoint = new(_points[1].X, 0),
                        EndPoint = new(_points[1].X, _board.ActualHeight),
                    };
                }
                _dottedLine.Data = lineGeometry;
        }

        private double GetAngleBetweenThreePoint(Point center, Point a, Point b)
        {
            double angle = 0;
            Vector o = new(center.X, center.Y), p = new(a.X, a.Y), q = new(b.X, b.Y);
            Vector op = p - o, oq = q - o;
            angle = Vector.AngleBetween(op, oq);

            return angle;
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void DrawPointAControl(Point p)
        {
            Shape pointA = DrawEllipse(p);
            _pointAControl = new()
            {
                Height = pointA.Height,
                Width = pointA.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = pointA,
                Tag = 0
            };
            _pointAControl.PreviewMouseMove += PointAControl_PreviewMouseMove;
            _pointAControl.PreviewMouseLeftButtonUp += PointAControl_PreviewMouseLeftButtonUp;

            Canvas.SetLeft(_pointAControl, p.X - _pointAControl.Width / 2);
            Canvas.SetTop(_pointAControl, p.Y - _pointAControl.Height / 2);
            
            _board.Children.Add(_pointAControl);
        }

        private void DrawPointBControl(Point p)
        {
            Shape pointB = DrawEllipse(p);
            _pointBControl = new()
            {
                Width = pointB.Width,
                Height = pointB.Height,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = pointB,
                Tag = 1,
            };
            _pointBControl.PreviewMouseMove += PointBControl_PreviewMouseMove;
            _pointBControl.PreviewMouseLeftButtonUp += PointBControl_PreviewMouseLeftButtonUp;

            Canvas.SetLeft(_pointBControl, p.X - _pointBControl.Width / 2);
            Canvas.SetTop(_pointBControl, p.Y - _pointBControl.Height / 2);

            _board.Children.Add(_pointBControl);
        }

        private void Board_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName || _isFinished)
            {
                e.Handled = true;
                return;
            }
            Point clk = e.GetPosition(_board);
            _points.Add(clk);
            if (_points.Count == 1)
            {
                DrawPointAControl(_points[0]);
            }
            if (_points.Count == 2)
            {
                DrawPointBControl(_points[1]);

                _points.Insert(1, GeometryFunctions.GetMidpointOfTwoPoints(_points[0], _points[1]));
                InitElements();
                UpdateElements();
                UpdateArc();
                ShowElements();
                _isFinished = true;

                // ReInit current class
                Global.CurrentTool = new(_board, ToolName);
                Global.CurrentTool.TheTool.DoAction(ActionSetOrientation, _orientation);
                _board.Focus();
            }

            e.Handled = true;
        }

        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public Path DrawLine()
        {
            Path myPath = new()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            return myPath;
        }

        public Path DrawDottedLine()
        {
            DoubleCollection dc = new();
            dc.Add(2);
            dc.Add(2);
            Path path = DrawLine();
            path.StrokeThickness = 1;
            path.StrokeDashArray = dc;

            return path;
        }

        private Ellipse DrawEllipse(Point center)
        {
            Ellipse ellipse = new()
            {
                Height = 6,
                Width = 6,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
            };

            Canvas.SetLeft(ellipse, (center.X - ellipse.Width / 2));
            Canvas.SetTop(ellipse, (center.Y - ellipse.Height / 2));

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
            switch (actName)
            {
                case ActionSetOrientation:
                    {
                        string val = (string)value;
                        if (val == OrientationHorizontal)
                        {
                            _orientation = OrientationHorizontal;
                        }
                        else if (val == OrientationVertical)
                        {
                            _orientation = OrientationVertical;
                        }

                        break;
                    }

                case ActionSetAngle:
                    {
                        double angle = (double)value;
                        //MessageBox.Show("Angle: " + angle);
                        _points[0] = new(_points[1].X + Math.Abs(_points[0].Y - _points[1].Y) * Math.Tan(angle), _points[0].Y);
                        _points[2] = new(_points[1].X - Math.Abs(_points[2].Y - _points[1].Y) * Math.Tan(angle), _points[2].Y);
                        UpdateElements();
                        UpdateArc();
                        break;
                    }

                case ActionSetOrigin:
                    {
                        Point origin = (Point)value;
                        _points[1] = origin;
                        break;
                    }
                case ActionInitPoints:
                    {
                        List<Point> points = (List<Point>)value;
                        _points = points;

                        DrawPointAControl(_points[0]);
                        DrawPointBControl(_points[2]);

                        InitElements();
                        UpdateElements();
                        UpdateArc();
                        ShowElements();
                        _isFinished = true;
                        break;
                    }
                default:

                    break;
            }
        }
    }
}
