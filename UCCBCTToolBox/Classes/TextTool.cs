using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class TextTool : ITool
    {
        public const string ToolName = nameof(TextTool);

        private Canvas _board;
        private Point up, down;

        private WrapPanel _focusedPanel = null;
        
        public TextTool(Canvas canvas)
        {
            _board = canvas;
            _board.Cursor = Cursors.IBeam;
            _board.PreviewMouseUp += Board_PreviewMouseUp;
            _board.PreviewMouseMove += Board_PreviewMouseMove;
        }

        private void Board_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _focusedPanel != null)
            {
                up = e.GetPosition(_board);

                Point lt = new(Canvas.GetLeft(_focusedPanel), Canvas.GetTop(_focusedPanel));
                Canvas.SetLeft(_focusedPanel, lt.X + up.X - down.X);
                Canvas.SetTop(_focusedPanel, lt.Y + up.Y - down.Y);

                down = up;
            }
        }

        private void Board_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush tback = new()
            {
                Color = Colors.Black,
                Opacity = 1
            };
            TextBox textBox = new()
            {
                FontSize = 16,
                Background = tback,
                Foreground = Brushes.White,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(1),
                AcceptsReturn = true,
                Margin = new Thickness(4),
                Padding = new Thickness(2),
            };
            textBox.PreviewMouseDoubleClick += TextBox_PreviewMouseDoubleClick;
            textBox.PreviewMouseUp += TextBox_PreviewMouseUp;
            textBox.LostFocus += TextBox_LostFocus;

            Point p = e.GetPosition(_board);

            WrapPanel panel = new()
            {
                Background = tback,
                Cursor = Cursors.SizeAll,
            };
            panel.Children.Add(textBox);
            panel.PreviewMouseDown += Panel_PreviewMouseDown;
            panel.PreviewMouseMove += Panel_PreviewMouseMove;
            panel.PreviewMouseUp += Panel_PreviewMouseUp;

            Canvas.SetLeft(panel, p.X);
            Canvas.SetTop(panel, p.Y);

            _board.Children.Add(panel);
            textBox.Focus();

            if (_focusedPanel != null)
            {
                up = e.GetPosition(_board);

                MovePanel();

                TextBox child = (TextBox)_focusedPanel.Children[0];
                child.Focus();

                _focusedPanel = null;
            }
        }

        private void MovePanel()
        {
            Point lt = new(Canvas.GetLeft(_focusedPanel), Canvas.GetTop(_focusedPanel));
            Canvas.SetLeft(_focusedPanel, lt.X + up.X - down.X);
            Canvas.SetTop(_focusedPanel, lt.Y + up.Y - down.Y);
        }

        private void Panel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_focusedPanel != null)
            {
                up = e.GetPosition(_board);

                MovePanel();

                TextBox child = (TextBox)_focusedPanel.Children[0];
                child.Focus();

                _focusedPanel = null;
            }
        }

        private void Panel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _focusedPanel != null)
            {
                up = e.GetPosition(_board);

                MovePanel();

                down = up;
            }
        }

        private void Panel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            down = e.GetPosition(_board);
            _focusedPanel = (WrapPanel)sender;
        }

        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
            textBox.Focus();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Trim().Length <= 0)
            {
                _board.Children.Remove((WrapPanel)textBox.Parent);
            }
        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Focus();
        }

        public void DestroyTool()
        {
            _board.Cursor = Cursors.Arrow;
            _board.PreviewMouseUp -= Board_PreviewMouseUp;
            _board.PreviewMouseMove -= Board_PreviewMouseMove;
        }

        public void DoAction(string actName, object value)
        {
            
        }
    }
}
