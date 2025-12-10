using System.Collections.Generic;
using System.Windows.Shapes;
using UCCBCTToolBox.Classes;
using UCCBCTToolBox.Notifiers;
using System.Windows.Media;
using System.Windows;

namespace UCCBCTToolBox.ViewModels
{
    internal class MainWindowViewModel
    {
        #region Private Properties
        private Tool _tool;
        private readonly Stack<Shape> _shapeStack;
        private int _strokeThickness;
        private SolidColorBrush _fillColor;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ClientStateManager = new ClientStateManager();
            //ClientStateManager.SetTop();
            _tool = new Tool(null);
            _shapeStack = new Stack<Shape>();
            _strokeThickness = 1;
            _fillColor = System.Windows.Media.Brushes.Red;
        }
        #endregion

        #region Public Properties  
        public ClientStateManager ClientStateManager { get; private set; }
        public Tool CurrentTool
        {
            get { return _tool; }
            set { _tool = value; }
        }
        public SolidColorBrush FillColor
            { get { return _fillColor; } set { _fillColor = value; } }
        public int StrokeThickness
            { get { return _strokeThickness; } set { _strokeThickness = value; } }
        public Stack<Shape> ShapeStack { get { return _shapeStack; } }

        public Visibility IsDeactivateButtonVissible => Global.GetSavedValueFromRegistry("UserID") != null ? Visibility.Visible : Visibility.Collapsed;
        #endregion

        #region Public Methods
        
        #endregion
    }
}