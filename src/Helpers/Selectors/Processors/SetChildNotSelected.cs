using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Processors
{
    public class SetChildNotSelected<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetChildNotSelected<TVm, T> WhenChild = new SetChildNotSelected<TVm, T>(HierarchicalResult.Child);

        public static SetChildNotSelected<TVm, T> WhenNotChild =
            new SetChildNotSelected<TVm, T>(HierarchicalResult.Current | HierarchicalResult.Parent |
                                            HierarchicalResult.Unrelated);

        private readonly HierarchicalResult _hr;


        public SetChildNotSelected(HierarchicalResult hr)
        {
            _hr = hr;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (_hr.HasFlag(hr))

                selector.SetSelectedChild(default(T));
            return true;
        }
    }
}