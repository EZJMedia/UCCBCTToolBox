using Point = System.Windows.Point;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.Generic;
using UCCBCTToolBox.Interfaces;
using System;

namespace UCCBCTToolBox.Classes
{
    internal class StraightLine : ITool
    {
        public const string ToolName = nameof(StraightLine);

        private Canvas _board;
        private List<Point> _points;
        private List<ContentControl> _controlDots;
        private List<Path> _lines;
        private Point _dimension;

        public StraightLine(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _points = new List<Point>();
            _controlDots = new List<ContentControl>();
            _lines = new List<Path>();

            _dimension = new()
            {
                X = 6,
                Y = 6,
            };
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed && lineMoving != "")
            {
                up = e.GetPosition(_board);

                LineMoved();

                down = up;
            }
        }

        private void Board_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Global.CurrentTool.Name);
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
            if (lineMoving != "")
            {
                up = e.GetPosition(_board);

                LineMoved();

                down = up;
                lineMoving = "";
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
                Tag = _controlDots.Count
            };



            Canvas.SetTop(contentControl, clk.Y - ellipse.Height / 2);
            Canvas.SetLeft(contentControl, clk.X - ellipse.Width / 2);
            contentControl.PreviewMouseMove += ContentControl_PreviewMouseMove;
            _controlDots.Add(contentControl);
            _board.Children.Add(contentControl);

            if (_points.Count % 2 == 0)
            {
                Path path = DrawPath();
                path.Tag = _lines.Count;
                path.PreviewMouseDown += Path_PreviewMouseDown;
                path.PreviewMouseMove += Path_PreviewMouseMove;
                path.PreviewMouseUp += Path_PreviewMouseUp;
                _lines.Add(path);

                _board.Children.Add(_lines[_lines.Count - 1]);
            }

            e.Handled = true;
        }

        private Point up, down;
        string lineMoving = "";
        private void Path_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }

            up = e.GetPosition(_board);

            LineMoved();

            down = up;
            lineMoving = "";
            e.Handled = true;
        }

        private void LineMoved()
        {
            int l = int.Parse(lineMoving);
            _points[2 * l] += up - down;
            _points[2 * l + 1] += up - down;

            LineGeometry line = new()
            {
                StartPoint = _points[2*l],
                EndPoint = _points[2*l+1],
            };
            _lines[l].Data = line;


            Canvas.SetLeft(_controlDots[2*l], _points[2 * l].X - _controlDots[2 * l].Width / 2);
            Canvas.SetTop(_controlDots[2 * l], _points[2 * l].Y - _controlDots[2*l].Height / 2);
            Canvas.SetLeft(_controlDots[2*l + 1], _points[2 * l + 1].X - _controlDots[2 * l + 1].Width / 2);
            Canvas.SetTop(_controlDots[2 * l + 1], _points[2 * l + 1].Y - _controlDots[2*l + 1].Height / 2);
        }

        private void Path_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                up = e.GetPosition(_board);

                LineMoved();

                down = up;
            }
        }

        private void Path_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            down = e.GetPosition(_board);
            lineMoving = ((Path)sender).Tag.ToString();
        }

        private void ContentControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point up = e.GetPosition(_board);
                ContentControl control = (ContentControl)sender;

                int idx = int.Parse(control.Tag.ToString());

                int cdx = idx / 2;
                int adder = cdx * 2;

                _points[adder + (idx % 2)] = up;

                if (cdx < _lines.Count)
                {
                    Point p = _points[adder + (idx % 2)];
                    Point q = _points[adder + ((idx + 1) % 2)];

                    LineGeometry line = new()
                    {
                        StartPoint = p,
                        EndPoint = q,
                    };
                    _lines[cdx].Data = line;
                }
                Canvas.SetLeft(control, up.X - control.Width / 2);
                Canvas.SetTop(control, up.Y - control.Height / 2);
            }
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

            LineGeometry line = new()
            {
                StartPoint = p,
                EndPoint = q,
            };
            Path path = new()
            {
                Data = line,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Cursor = Cursors.SizeAll,
            };

            return path;
        }

        private Ellipse DrawEllipse()
        {
            Point _ellipseCenter = _points[_points.Count - 1];
            Ellipse ellipse = new()
            {
                Height = _dimension.Y,
                Width = _dimension.X,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
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

            _points.RemoveAt(_points.Count - 1);
            _points.RemoveAt(_points.Count - 1);
            _controlDots.RemoveAt(_controlDots.Count - 1);
            _controlDots.RemoveAt(_controlDots.Count - 1);
            _lines.RemoveAt(_lines.Count - 1);
            _board.Children.RemoveAt(_board.Children.Count - 1);
            _board.Children.RemoveAt(_board.Children.Count - 1);
            _board.Children.RemoveAt(_board.Children.Count - 1);
        }
    }
}
