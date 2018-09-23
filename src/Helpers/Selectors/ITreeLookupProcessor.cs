using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors
{
    public interface ITreeLookupProcessor<TVm, T>
    {
        bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector);
    }
}