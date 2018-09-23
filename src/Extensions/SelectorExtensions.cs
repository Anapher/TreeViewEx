using TreeViewEx.Helpers.Selectors;

namespace TreeViewEx.Extensions
{
    public static class SelectorExtensions
    {
        public static ITreeRootSelector<TVm, T> AsRoot<TVm, T>(this ITreeSelector<TVm, T> selector) => selector as ITreeRootSelector<TVm, T>;
    }
}