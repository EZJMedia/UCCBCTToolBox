using System;
using Brushes = System.Windows.Media.Brushes;
using UCCBCTToolBox.Interfaces;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace UCCBCTToolBox.Classes
{
    internal class SnipImport : ITool
    {
        public const string ToolName = nameof(SnipImport);

        public static string _lastOpenDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                                   + System.IO.Path.DirectorySeparatorChar;
        private Canvas _board;

        public const string ActionImport = nameof(ActionImport);

        public SnipImport(Canvas canvas)
        {
            _board = canvas;

            _lastOpenDir = SnipTool._lastSaveDir;
        }

        private void ChooseImportSnip()
        {
            using var dialog = new System.Windows.Forms.OpenFileDialog()
            {
                InitialDirectory = _lastOpenDir,
                Filter = "Image Files(*.png)|*.png",
                Multiselect = true,
                Title = "Select Images to import",
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportSnip(dialog.FileNames);
            }
        }

        private void ImportSnip(string [] files)
        {
            try
            {
                foreach (string file in files)
                {
                    ImageBrush brush = new ImageBrush(new BitmapImage(new Uri(file, UriKind.Relative)));

                    System.Windows.Shapes.Rectangle imgPath = new()
                    {
                        Stroke = Brushes.Red,
                        Width = brush.ImageSource.Width,
                        Height = brush.ImageSource.Height,
                        Fill = brush
                    };

                    ContentControl contentControl = new()
                    {
                        Height = imgPath.Height,
                        Width = imgPath.Width,
                        Template = (ControlTemplate)_board.TryFindResource("DesignerItemTemplate"),
                        Content = imgPath,
                        Focusable = true
                    };

                    Canvas.SetLeft(contentControl, _board.Width / 2);
                    Canvas.SetTop(contentControl, _board.Height / 2);

                    contentControl.MouseMove += new MouseEventHandler((s, e) => { ((ContentControl)s).Focus(); e.Handled = true; });
                    contentControl.KeyUp += ContentControl_KeyUp;
                    contentControl.SizeChanged += ContentControl_SizeChanged;
                    _board.Children.Add(contentControl);

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Please snip before trying to import.", "Excetion while import", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        private void ContentControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ContentControl content = (ContentControl)sender;
            System.Windows.Shapes.Rectangle rectangle = (System.Windows.Shapes.Rectangle)content.Content;
            rectangle.Width = content.Width;
            rectangle.Height = content.Height;
        }

        private void ContentControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _board.Children.Remove((ContentControl)sender);
            }
        }

        public void DestroyTool()
        {

        }

        public void DoAction(string actName, object value)
        {
            switch (actName)
            {
                case ActionImport:
                    ChooseImportSnip();
                    break;
                default:
                    break;
            }
        }
    }
}
