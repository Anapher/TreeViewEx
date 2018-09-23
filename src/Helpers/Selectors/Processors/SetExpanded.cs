using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.Selectors.Processors
{
    public class SetExpanded<TVm, T> : ITreeLookupProcessor<TVm, T>
    {
        public static SetExpanded<TVm, T> WhenChildSelected = new SetExpanded<TVm, T>(HierarchicalResult.Child);
        public static SetExpanded<TVm, T> WhenSelected = new SetExpanded<TVm, T>(HierarchicalResult.Current);

        public SetExpanded(HierarchicalResult matchResult)
        {
            MatchResult = matchResult;
        }

        private HierarchicalResult MatchResult { get; }

        public bool Process(HierarchicalResult hr, ITreeSelector<TVm, T> parentSelector, ITreeSelector<TVm, T> selector)
        {
            if (MatchResult.HasFlag(hr))
                selector.EntryHelper.IsExpanded = true;
            if (hr == HierarchicalResult.Current)
                ((IIntoViewBringable) selector.ViewModel).BringIntoView();
            return true;
        }
    }
}