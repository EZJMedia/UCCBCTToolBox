using System.Windows.Controls;
using UCCBCTToolBox.Interfaces;
using UCCBCTToolBox.Views;

namespace UCCBCTToolBox.Classes
{
    internal class MainCalcsTool : ITool
    {
        public const string ToolName = nameof(MainCalcsTool);
        public static bool IsActive { get; set; }

        private readonly Canvas _board;
        private static readonly MainCalcsPanel _mainCalcsPanel = new();

        public MainCalcsTool(Canvas canvas)
        {
            _board = canvas;
        }
        
        public void DestroyTool()
        {
            throw new System.NotImplementedException();
        }

        public void DoAction(string actName, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
