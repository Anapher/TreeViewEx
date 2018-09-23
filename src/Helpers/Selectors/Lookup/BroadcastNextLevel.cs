using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Lookup
{
    public class BroadcastNextLevel<TVm, T> : ITreeLookup<TVm, T>
    {
        public static BroadcastNextLevel<TVm, T> LoadSubentriesIfNotLoaded = new BroadcastNextLevel<TVm, T>();

        public async Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            foreach (var current in await parentSelector.EntryHelper.LoadAsync())
                if (current is ISupportTreeSelector<TVm, T> selector)
                {
                    var currentSelectionHelper = selector.Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                    processors.Process(compareResult, parentSelector, currentSelectionHelper);
                }
        }
    }
}