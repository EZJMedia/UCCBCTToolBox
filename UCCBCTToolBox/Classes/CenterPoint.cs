using Point = System.Windows.Point;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.Generic;
using UCCBCTToolBox.Interfaces;
using UCCBCTToolBox.Views;
using System;

namespace UCCBCTToolBox.Classes
{
    internal class CenterPoint : ITool
    {
        public const string ToolName = "CenterPoint";
        public const string ActionSetSettings = nameof(ActionSetSettings);

        private Canvas _board;
        private List<ContentControl> _contentControlDots;
        private List<Point> _points;
        private List<Path> _crosses;
        private Point _dimension;
        private string _dotColor, _crossColor;

        public CenterPoint(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _points = new List<Point>();
            _contentControlDots = new List<ContentControl>();
            _crosses = new List<Path>();

            _dimension = new()
            {
                X = 6,
                Y = 6,
            };

            _dotColor = "Red";
            _crossColor = "Blue";
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void Board_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Global.CurrentTool.Name);
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
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

            if (_points.Count % 2 == 0)
            {
                Path path = DrawPath();
                _crosses.Add(path);

                _board.Children.Add(_crosses[_crosses.Count - 1]);
            }

            e.Handled = true;
        }

        private void ContentControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Global.CurrentTool.Name);
            Point up = e.GetPosition(_board);

            int idx = int.Parse((sender as ContentControl).Tag.ToString());

            int cdx = idx / 2;
            int adder = cdx * 2;

            _points[adder + (idx % 2)] = up;

            if (cdx < _crosses.Count)
            {
                Point p = _points[adder + (idx % 2)];
                Point q = _points[adder + ((idx + 1) % 2)];
                Point _pathCenter = new Point((p.X + q.X) / 2, (p.Y + q.Y) / 2);

                Canvas.SetLeft(_crosses[cdx], (_pathCenter.X - _crosses[cdx].Width / 2));
                Canvas.SetTop(_crosses[cdx], (_pathCenter.Y - _crosses[cdx].Height / 2));
            }

            e.Handled = true;
        }

        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private Path DrawPath()
        {
            Point p = _points[_points.Count - 1];
            Point q = _points[_points.Count - 2];
            Point _pathCenter = new Point((p.X + q.X) / 2, (p.Y + q.Y) / 2);

            string geometry = "M0, 0L" + _dimension.X + ", " + _dimension.X + "M" +
                _dimension.X + ", 0L0, " + _dimension.X;
            Path path = new()
            {
                Data = Geometry.Parse(geometry),
                Stroke = new BrushConverter().ConvertFromString(_crossColor) as SolidColorBrush,
                Width = _dimension.X,
                Height = _dimension.X,
            };
            Canvas.SetLeft(path, (_pathCenter.X - path.Width / 2));
            Canvas.SetTop(path, (_pathCenter.Y - path.Height / 2));

            return path;
        }

        private Ellipse DrawEllipse()
        {
            Point _ellipseCenter = _points[_points.Count - 1];
            Ellipse ellipse = new()
            {
                Height = _dimension.Y,
                Width = _dimension.X,
                Stroke = new BrushConverter().ConvertFromString(_dotColor) as SolidColorBrush,
                Fill = new BrushConverter().ConvertFromString(_dotColor) as SolidColorBrush,
            };

            Canvas.SetLeft(ellipse, (_ellipseCenter.X - ellipse.Width / 2));
            Canvas.SetTop(ellipse, (_ellipseCenter.Y - ellipse.Height / 2));

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
                case ActionSetSettings:
                    ColorPickerDialog(value);
                    break;
                case Tool.ActionUndo:
                    DoUndo();
                    break;
                default:
                    break;
            }
        }

        private void DoUndo()
        {
            if (_points.Count <= 0)
            {
                return;
            }
            if (_points.Count > 0 && _points.Count % 2 != 0)
            {
                _board.Children.RemoveAt(_board.Children.Count - 1);
            }
            else if (_points.Count > 0)
            {
                _board.Children.RemoveAt(_board.Children.Count - 1);
                _board.Children.RemoveAt(_board.Children.Count - 1);
                _crosses.RemoveAt(_crosses.Count - 1);
            }
            _points.RemoveAt(_points.Count - 1);
        }

        private void ColorPickerDialog(object owner)
        {
            var dialog = new CenterPointSettingsDialog(_dotColor, _crossColor)
            {
                Owner = (MainWindow)owner
            };

            if (dialog.ShowDialog() == true)
            {
                _dotColor = dialog.DotColor;
                _crossColor = dialog.CrossColor;
            }
        }
    }
}
