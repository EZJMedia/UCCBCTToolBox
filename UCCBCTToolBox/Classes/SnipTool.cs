using System;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;
using Point = System.Windows.Point;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Input;
using System.Collections.Generic;
using UCCBCTToolBox.Helpers;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UCCBCTToolBox.Views;
using System.Drawing.Imaging;

namespace UCCBCTToolBox.Classes
{
    internal class SnipTool : ITool
    {
        public const string ToolName = "SnipTool";
        public const string ActionSettingsDialog = nameof(ActionSettingsDialog);

        public static string _lastSaveDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                                   + System.IO.Path.DirectorySeparatorChar;
        private static string _imageType = "PNG";

        private Canvas _board;
        private Point _initialPoint;
        private Point _finalPoint;
        private readonly Stack<Shape> _shapeStack;
        private bool _isShapeDrawn;
        public const string InitialPointName = nameof(_initialPoint);
        public const string FinalPointName = nameof(_finalPoint);
        public const string SaveSS = "SaveSS";
        public static string _lastSavedSnip = "";

        public const string ActionImport = nameof(ActionImport);

        public SnipTool(Canvas canvas)
        {
            _board = canvas;
            _board.MouseDown += Board_MouseDown;
            _board.MouseUp += Board_MouseUp;
            _board.MouseMove += Board_MouseMove;

            _shapeStack = new Stack<Shape>();
            _isShapeDrawn = false;
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

                if (_isShapeDrawn)
                {
                    _board.Children.Remove(_shapeStack.Peek());
                }
                
                _shapeStack.Push(DrawRec());
                _board.Children.Add(_shapeStack.Peek());
                _isShapeDrawn = true;
            }
            if (e.LeftButton == MouseButtonState.Released && _isShapeDrawn)
            {
                System.Windows.Shapes.Rectangle rect = (System.Windows.Shapes.Rectangle)_shapeStack.Peek();
                double left = Canvas.GetLeft(rect), top = Canvas.GetTop(rect);
                //model.SaveSnip(ScreenSnip.CopyScreen((int)left, (int)top, (int)(left + rect.Width), (int)(top + rect.Height)));
                var dpi = MainWindow.ScreenDPI;
                Point scs = _board.PointToScreen(new Point(left, top));
                Point sce = _board.PointToScreen(new Point(left + rect.Width, top + rect.Height)); // No need to use dpi division, pointToScreen solves this.
                //scs.X /= dpi.DpiScaleX;
                //scs.Y /= dpi.DpiScaleY;
                //sce.X /= dpi.DpiScaleX;
                //sce.Y /= dpi.DpiScaleY;

                //System.Windows.Forms.MessageBox.Show("X: " + dpi.DpiScaleX.ToString() + ", Y: " + dpi.DpiScaleY.ToString());

                SaveSnip(ScreenSnip.CopyScreen((int)scs.X, (int)scs.Y, (int)sce.X, (int)sce.Y));
                if (rect != null)
                    _board.Children.Remove(rect);
                _isShapeDrawn = false;
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
            }

            e.Handled = true;
        }

        public Shape DrawRec()
        {
            System.Windows.Shapes.Rectangle rec = new()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Width = Math.Abs(_initialPoint.X - _finalPoint.X),
                Height = Math.Abs(_initialPoint.Y - _finalPoint.Y),
            };

            Canvas.SetLeft(rec, Math.Min(_initialPoint.X, _finalPoint.X));
            Canvas.SetTop(rec, Math.Min(_initialPoint.Y, _finalPoint.Y));

            return rec;
        }

        public static void SaveSnip(Bitmap source)
        {
            string ext = "." + _imageType.ToLower();
            using var savefile = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "Save Capture",
                InitialDirectory = _lastSaveDir,
                FileName = "CBCT_Capture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ext,
                CheckPathExists = true,
                Filter = _imageType + " Image|*" + ext,
            };

            if (savefile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {

            }
            else
            {
                _lastSavedSnip = savefile.FileName;
                if (_imageType == "JPG")
                    source.Save(savefile.FileName, ImageFormat.Jpeg);
                else
                    source.Save(savefile.FileName);
                string? dir = System.IO.Path.GetDirectoryName(savefile.FileName);
                _lastSaveDir = dir ?? _lastSaveDir;
            }
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
                case ActionSettingsDialog:
                    OpenSettingsDialog(value);
                    break;
                default:
                    break;
            }
        }

        private void OpenSettingsDialog(object owner)
        {
            var dialog = new SnipToolSettingsDialog(_imageType)
            {
                Owner = (MainWindow)owner
            };

            if (dialog.ShowDialog() == true)
            {
                _imageType = dialog.ImageType;
            }
        }
    }
}
