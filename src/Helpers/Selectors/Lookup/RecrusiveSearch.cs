using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors.Lookup
{
    public class RecrusiveSearch<TVm, T> : ITreeLookup<TVm, T>
    {
        public static RecrusiveSearch<TVm, T> LoadSubentriesIfNotLoaded = new RecrusiveSearch<TVm, T>(true);
        public static RecrusiveSearch<TVm, T> SkipIfNotLoaded = new RecrusiveSearch<TVm, T>(false);

        private readonly bool _loadSubEntries;

        public RecrusiveSearch(bool loadSubEntries)
        {
            _loadSubEntries = loadSubEntries;
        }

        public async Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            var subentries = _loadSubEntries
                ? await parentSelector.EntryHelper.LoadAsync()
                : parentSelector.EntryHelper.AllNonBindable;

            if (subentries != null)
                foreach (var current in subentries)
                    if (current is ISupportTreeSelector<TVm, T> selector)
                    {
                        var currentSelectionHelper = selector.Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                        switch (compareResult)
                        {
                            case HierarchicalResult.Current:
                                processors.Process(compareResult, parentSelector, currentSelectionHelper);
                                return;

                            case HierarchicalResult.Child:
                                if (processors.Process(compareResult, parentSelector, currentSelectionHelper))
                                {
                                    await Lookup(value, currentSelectionHelper, comparer, processors);
                                    return;
                                }

                                break;
                        }
                    }
        }
    }
}