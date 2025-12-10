using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Helpers;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class MisalignmentTool : ITool
    {
        public const string ToolName = nameof(MisalignmentTool);
        public const string ActionSetHeadTilt = nameof(ActionSetHeadTilt);
        public const string ActionSetSpineTilt = nameof(ActionSetSpineTilt);
        public const string ActionSetFA = nameof(ActionSetFA);
        public const string ActionSetHTRight = nameof(ActionSetHTRight);
        public const string ActionSetSTRight = nameof(ActionSetSTRight);
        public const string ActionSetFACRight = nameof(ActionSetFACRight);
        public const string ActionSetFAHigh = nameof(ActionSetFAHigh);
        public const string ActionDrawMisalignment = nameof(ActionDrawMisalignment);

        private readonly Canvas _board;
        private double _headTilt, _spineTilt, _fa;
        private bool _htRight, _stRight, _faHigh, _facRight, _rightReversed;
        private Point _hA, _hB, _vT, _vB, _rA, _rB, _cR, _rVT, _cVT, _rVB, _cVB, _cRVB, _mVB; // Horizontal A, B, Vertical Top, Bottom
        private Path _hAB, _vTop, _vBottom, _refAB, _refTop, _refBottom;
        private TextBlock _leftRightBlock;
        private string lineMoving = "";
        private Shape _refCenter;

        public MisalignmentTool(Canvas canvas)
        {
            _board = canvas;
            _board.PreviewMouseUp += Board_PreviewMouseUp;
            _board.PreviewMouseMove += Board_PreviewMouseMove;

            _rA = new(100, _board.ActualHeight / 2);
            _rB = new(_board.ActualWidth - 100, _board.ActualHeight / 2);
            _cR = new((_rA.X + _rB.X) / 2, (_rA.Y + _rB.Y) / 2);
            _rVT = new(_cR.X, 100);
            _cVT = new(_cR.X, _cR.Y);
            _rVB = new(_cR.X, _board.ActualHeight - 100);
            _cVB = new(_cR.X, _cR.Y);
            _cRVB = _cVB;

            _hA = new(_board.ActualWidth / 2 - 1000, _board.ActualHeight / 2);
            _hB = new(_board.ActualWidth / 2 + 1000, _board.ActualHeight / 2);
            _vT = new(_board.ActualWidth / 2, _board.ActualHeight / 2 - 1000);
            _vB = new(_board.ActualWidth / 2, _board.ActualHeight / 2 + 1000);

            InitElements();
        }

        private void Board_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed && lineMoving == nameof(_hAB))
            {
                up = e.GetPosition(_board);

                HALineMoved();

                down = up;
            }
            if (e.LeftButton == MouseButtonState.Pressed && lineMoving == nameof(_vTop))
            {
                up = e.GetPosition(_board);

                VTopLineMoved();
                
                down = up;
            }
            if (e.LeftButton == MouseButtonState.Pressed && lineMoving == nameof(_vBottom))
            {
                up = e.GetPosition(_board);

                VBottomLineMoved();

                down = up;
            }
        }

        private void HALineMoved()
        {
            _hA = new(_hA.X + up.X - down.X, _hA.Y + up.Y - down.Y);
            _rA = new(_rA.X + up.X - down.X, _rA.Y + up.Y - down.Y);
            _rB = new(_rB.X + up.X - down.X, _rB.Y + up.Y - down.Y);

            _hB = new(_hB.X + up.X - down.X, _hB.Y + up.Y - down.Y);
            _cR = new((_rA.X + _rB.X) / 2, (_rA.Y + _rB.Y) / 2);

            _cVT = GeometryFunctions.GetIntersectionOfTwoLine(_hA, _hB, _vT, _cVT);
            Point cvb = GeometryFunctions.GetIntersectionOfTwoLine(_hA, _hB, _vB, _cVB);
            _cRVB = new(_cRVB.X + cvb.X - _cVB.X, _cRVB.Y + cvb.Y - _cVB.Y);
            
            _cVB = cvb;

            //UpdateCalculations();

            UpdateElements();
        }

        private void VTopLineMoved()
        {
            _cVT = new(_cVT.X + up.X - down.X, _cVT.Y + up.Y - down.Y);
            _vT = new(_vT.X + up.X - down.X, _vT.Y + up.Y - down.Y);
            _rVT = new(_cVT.X, _vT.Y);
            _cVT = GeometryFunctions.GetIntersectionOfTwoLine(_hA, _hB, _vT, _cVT);

            UpdateVerticalTopLine();
        }

        

        private void VBottomLineMoved()
        {
            _cRVB = up;
            _cVB = new(_cVB.X + up.X - down.X, _cVB.Y + up.Y - down.Y);
            _vB = new(_vB.X + up.X - down.X, _vB.Y + up.Y - down.Y);
            _rVB = new(_cVB.X, _vB.Y);
            _cVB = GeometryFunctions.GetIntersectionOfTwoLine(_hA, _hB, _vB, _cVB);
            _mVB = up;

            UpdateVerticalBottomLine();
        }

        private void Board_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (lineMoving == nameof(_hAB))
            {
                up = e.GetPosition(_board);

                HALineMoved();

                down = up;
            }
            if (lineMoving == nameof(_vTop))
            {
                up = e.GetPosition(_board);

                VTopLineMoved();
                
                down = up;
            }
            if (lineMoving == nameof(_vBottom))
            {
                up = e.GetPosition(_board);

                VBottomLineMoved();
                
                down = up;
            }
            lineMoving = "";
        }

        private void InitElements()
        {
            _refCenter = DrawEllipse();
            _hAB = DrawLine();
            _vTop = DrawLine();
            _vBottom = DrawLine();
            _refAB = DrawLine(0.5);
            _refTop = DrawLine(0.5);
            _refBottom = DrawLine();

            _hAB.Cursor = Cursors.SizeAll;
            _hAB.PreviewMouseDown += HAB_PreviewMouseDown;
            _hAB.PreviewMouseUp += HAB_PreviewMouseUp;
            _hAB.PreviewMouseMove += HAB_PreviewMouseMove;

            _vTop.Cursor = Cursors.SizeAll;
            _vTop.PreviewMouseDown += VTop_PreviewMouseDown;
            _vTop.PreviewMouseUp += VTop_PreviewMouseUp;
            _vTop.PreviewMouseMove += VTop_PreviewMouseMove;

            _vBottom.Cursor = Cursors.SizeAll;
            _vBottom.PreviewMouseDown += VBottom_PreviewMouseDown;
            _vBottom.PreviewMouseUp += VBottom_PreviewMouseUp;
            _vBottom.PreviewMouseMove += VBottom_PreviewMouseMove;

            _leftRightBlock = new()
            {
                Text = "L/R",
                FontSize = 60,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new(0, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            _leftRightBlock.PreviewMouseLeftButtonUp += LeftRightBlock_PreviewMouseLeftButtonUp;
        }

        private void LeftRightBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            _rightReversed = !_rightReversed;
            if (_rightReversed)
            {
                _leftRightBlock.Text = "R/L";
            }
            else
            {
                _leftRightBlock.Text = "L/R";
            }
            UpdateCalculations();
            Point intersection = GeometryFunctions.GetIntersectionOfTwoLine(_vB, _cVB, _cRVB, new(_cRVB.X + 10, _cRVB.Y));
            _cRVB = intersection;

            HALineMoved();
        }

        private void VBottom_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed && lineMoving == nameof(_vBottom))
            {
                up = e.GetPosition(_board);

                VBottomLineMoved();
                
                down = up;
            }
        }

        private void VBottom_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (lineMoving == nameof(_vBottom))
            {
                up = e.GetPosition(_board);

                VBottomLineMoved();
                
                down = up;
            }
            lineMoving = "";
        }

        private void VBottom_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            down = e.GetPosition(_board);
            lineMoving = nameof(_vBottom);
        }

        private void VTop_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            up = e.GetPosition(_board);

            VTopLineMoved();
            
            down = up;
            lineMoving = "";
        }

        private void VTop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            down = e.GetPosition(_board);
            lineMoving = nameof(_vTop);
        }

        private void VTop_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                up = e.GetPosition(_board);

                VTopLineMoved();
                
                down = up;
            }
        }

        private void HAB_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                up = e.GetPosition(_board);

                HALineMoved();

                down = up;
            }
        }

        Point up, down;
        private void HAB_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            up = e.GetPosition(_board);

            HALineMoved();

            down = up;
            lineMoving = "";
        }

        private void HAB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                return;
            }
            down = e.GetPosition(_board);
            lineMoving = nameof(_hAB);
        }

        private void UpdateHorizontalLine()
        {
            LineGeometry refABLineGeometry = new()
            {
                StartPoint = _rA,
                EndPoint = _rB,
            };

            _refAB.Data = refABLineGeometry;
            
            LineGeometry hABLineGeometry = new()
            {
                StartPoint = _hA,
                EndPoint = _hB,
            };

            _hAB.Data = hABLineGeometry;
        }

        private void UpdateVerticalTopLine()
        {
            LineGeometry refTopLineGeometry = new()
            {
                StartPoint = _cVT,
                EndPoint = _rVT,
            };

            _refTop.Data = refTopLineGeometry;
            
            LineGeometry vTopLineGeometry = new()
            {
                StartPoint = _cVT,
                EndPoint = _vT,
            };

            _vTop.Data = vTopLineGeometry;
        }

        private void UpdateVerticalBottomLine()
        {
            LineGeometry refBottomLineGeometry = new()
            {
                StartPoint = _mVB,
                EndPoint = new(_mVB.X, 0),
            };

            Canvas.SetLeft(_refCenter, (_mVB.X - _refCenter.Width / 2));
            Canvas.SetTop(_refCenter, (_mVB.Y - _refCenter.Height / 2));

            _refBottom.Data = refBottomLineGeometry;
            _refBottom.Stroke = Brushes.Green;
            
            LineGeometry vBottomLineGeometry = new()
            {
                StartPoint = _cVB,
                EndPoint = _vB,
            };

            _vBottom.Data = vBottomLineGeometry;
        }

        

        private void UpdateElements()
        {
            UpdateHorizontalLine();
            UpdateVerticalTopLine();
            UpdateVerticalBottomLine();
        }

        private void ShowElements()
        {
            _board.Children.Add(_refCenter);

            _board.Children.Add(_refBottom);

            _board.Children.Add(_hAB);

            _board.Children.Add(_vTop);

            _board.Children.Add(_vBottom);

            Canvas.SetLeft(_leftRightBlock, _board.ActualWidth - _leftRightBlock.ActualWidth - 140);
            Canvas.SetTop(_leftRightBlock, 20);

            _board.Children.Add(_leftRightBlock);

            //_board.Children.Add(_refAB);

            //_board.Children.Add(_refTop);
        }

        public void DestroyTool()
        {
            _board.PreviewMouseUp -= Board_PreviewMouseUp;
            _board.PreviewMouseMove -= Board_PreviewMouseMove;
        }

        public void DoAction(string actName, object value)
        {
            switch (actName)
            {
                case ActionSetHeadTilt:
                    _headTilt = (double)value;
                    break;
                case ActionSetSpineTilt:
                    _spineTilt = (double)value;
                    break;
                case ActionSetFA:
                    _fa = (double)value;
                    break;
                case ActionSetHTRight:
                    _htRight = (bool)value;
                    break;
                case ActionSetSTRight:
                    _stRight = (bool)value;
                    break;
                case ActionSetFAHigh:
                    _faHigh = (bool)value;
                    break;
                case ActionSetFACRight:
                    _facRight = (bool)value;
                    break;
                case ActionDrawMisalignment:
                    DrawMisalignment();
                    break;
            }
        }

        private void DrawMisalignment()
        {
            UpdateCalculations();
            UpdateElements();
            ShowElements();
        }

        private void UpdateCalculations()
        {
            //_fa = 5; _headTilt = 15; _spineTilt = 25;
            double ht = _headTilt, st = _spineTilt, fa = _fa;
            if (!_htRight)
            {
                ht *= -1;
            }
            if (!_stRight)
            {
                st *= -1;
            }
            if (!_faHigh)
            {
                fa *= -1;
            }
            if (!_facRight)
            {
                fa *= -1;
            }
            if (_rightReversed)
            {
                ht *= -1;
                st *= -1;
                fa *= -1;
            }
            double m = Math.Tan(ht * Math.PI / 180);
            double x = (_cVT.Y - _vT.Y) * m;
            _vT = new(_cVT.X + x, _vT.Y);

            m = Math.Tan(st * Math.PI / 180);
            x = (_cVB.Y - _vB.Y) * m;
            _vB = new(_cVB.X + x, _vB.Y);

            m = Math.Tan(fa * Math.PI / 180);
            double y = (_cR.X - _hB.X) * m;
            _hB = new(_hB.X, _cR.Y + y);
            _hA = new(_hA.X, _cR.Y - y);

            Point verc = new(GeometryFunctions.GetXFromY(_cVB, _vB, _board.ActualHeight), _board.ActualHeight);
            _mVB = GeometryFunctions.GetMidpointOfTwoPoints(_cVB, verc);
        }

        public Path DrawLine(double opacity = 1)
        {
            SolidColorBrush solidColorBrush = new()
            {
                Color = Colors.Red,
                Opacity = opacity,
            };
            
            Path myPath = new()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = solidColorBrush,
                StrokeThickness = 3,
            };

            return myPath;
        }

        private Ellipse DrawEllipse()
        {
            Ellipse ellipse = new()
            {
                Height = 8,
                Width = 8,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
            };

            return ellipse;
        }
    }
}
