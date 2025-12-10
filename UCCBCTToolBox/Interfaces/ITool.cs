namespace UCCBCTToolBox.Interfaces
{
    internal interface ITool
    {
        public void DestroyTool();
        public void DoAction(string actName, object value);
    }
}
