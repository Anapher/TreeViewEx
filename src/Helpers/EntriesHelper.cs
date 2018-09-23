using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using TreeViewEx.Controls.Models;
using TreeViewEx.Utilities;

namespace TreeViewEx.Helpers
{
    public class EntriesHelper<TVm> : PropertyChangedBase, IEntriesHelper<TVm>
    {
        private bool _isExpanded;
        
        private bool _isLoaded;
        private bool _isLoading;

        private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();
        protected Func<bool, object, Task<IEnumerable<TVm>>> LoadSubEntryFunc;

        public EntriesHelper(Func<bool, object, Task<IEnumerable<TVm>>> loadSubEntryFunc)
        {
            LoadSubEntryFunc = loadSubEntryFunc;

            //add null item because we don't actually know if there exist any items
            All = new TransactionalObservableCollection<TVm> {default(TVm)};
        }

        public EntriesHelper(Func<bool, Task<IEnumerable<TVm>>> loadSubEntryFunc) : this((b, _) => loadSubEntryFunc(b))
        {
        }

        public EntriesHelper(Func<Task<IEnumerable<TVm>>> loadSubEntryFunc) : this(_ => loadSubEntryFunc())
        {
        }

        public EntriesHelper() : this(Enumerable.Empty<TVm>())
        {
        }

        public EntriesHelper(IEnumerable<TVm> entries)
        {
            _isLoaded = true;
            All = new TransactionalObservableCollection<TVm>();

            ((TransactionalObservableCollection<TVm>) All).AddRange(entries);
        }

        public event EventHandler EntriesChanged;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(value, ref _isExpanded))
                    LoadAsync().Forget();
            }
        }

        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(value, ref _isLoaded);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(value, ref _isLoading);
        }

        public bool ClearBeforeLoad { get; set; } = false;
        public DateTimeOffset LastRefreshTimeUtc { get; private set; }

        public IEnumerable<TVm> AllNonBindable => All;
        public ObservableCollection<TVm> All { get; }
        public AsyncLock LoadingLock { get; } = new AsyncLock();

        public async Task UnloadAsync()
        {
            _lastCancellationToken.Cancel(); //Cancel previous load.  

            using (await LoadingLock.LockAsync())
            {
                All.Clear();
                _isLoaded = false;
            }
        }

        public async Task<IEnumerable<TVm>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false,
            object parameter = null, TaskScheduler uiScheduler = null)
        {
            if (LoadSubEntryFunc != null) //Ignore if contructucted using entries but not entries func
            {
                _lastCancellationToken.Cancel(); //Cancel previous load.    

                using (await LoadingLock.LockAsync())
                {
                    _lastCancellationToken = new CancellationTokenSource();
                    if (!_isLoaded || force)
                    {
                        if (ClearBeforeLoad)
                            All.Clear();

                        if (uiScheduler == null)
                            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                        try
                        {
                            IsLoading = true;
                            await LoadSubEntryFunc(_isLoaded, parameter).ContinueWith((prevTask, _) =>
                            {
                                IsLoaded = true;
                                IsLoading = false;

                                if (!prevTask.IsCanceled && !prevTask.IsFaulted)
                                {
                                    switch (updateMode)
                                    {
                                        case UpdateMode.Replace:
                                            SetEntries(prevTask.Result);
                                            break;
                                        case UpdateMode.Update:
                                            UpdateEntries(prevTask.Result);
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(updateMode), updateMode, null);
                                    }

                                    LastRefreshTimeUtc = DateTimeOffset.UtcNow;
                                }
                            }, _lastCancellationToken, uiScheduler);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Debug.Fail("Cannot obtain SynchronizationContext", ex.ToString());
                        }
                    }
                }
            }

            return All;
        }

        public void UpdateEntries(IEnumerable<TVm> viewModels)
        {
            var all = (TransactionalObservableCollection<TVm>) All;

            var removedItems = all.ToList();
            var addedItems = new List<TVm>();

            all.SuspendCollectionChangeNotification();
            try
            {
                addedItems.AddRange(viewModels.Where(viewModel => !removedItems.Remove(viewModel)));

                foreach (var vm in removedItems)
                    all.Remove(vm);

                foreach (var vm in addedItems)
                    all.Add(vm);
            }
            finally
            {
                all.NotifyChanges();
            }
            
            EntriesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetEntries(IEnumerable<TVm> viewModels)
        {
            var all = (TransactionalObservableCollection<TVm>) All;

            all.SuspendCollectionChangeNotification();
            all.Clear();
            all.AddRange(AllNonBindable);

            EntriesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}