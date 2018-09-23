using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup
{
    public interface ITreeLookup<TVm, T>
    {
        Task Lookup(T value, ITreeSelector<TVm, T> parentSelector, ICompareHierarchy<T> comparer,
            params ITreeLookupProcessor<TVm, T>[] processors);
    }
}