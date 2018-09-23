using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Lookup
{
    public class RecrusiveBroadcast<TVm, T> : ITreeLookup<TVm, T>
    {
        public static RecrusiveBroadcast<TVm, T> LoadSubentriesIfNotLoaded = new RecrusiveBroadcast<TVm, T>(false);
        public static RecrusiveBroadcast<TVm, T> SkipIfNotLoaded = new RecrusiveBroadcast<TVm, T>(false);

        private readonly bool _loadSubEntries;

        public RecrusiveBroadcast(bool loadSubEntries)
        {
            _loadSubEntries = loadSubEntries;
        }

        public async Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            var subentries = _loadSubEntries
                ? await parentSelector.EntryHelper.LoadAsync()
                : parentSelector.EntryHelper.AllNonBindable;

            foreach (var current in subentries)
                if (current is ISupportTreeSelector<TVm, T> selector)
                {
                    var currentSelectionHelper = selector.Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                    if (processors.Process(compareResult, parentSelector, currentSelectionHelper))
                    {
                        await Lookup(value, currentSelectionHelper, comparer, processors);
                    }

                    break;
                }
        }
    }
}