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
    internal class Freehand : ITool
    {
        public const string ToolName = "Freehand";

        private Canvas _board;
        private Point _initialPoint;
        private Point _finalPoint;
        public const string InitialPointName = nameof(_initialPoint);
        public const string FinalPointName = nameof(_finalPoint);
        public static string FillColor { get; set; } = "Red";
        public static int StrokeThickness { get; set; } = 1;

        private readonly List<int> _downUpIndices;

        public Freehand(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _downUpIndices = new();
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                _finalPoint = e.GetPosition(_board);

                Shape shape = DrawLine();
                _initialPoint = e.GetPosition(_board);

                _board.Children.Add(shape);
             
            }
            e.Handled = true;
        }

        private void Board_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }

            _finalPoint = e.GetPosition(_board);

            Shape shape = DrawLine();

            _downUpIndices.Add(_board.Children.Add(shape));

            e.Handled = true;
        }

        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.CurrentTool.Name != ToolName)
            {
                e.Handled = true;
                return;
            }
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _initialPoint = e.GetPosition(_board);
                _finalPoint = e.GetPosition(_board);

                Shape shape = DrawLine();

                _downUpIndices.Add(_board.Children.Add(shape));
            }
            e.Handled = true;
        }

        private Shape DrawLine()
        {
            Line line = new()
            {
                Stroke = new BrushConverter().ConvertFromString(FillColor) as SolidColorBrush,
                StrokeThickness = StrokeThickness,
                X1 = _initialPoint.X,
                Y1 = _initialPoint.Y,
                X2 = _finalPoint.X,
                Y2 = _finalPoint.Y,
            };

            return line;
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
            if (_downUpIndices.Count > 1)
            {
                int upIndex = _downUpIndices[^1];
                int downIndex = _downUpIndices[^2];

                _board.Children.RemoveRange(downIndex, upIndex - downIndex + 1);
                _downUpIndices.RemoveRange(_downUpIndices.Count - 2, 2);
            }
        }
    }
}
