using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors.Processors
{
    public class SetChildSelected<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetChildSelected<TVm, T> ToSelectedChild = new SetChildSelected<TVm, T>();

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (hr == HierarchicalResult.Child || hr == HierarchicalResult.Current)
                parentSelector.SetSelectedChild(selector.Value);
            return true;
        }
    }
}