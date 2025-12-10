using System;
using System.Windows;
using System.Windows.Shapes;
using UCCBCTToolBox.Interfaces;

namespace UCCBCTToolBox.Classes
{
    internal class NullTool : ITool
    {
        public const string ToolName = "NullTool";

        public void DestroyTool()
        {
            throw new NotImplementedException();
        }

        public void DoAction(string actName, object value)
        {
            throw new NotImplementedException();
        }
    }
}
