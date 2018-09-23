using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Nito.AsyncEx;
using TreeViewEx.Controls.Models;

namespace TreeViewEx.Helpers
{
    /// <summary>
    ///     Helper view model class that provide support of loading sub-entries.
    /// </summary>
    /// <typeparam name="TVm">The view model type</typeparam>
    public interface IEntriesHelper<TVm> : INotifyPropertyChanged
    {
        /// <summary>
        ///     Load when expand the first time.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        ///     Whether subentries loaded.
        /// </summary>
        bool IsLoaded { get; set; }

        /// <summary>
        ///     True if the entries are currently being loaded
        /// </summary>
        bool IsLoading { get; set; }

        IEnumerable<TVm> AllNonBindable { get; }

        /// <summary>
        ///     A list of sub-entries, after loaded.
        /// </summary>
        ObservableCollection<TVm> All { get; }

        AsyncLock LoadingLock { get; }

        /// <summary>
        ///     Call to load sub-entries.
        /// </summary>
        /// <param name="updateMode">The update mode</param>
        /// <param name="force">Load sub-entries even if it's already loaded.</param>
        /// <param name="parameter">Custom parameter that is passed to the load function</param>
        /// <param name="uiScheduler">
        ///     The <see cref="TaskScheduler" /> of the UI thread. If null, the current synchronization
        ///     context will be used.
        /// </param>
        Task<IEnumerable<TVm>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false,
            object parameter = null, TaskScheduler uiScheduler = null);

        Task UnloadAsync();

        /// <summary>
        ///     Set the entries to the new entry collection. If previous entries exist, they will be cleared
        /// </summary>
        /// <param name="viewModels">The new entries</param>
        void SetEntries(IEnumerable<TVm> viewModels);

        /// <summary>
        ///     Update the entries with a new collection. Entries that already exited won't be moved, new entries will be added and
        ///     old entries will be removed.
        /// </summary>
        /// <param name="viewModels">The new entries</param>
        void UpdateEntries(IEnumerable<TVm> viewModels);

        event EventHandler EntriesChanged;
    }
}