using System.Collections.Generic;
using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Lookup
{
    public class SearchNextUsingReverseLookup<TVm, T> : ITreeLookup<TVm, T>
    {
        private readonly Stack<ITreeSelector<TVm, T>> _hierarchy;
        private ITreeSelector<TVm, T> _targetSelector;

        public SearchNextUsingReverseLookup(ITreeSelector<TVm, T> targetSelector)
        {
            _targetSelector = targetSelector;
            _hierarchy = new Stack<ITreeSelector<TVm, T>>();
            var current = targetSelector;
            while (current != null)
            {
                _hierarchy.Push(current);
                current = current.ParentSelector;
            }
        }

        public Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            if (parentSelector.EntryHelper.IsLoaded)
                foreach (var current in parentSelector.EntryHelper.AllNonBindable)
                    if (current is ISupportTreeSelector<TVm, T> selector)
                    {
                        var currentSelectionHelper = selector.Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                        switch (compareResult)
                        {
                            case HierarchicalResult.Child:
                            case HierarchicalResult.Current:
                                if (_hierarchy.Contains(currentSelectionHelper))
                                {
                                    processors.Process(compareResult, parentSelector, currentSelectionHelper);
                                    return Task.CompletedTask;
                                }

                                break;
                        }
                    }

            return Task.CompletedTask;
        }
    }
}