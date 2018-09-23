using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TreeViewEx.Helpers.Selectors
{
    public interface ITreeSelector<TVm, T> : INotifyPropertyChanged
    {
        /// <summary>
        ///     Whether current view model is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        ///     This is marked by TreeRootSelector, for overflow menu support.
        /// </summary>
        bool IsRoot { get; set; }

        /// <summary>
        ///     Whether a child of current view model is selected.
        /// </summary>
        bool IsChildSelected { get; }

        /// <summary>
        ///     Based on IsRoot and IsChildSelected
        /// </summary>
        bool IsRootAndIsChildSelected { get; }

        /// <summary>
        ///     The selected child of current view model.
        /// </summary>
        T SelectedChild { get; set; }

        /// <summary>
        ///     The owner view model of this selection helper.
        /// </summary>
        TVm ViewModel { get; }

        /// <summary>
        ///     The represented value of this selection helper.
        /// </summary>
        T Value { get; }

        ITreeSelector<TVm, T> ParentSelector { get; }
        ITreeRootSelector<TVm, T> RootSelector { get; }
        IEntriesHelper<TVm> EntryHelper { get; }

        /// <summary>
        ///     Used by a tree node to report to it's root it's selected.
        /// </summary>
        /// <param name="path"></param>
        void ReportChildSelected(Stack<ITreeSelector<TVm, T>> path);

        /// <summary>
        ///     Used by a tree node to report to it's parent it's deselected.
        /// </summary>
        /// <param name="path"></param>
        void ReportChildDeselected(Stack<ITreeSelector<TVm, T>> path);

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pathAction">Run when lookup along the path (e.g. when HierarchicalResult = Child or Current)</param>
        /// <param name="nextNodeOnly"></param>
        /// <returns></returns>
        Task LookupAsync(T value, ITreeLookup<TVm, T> lookupProc, params ITreeLookupProcessor<TVm, T>[] processors);

        void SetIsSelected(bool value);
        void SetSelectedChild(T value);
    }
}