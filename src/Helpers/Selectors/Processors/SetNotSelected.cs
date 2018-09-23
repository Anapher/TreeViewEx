using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors.Processors
{
    public class SetNotSelected<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetNotSelected<TVm, T> WhenCurrent = new SetNotSelected<TVm, T>(HierarchicalResult.Current);

        public static SetNotSelected<TVm, T> WhenNotCurrent = new SetNotSelected<TVm, T>(
            HierarchicalResult.Child | HierarchicalResult.Parent | HierarchicalResult.Unrelated);

        private readonly HierarchicalResult _hr;

        public SetNotSelected(HierarchicalResult hr)
        {
            _hr = hr;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (_hr.HasFlag(hr))
                selector.IsSelected = false;
            return true;
        }
    }
}