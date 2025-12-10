using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class Protractor : ITool
    {
        public const string ToolName = nameof(Protractor);

        private readonly Canvas _board;
        private List<Point> _points;
        private List<Vector> _vectors;
        private List<ContentControl> _pointControls;
        private Point _dimension;
        private bool _isFinished;

        private StackPanel _resultPanel;
        private TextBlock _rAngle, _rTitle;
        private Path _arcPath;
        private PathGeometry _arcGeometry;
        private PathFigure _arcFigure;
        private ArcSegment _arcSegment;
        private Line _caLine, _cbLine;
        private ContentControl _resultControl;

        public Protractor(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _points = new List<Point>();
            _vectors = new List<Vector>();
            _pointControls = new List<ContentControl>();
            _dimension = new Point()
            {
                X = 6,
                Y = 6,
            };
            _isFinished = false;
            InitElements();
        }

        private void InitElements()
        {
            _caLine = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
            };
            _cbLine = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
            };

            _arcPath = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            _arcGeometry = new PathGeometry();
            _arcFigure = new PathFigure();
            _arcSegment = new ArcSegment();
            Canvas.SetLeft(_arcPath, 0);
            Canvas.SetTop(_arcPath, 0);

            _rTitle = new()
            {
                Text = "Protractor",
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

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {

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
            _vectors.Add(new(clk.X, clk.Y));

            Ellipse ellipse = DrawEllipse();
            ContentControl contentControl = new()
            {
                Height = ellipse.Height,
                Width = ellipse.Width,
                Template = (ControlTemplate)_board.TryFindResource("MovableItemTemplate"),
                Content = ellipse,
                Tag = _points.Count - 1,
            };



            Canvas.SetTop(contentControl, clk.Y - contentControl.Height / 2);
            Canvas.SetLeft(contentControl, clk.X - contentControl.Width / 2);
            contentControl.PreviewMouseUp += ContentControl_PreviewMouseUp;
            contentControl.PreviewMouseMove += ContentControl_PreviewMouseMove;
            _pointControls.Add(contentControl);

            _board.Children.Add(contentControl);

            if (_points.Count >= 3)
            {
                UpdateElements();
                UpdateArc();
                ShowElements();
                _isFinished = true;
            }

            e.Handled = true;
        }

        private void ContentControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(_board);

            int idx = int.Parse((sender as ContentControl).Tag.ToString());
            if (idx == 1 && _vectors.Count == 3)
            {
                _points[0] = new(_points[0].X + p.X - _points[1].X, _points[0].Y + p.Y - _points[1].Y);
                _vectors[0] = new(_points[0].X, _points[0].Y);
                _points[2] = new(_points[2].X + p.X - _points[1].X, _points[2].Y + p.Y - _points[1].Y);
                _vectors[2] = new(_points[2].X, _points[2].Y);
            }
            _points[idx] = p;
            _vectors[idx] = new(p.X, p.Y);

            UpdateElements();
            UpdateArc();
        }

        private void ContentControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(_board);

                int idx = int.Parse((sender as ContentControl).Tag.ToString());
                if (idx == 1 && _vectors.Count == 3)
                {
                    _points[0] = new(_points[0].X + p.X - _points[1].X, _points[0].Y + p.Y - _points[1].Y);
                    _vectors[0] = new(_points[0].X, _points[0].Y);
                    _points[2] = new(_points[2].X + p.X - _points[1].X, _points[2].Y + p.Y - _points[1].Y);
                    _vectors[2] = new(_points[2].X, _points[2].Y);
                }
                _points[idx] = p;
                _vectors[idx] = new(p.X, p.Y);

                UpdateElements();
            }
        }

        private void ShowElements()
        {
            _board.Children.Add(_caLine);
            _board.Children.Add(_cbLine);
            _board.Children.Add(_arcPath);

            _board.Children.Add(_resultControl);
        }

        private void UpdateArc()
        {
            if (_vectors.Count < 3) return;
            Vector center = _vectors[1], a = _vectors[0], b = _vectors[2];
            Vector ca = a - center, cb = b - center;

            double angle = Vector.AngleBetween(ca, cb);

            _rAngle.Text = "Angle: " + Math.Round(Math.Abs(angle), 2) + " degrees";

            Vector cd = ca / 5, ce = cb / 5;
            double radius = Math.Max(cd.Length, ce.Length);

            Vector start = center + radius * ca / ca.Length;
            _arcFigure.StartPoint = new(start.X, start.Y);

            //set end point of arc.
            Vector end = center + radius * cb / cb.Length;
            _arcSegment.Point = new(end.X, end.Y);
            _arcSegment.Size = new Size(radius, radius);
            _arcSegment.SweepDirection = angle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

            _arcFigure.Segments.Add(_arcSegment);
            _arcGeometry.Figures.Add(_arcFigure);
            _arcPath.Data = _arcGeometry;
        }

        public void UpdateElements()
        {
            if (_vectors.Count < 3) return;
            Vector center = _vectors[1], a = _vectors[0], b = _vectors[2];

            _caLine.X1 = center.X; _caLine.Y1 = center.Y; _caLine.X2 = a.X; _caLine.Y2 = a.Y;
            _cbLine.X1 = center.X; _cbLine.Y1 = center.Y; _cbLine.X2 = b.X; _cbLine.Y2 = b.Y;

            Canvas.SetTop(_pointControls[0], _points[0].Y - _pointControls[0].Height / 2);
            Canvas.SetLeft(_pointControls[0], _points[0].X - _pointControls[0].Width / 2);

            Canvas.SetTop(_pointControls[2], _points[2].Y - _pointControls[2].Height / 2);
            Canvas.SetLeft(_pointControls[2], _points[2].X - _pointControls[2].Width / 2);

            Canvas.SetTop(_resultControl, _points[2].Y - _resultControl.Height / 2);
            Canvas.SetLeft(_resultControl, _points[2].X + 8);
        }

        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {

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
            System.Windows.Forms.MessageBox.Show(actName);
        }
    }
}
