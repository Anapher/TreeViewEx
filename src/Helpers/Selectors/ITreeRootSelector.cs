using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers.TreeLookup
{
    /// <summary>
    ///     Implemented in tree node view model, to provide selection support.
    /// </summary>
    /// <typeparam name="TVm">ViewModel.</typeparam>
    /// <typeparam name="T">Value</typeparam>
    public interface ITreeRootSelector<TVm, T> : ITreeSelector<TVm, T>, ICompareHierarchy<T>
    {
        /// <summary>
        ///     Selected node.
        /// </summary>
        TVm SelectedViewModel { get; }

        ITreeSelector<TVm, T> SelectedSelector { get; }

        /// <summary>
        ///     Value of SelectedViewModel.
        /// </summary>
        T SelectedValue { get; set; }


        /// <summary>
        ///     Compare Hierarchy of two value.
        /// </summary>
        IEnumerable<ICompareHierarchy<T>> Comparers { get; set; }

        ObservableCollection<TVm> OverflowedAndRootItems { get; set; }

        /// <summary>
        ///     Select a tree node by value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SelectAsync(T value);

        /// <summary>
        ///     Raised when a node is selected, use SelectedValue/ViewModel to return the selected item.
        /// </summary>
        event EventHandler SelectionChanged;
    }
}