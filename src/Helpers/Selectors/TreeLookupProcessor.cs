using System;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors
{
    public class TreeLookupProcessor<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        private readonly HierarchicalResult _appliedResult;

        private readonly Func<HierarchicalResult, ITreeSelector<TVm, T>, ITreeSelector<TVm, T>, bool> _processFunc;

        public TreeLookupProcessor(HierarchicalResult appliedResult,
            Func<HierarchicalResult, ITreeSelector<TVm, T>, ITreeSelector<TVm, T>, bool> processFunc)
        {
            _processFunc = processFunc;
            _appliedResult = appliedResult;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (_appliedResult.HasFlag(hr))
                return _processFunc(hr, parentSelector, selector);
            return true;
        }
    }
}