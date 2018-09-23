using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup
{
    public interface ITreeLookupProcessor<TVm, T>
    {
        bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector);
    }
}