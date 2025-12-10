using System.Windows.Controls;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class Tool
    {
        public const string ActionUndo = nameof(ActionUndo);
        
        private string _name;
        private readonly ITool _tool;
        public Tool(Canvas designer, string name = NullTool.ToolName)
        {
            if (Global.CurrentTool?.TheTool.GetType() != typeof(NullTool))
            {
                Global.CurrentTool?.TheTool.DestroyTool();
            }
            _name = name;
            _tool = _name switch
            {
                CephaloTool.ToolName => new CephaloTool(designer),
                SnipImport.ToolName => new SnipImport(designer),
                StraightLine.ToolName => new StraightLine(designer),
                TextTool.ToolName => new TextTool(designer),
                MisalignmentTool.ToolName => new MisalignmentTool(designer),
                HAVATool.ToolName => new HAVATool(designer),
                Protractor.ToolName => new Protractor(designer),
                ThreePointCircle.ToolName => new ThreePointCircle(designer),
                CalibrateTool.ToolName => new CalibrateTool(designer),
                Freehand.ToolName => new Freehand(designer),
                SnipTool.ToolName => new SnipTool(designer),
                CenterPoint.ToolName => new CenterPoint(designer),
                _ => new NullTool(),
            };
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ITool TheTool
        {
            get { return _tool; }
        }
    }
}
