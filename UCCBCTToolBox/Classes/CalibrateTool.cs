using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;
using UCCBCTToolBox.Views;

namespace UCCBCTToolBox.Classes
{
    internal class CalibrateTool : ITool
    {
        public const string ToolName = "CalibrateTool";
        private Point _pointA;
        private Point _pointB;
        public const string PointAName = nameof(_pointA);
        public const string PointBName = nameof(_pointB);
        private readonly Canvas _board;

        public CalibrateTool(Canvas canvas)
        {
            _board = canvas;

            _pointA = new(0, 0);
            _pointB = new(_pointA.X + 2000, 0);
            Shape shape = DrawLine();

            ContentControl contentControl = new()
            {
                MinWidth = 20,
                Height = shape.StrokeThickness,
                Width = 400,
                Template = (ControlTemplate)_board.TryFindResource("DesignerItemTemplate"),
                Content = shape
            };

            Canvas.SetTop(contentControl, _board.ActualHeight / 2);
            Canvas.SetLeft(contentControl, (_board.ActualWidth / 2) - 200);

            _board.Children.Add(contentControl);

            var dialog = new CalibrationDialog();
            dialog.Show();
            dialog.Closed += new EventHandler((s, e) =>
            {
                if (!dialog.IsCancel)
                {
                    ThreePointCircle.CalibrateUnit = dialog.unit.Text;
                    ThreePointCircle.CalibratedWidth = contentControl.Width;
                    ThreePointCircle.CalibratedValue = double.Parse(dialog.lengthValue.Text);
                }
                _board.Children.Remove(contentControl);
            });
        }

        public Shape DrawLine()
        {
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

            return myPath;
        }

        public void DestroyTool()
        {

        }

        public void DoAction(string actName, object value)
        {

        }
    }
}
