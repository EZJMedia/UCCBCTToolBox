using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;
using UCCBCTToolBox.ViewModels;
using UCCBCTToolBox.Views;
using MessageBox = System.Windows.Forms.MessageBox;

namespace UCCBCTToolBox.Classes
{
    internal class CephaloTool : ITool
    {
        public const string ToolName = nameof(CephaloTool);
        private readonly Canvas _board;
        private ContentControl _gridControl, _ellipseControl, _rotateEllipseControl;
        private Line _rotateLine;
        private Vector _rotateStart, _center;

        public CephaloTool(Canvas canvas)
        {
            _board = canvas;

            Shape grid = DrawShape();

            _gridControl = new()
            {
                MinWidth = 50,
                MinHeight = 50,
                Height = 400,
                Width = 400,
                Template = (ControlTemplate)_board.TryFindResource("FourCornerResizeTemplate"),
                Content = grid,
                Focusable = true,
                RenderTransform = new RotateTransform(0, 200, 200),
            };
            _gridControl.SizeChanged += Grid_SizeChanged;
            _gridControl.KeyDown += Grid_KeyDown;

            Canvas.SetTop(_gridControl, (_board.ActualHeight / 2) - (_gridControl.Height / 2));
            Canvas.SetLeft(_gridControl, (_board.ActualWidth / 2) - (_gridControl.Width / 2));

            _board.Children.Add(_gridControl);

            Ellipse ellipse = new()
            {
                Height = 10,
                Width = 10,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
            };
            _ellipseControl = new()
            {
                Height = 2 * ellipse.Height,
                Width = 2 * ellipse.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = ellipse,
            };
            _ellipseControl.PreviewMouseMove += EllipseControl_PreviewMouseMove;
            _ellipseControl.PreviewMouseLeftButtonUp += EllipseControl_PreviewMouseLeftButtonUp;
            _ellipseControl.PreviewMouseLeftButtonDown += EllipseControl_PreviewMouseLeftButtonDown;

            _center = new()
            {
                X = (_board.ActualWidth / 2) - (_ellipseControl.Width / 2),
                Y = (_board.ActualHeight / 2) - (_ellipseControl.Height / 2),
            };
            Canvas.SetTop(_ellipseControl, _center.X);
            Canvas.SetLeft(_ellipseControl, _center.Y);

            _board.Children.Add(_ellipseControl);

            Ellipse rotateEllipse = new()
            {
                Height = 10,
                Width = 10,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
            };
            _rotateEllipseControl = new()
            {
                Height = 2 * rotateEllipse.Height,
                Width = 2 * rotateEllipse.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = rotateEllipse,
            };
            _rotateEllipseControl.PreviewMouseMove += RotateEllipseControl_PreviewMouseMove;
            _rotateEllipseControl.PreviewMouseLeftButtonUp += RotateEllipseControl_PreviewMouseLeftButtonUp;

            _rotateStart.X = (_board.ActualWidth / 2) - (_rotateEllipseControl.Width / 2);
            _rotateStart.Y = (_board.ActualHeight / 2) - _gridControl.Height / 2 - 50 - (_rotateEllipseControl.Height / 2);
            Canvas.SetTop(_rotateEllipseControl, _rotateStart.Y);
            Canvas.SetLeft(_rotateEllipseControl, _rotateStart.X);

            _board.Children.Add(_rotateEllipseControl);

            _rotateLine = new()
            {
                X1 = _rotateStart.X + _rotateEllipseControl.Width / 2,
                Y1 = _rotateStart.Y + _rotateEllipseControl.Height / 2,
                X2 = _center.X + _ellipseControl.Width / 2,
                Y2 = _center.Y + _ellipseControl.Height / 2,
                StrokeThickness = 1,
                Stroke = Brushes.Red
            };
            _board.Children.Add(_rotateLine);

            _gridControl.Focus();
        }

        private Point _ellipseMouseDown;
        private void EllipseControl_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _ellipseMouseDown = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
        }

        private void RotateEllipseControl_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RotateGridControl();
        }

        private void RotateEllipseControl_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                RotateGridControl();
            }
        }

        private void RotateGridControl()
        {
            Vector rotateEnd = new()
            {
                X = Canvas.GetLeft(_rotateEllipseControl),
                Y = Canvas.GetTop(_rotateEllipseControl)
            };

            Vector center = new()
            {
                X = Canvas.GetLeft(_ellipseControl),
                Y = Canvas.GetTop(_ellipseControl)
            };

            _rotateLine.X1 = rotateEnd.X + _rotateEllipseControl.Width / 2;
            _rotateLine.Y1 = rotateEnd.Y + _rotateEllipseControl.Height / 2;
            _rotateLine.X2 = center.X + _ellipseControl.Width / 2;
            _rotateLine.Y2 = center.Y + _ellipseControl.Height / 2;

            Vector line1 = rotateEnd - center, line2 = _rotateStart - center;

            double angle = Vector.AngleBetween(line2, line1);
            RotateTransform rotateTransform = (RotateTransform)_gridControl.RenderTransform;
            rotateTransform.Angle += angle;

            _rotateStart = rotateEnd;
        }

        private void UpdateGridControlPosition()
        {
            Vector p = new(_ellipseMouseDown.X, _ellipseMouseDown.Y);
            Canvas.SetTop(_gridControl, Canvas.GetTop(_ellipseControl) + _ellipseControl.Height / 2 - _gridControl.Height / 2);
            Canvas.SetLeft(_gridControl, Canvas.GetLeft(_ellipseControl) + _ellipseControl.Width / 2 - _gridControl.Width / 2);

            Vector q = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
            Vector r = new(Canvas.GetLeft(_rotateEllipseControl), Canvas.GetTop(_rotateEllipseControl));
            Vector v = new(r.X + q.X - p.X, r.Y + q.Y - p.Y);

            if (v.X < 0 || v.Y < 0)
                return;
            Canvas.SetTop(_rotateEllipseControl, v.Y);
            Canvas.SetLeft(_rotateEllipseControl, v.X);

            _rotateLine.X1 = v.X + _rotateEllipseControl.Width / 2;
            _rotateLine.Y1 = v.Y + _rotateEllipseControl.Height / 2;
            _rotateLine.X2 = q.X + _ellipseControl.Width / 2;
            _rotateLine.Y2 = q.Y + _ellipseControl.Height / 2;
            _rotateStart = v;
            _ellipseMouseDown = new(q.X, q.Y);
        }

        private void UpdateEllipseControlPosition()
        {
            Vector p = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
            Canvas.SetTop(_ellipseControl, Canvas.GetTop(_gridControl) + _gridControl.Height / 2 - (_ellipseControl.Height / 2));
            Canvas.SetLeft(_ellipseControl, Canvas.GetLeft(_gridControl) + _gridControl.Width / 2 - (_ellipseControl.Width / 2));

            Vector q = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
            Vector r = new(Canvas.GetLeft(_rotateEllipseControl), Canvas.GetTop(_rotateEllipseControl));
            Vector v = new(r.X + q.X - p.X, r.Y + q.Y - p.Y);

            if (v.X < 0 || v.Y < 0)
                return;
            Canvas.SetTop(_rotateEllipseControl, v.Y);
            Canvas.SetLeft(_rotateEllipseControl, v.X);

            _rotateLine.X1 = v.X + _rotateEllipseControl.Width / 2;
            _rotateLine.Y1 = v.Y + _rotateEllipseControl.Height / 2;
            _rotateLine.X2 = q.X + _ellipseControl.Width / 2;
            _rotateLine.Y2 = q.Y + _ellipseControl.Height / 2;
            _rotateStart = v;
        }

        private void EllipseControl_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateGridControlPosition();
        }

        private void EllipseControl_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                UpdateGridControlPosition();
            }
        }

        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ContentControl control = (ContentControl)sender;
            double angleShift = 0.1; // degree
            double translate = 1.0;
            double scale = 1.0;

            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    Canvas.SetTop(control, Canvas.GetTop(control) - translate);
                    UpdateEllipseControlPosition();
                    break;
                case System.Windows.Input.Key.Right:
                    Canvas.SetLeft(control, Canvas.GetLeft(control) + translate);
                    UpdateEllipseControlPosition();
                    break;
                case System.Windows.Input.Key.Down:
                    Canvas.SetTop(control, Canvas.GetTop(control) + translate);
                    UpdateEllipseControlPosition();
                    break;
                case System.Windows.Input.Key.Left:
                    Canvas.SetLeft(control, Canvas.GetLeft(control) - translate);
                    UpdateEllipseControlPosition();
                    break;
                case System.Windows.Input.Key.Add:
                case System.Windows.Input.Key.OemPlus:
                    control.Width += 1;
                    control.Height += 1;
                    Canvas.SetTop(control, Canvas.GetTop(control) - scale / 2);
                    Canvas.SetLeft(control, Canvas.GetLeft(control) - scale / 2);
                    break;
                case System.Windows.Input.Key.Subtract:
                case System.Windows.Input.Key.OemMinus:
                    control.Width -= 1;
                    control.Height -= 1;
                    Canvas.SetTop(control, Canvas.GetTop(control) + scale / 2);
                    Canvas.SetLeft(control, Canvas.GetLeft(control) + scale / 2);
                    break;
                case System.Windows.Input.Key.OemPeriod:
                    {
                        RotateTransform rotateTransform = (RotateTransform)_gridControl.RenderTransform;
                        Vector p = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
                        Vector q = new(Canvas.GetLeft(_rotateEllipseControl), Canvas.GetTop(_rotateEllipseControl));
                        double r = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
                        Canvas.SetLeft(_rotateEllipseControl, p.X + r * Math.Cos((270 + rotateTransform.Angle + angleShift) * Math.PI / 180));
                        Canvas.SetTop(_rotateEllipseControl, p.Y + r * Math.Sin((270 + rotateTransform.Angle + angleShift) * Math.PI / 180));
                        RotateGridControl();
                        break;
                    }

                case System.Windows.Input.Key.OemComma:
                    {
                        RotateTransform rotateTransform = (RotateTransform)_gridControl.RenderTransform;
                        Vector p = new(Canvas.GetLeft(_ellipseControl), Canvas.GetTop(_ellipseControl));
                        Vector q = new(Canvas.GetLeft(_rotateEllipseControl), Canvas.GetTop(_rotateEllipseControl));
                        double r = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
                        Canvas.SetLeft(_rotateEllipseControl, p.X + r * Math.Cos((270 + rotateTransform.Angle - angleShift) * Math.PI / 180));
                        Canvas.SetTop(_rotateEllipseControl, p.Y + r * Math.Sin((270 + rotateTransform.Angle - angleShift) * Math.PI / 180));
                        RotateGridControl();
                        break;
                    }
                case System.Windows.Input.Key.Enter:
                    {
                        RotateTransform rotateTransform = (RotateTransform)control.RenderTransform;
                        double radian = rotateTransform.Angle * Math.PI / 180.0;

                        MainCalcsPanelViewModel.HTValue = rotateTransform.Angle;

                        Global.CurrentTool = new Tool(_board, HAVATool.ToolName);
                        Global.CurrentTool.TheTool.DoAction(HAVATool.ActionSetOrientation, HAVATool.OrientationVertical);
                        Point start = new Point(_board.Width / 2, 20);
                        Point centre = new Point(Canvas.GetLeft(_ellipseControl) + _ellipseControl.Width / 2, Canvas.GetTop(_ellipseControl) + _ellipseControl.Height / 2);
                        Point end = new Point(_board.Width / 2, _board.Height - 120);
                        Global.CurrentTool.TheTool.DoAction(HAVATool.ActionInitPoints, new List<Point> { start, centre, end });
                        Global.CurrentTool.TheTool.DoAction(HAVATool.ActionSetOrigin, centre);
                        Global.CurrentTool.TheTool.DoAction(HAVATool.ActionSetAngle, radian);
                        break;
                    }
                default:
                    break;
            }


            if (control != null && !control.IsFocused)
            {
                control.Focus();
            }
            e.Handled = true;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ContentControl control = (ContentControl)sender;
            RotateTransform rotateTransform = (RotateTransform)control.RenderTransform;
            rotateTransform.CenterX = e.NewSize.Width / 2;
            rotateTransform.CenterY = e.NewSize.Height / 2;

            Polygon polygon = (Polygon)control.Content;
            polygon.Points.Clear();
            polygon.Points = getPointCollection(new(0, 0), new(e.NewSize.Width, e.NewSize.Height));

            UpdateEllipseControlPosition();
            control.Focus();
        }

        private PointCollection getPointCollection(Point start, Point end)
        {
            PointCollection points = new()
            {
                start,
                new(end.X, start.Y)
            };
            int n = 20, m = 20;
            for (int i = 0; i < n; i++)
            {
                Point last = points[^1];
                Point below = new(last.X, last.Y + (end.Y - start.Y) / n);
                points.Add(below);
                if (i % 2 == 0)
                {
                    points.Add(new(start.X, below.Y));
                }
                else
                {
                    points.Add(new(end.X, below.Y));
                }
            }
            if (n % 2 == 0)
            {
                points.Add(new(points[^1].X, start.Y));
                points.Add(new(start.X, points[^1].Y));
            }
            else
            {
                points.Add(new(points[^1].X, start.Y));
            }
            
            points.Add(new(start.X, end.Y));
            for (int i = 0; i < m; i++)
            {
                Point last = points[^1];
                Point right = new(last.X + (end.X - start.X) / m, last.Y);
                points.Add(right);
                if (i % 2 == 0)
                {
                    points.Add(new(right.X, start.Y));
                }
                else
                {
                    points.Add(new(right.X, end.Y));
                }
            }
            if (m % 2 == 0)
            {
                points.Add(new(start.X, points[^1].Y));
                points.Add(new(points[^1].X, start.Y));
            }
            else
            {
                points.Add(new(start.X, points[^1].Y));
            }

            return points;
        }

        public Shape DrawShape()
        {
            Point start = new(0, 0);
            Point end = new(400, 400);


            Polygon myPath = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Points = getPointCollection(start, end)
            };

            return myPath;
        }

        public void DestroyTool()
        {
            _board.Children.Remove(_rotateEllipseControl);
            _board.Children.Remove(_rotateLine);
            _board.Children.Remove(_ellipseControl);
            _board.Children.Remove(_gridControl);
        }

        public void DoAction(string actName, object value)
        {

        }
    }
}
