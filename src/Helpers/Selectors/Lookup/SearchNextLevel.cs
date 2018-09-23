using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors.Lookup
{
    public class SearchNextLevel<TVm, T> : ITreeLookup<TVm, T>
    {
        public static SearchNextLevel<TVm, T> LoadSubentriesIfNotLoaded = new SearchNextLevel<TVm, T>();

        public async Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            foreach (var current in await parentSelector.EntryHelper.LoadAsync())
                if (current is ISupportTreeSelector<TVm, T> selector)
                {
                    var currentSelectionHelper = selector.Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                    switch (compareResult)
                    {
                        case HierarchicalResult.Current:
                        case HierarchicalResult.Child:
                            processors.Process(compareResult, parentSelector, currentSelectionHelper);
                            return;
                    }
                }
        }
    }
}