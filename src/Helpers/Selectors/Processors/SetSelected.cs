using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Processors
{
    public class SetSelected<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetSelected<TVm, T> WhenSelected = new SetSelected<TVm, T>();

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (hr == HierarchicalResult.Current)
                selector.IsSelected = true;
            return true;
        }
    }
}