using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup.Processors
{
    public class SetCollapsed<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetCollapsed<TVm, T> WhenChildSelected = new SetCollapsed<TVm, T>(HierarchicalResult.Child);
        public static SetCollapsed<TVm, T> WhenNotRelated = new SetCollapsed<TVm, T>(HierarchicalResult.Unrelated);

        public SetCollapsed(HierarchicalResult matchResult)
        {
            MatchResult = matchResult;
        }

        private HierarchicalResult MatchResult { get; }

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (MatchResult.HasFlag(hr))
                selector.EntryHelper.IsExpanded = false;
            return true;
        }
    }
}