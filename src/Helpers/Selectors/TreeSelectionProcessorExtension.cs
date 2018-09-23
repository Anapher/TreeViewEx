using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup
{
    public static class TreeSelectionProcessorExtension
    {
        public static bool Process<TVm, T>(this ITreeLookupProcessor<TVm, T>[] processors, HierarchicalResult hr,
            ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            foreach (var p in processors)
                if (!p.Process(hr, parentSelector, selector))
                    return false;
            return true;
        }
    }
}