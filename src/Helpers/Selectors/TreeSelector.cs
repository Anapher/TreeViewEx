using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using TreeViewEx.Controls.Models;
using TreeViewEx.Helpers.Selectors.Lookup;
using TreeViewEx.Utilities;

namespace TreeViewEx.Helpers.Selectors
{
    public class TreeSelector<TVm, T> : PropertyChangedBase, ITreeSelector<TVm, T>
    {
        private readonly AsyncLock _lookupLock = new AsyncLock();
        private bool _isRoot;
        private bool _isSelected;
        private ITreeSelector<TVm, T> _prevSelected;
        private T _selectedValue;

        protected TreeSelector(IEntriesHelper<TVm> entryHelper)
        {
            EntryHelper = entryHelper;
            RootSelector = this as ITreeRootSelector<TVm, T>;
        }

        public TreeSelector(T currentValue, TVm currentViewModel, ITreeSelector<TVm, T> parentSelector,
            IEntriesHelper<TVm> entryHelper)
        {
            RootSelector = parentSelector.RootSelector;
            ParentSelector = parentSelector;
            EntryHelper = entryHelper;
            Value = currentValue;
            ViewModel = currentViewModel;
        }

        public T Value { get; }
        public TVm ViewModel { get; }

        public ITreeSelector<TVm, T> ParentSelector { get; }
        public ITreeRootSelector<TVm, T> RootSelector { get; }
        public IEntriesHelper<TVm> EntryHelper { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    SetIsSelected(value);
                    OnSelected(value);
                }
            }
        }

        public bool IsRoot
        {
            get => _isRoot;
            set
            {
                if (SetProperty(value, ref _isRoot))
                    OnPropertyChanged(nameof(IsRootAndIsChildSelected));
            }
        }

        public virtual bool IsChildSelected => _selectedValue != null;
        public virtual bool IsRootAndIsChildSelected => IsRoot && IsChildSelected;

        public T SelectedChild
        {
            get => _selectedValue;
            set
            {
                SetIsSelected(false);
                OnPropertyChanged(nameof(IsSelected));
                OnChildSelected(value);
                OnPropertyChanged(nameof(IsChildSelected));
            }
        }
        
        /// <summary>
        ///     Bubble up to TreeSelectionHelper for selection.
        /// </summary>
        /// <param name="path"></param>
        public virtual void ReportChildSelected(Stack<ITreeSelector<TVm, T>> path)
        {
            Debug.Print("Child selected: " + Value);
            if (path.Any())
            {
                _selectedValue = path.Peek().Value;

                OnPropertyChanged(nameof(SelectedChild));
                OnPropertyChanged(nameof(IsChildSelected));
            }

            path.Push(this);
            ParentSelector?.ReportChildSelected(path);
        }

        public virtual void ReportChildDeselected(Stack<ITreeSelector<TVm, T>> path)
        {
            Debug.Print("Child deselected: " + Value);

            path.Push(this);
            ParentSelector?.ReportChildDeselected(path);

            if (EntryHelper.IsLoaded) SetSelectedChild(default(T));
        }

        /// <summary>
        ///     Tunnel down to select the specified item.
        /// </summary>
        public async Task LookupAsync(T value, ITreeLookup<TVm, T> lookupProc,
            params ITreeLookupProcessor<TVm, T>[] processors)
        {
            using (await _lookupLock.LockAsync())
            {
                await lookupProc.Lookup(value, this, RootSelector, processors);
            }
        }

        public void SetIsSelected(bool value)
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
            SetSelectedChild(default(T));
        }

        public void SetSelectedChild(T newValue)
        {
            _selectedValue = newValue;

            OnPropertyChanged(nameof(SelectedChild));
            OnPropertyChanged(nameof(IsChildSelected));
            OnPropertyChanged(nameof(IsRootAndIsChildSelected));
        }

        public override string ToString() => Value == null ? "" : Value.ToString();

        public void OnSelected(bool selected)
        {
            if (selected)
                ReportChildSelected(new Stack<ITreeSelector<TVm, T>>());
            else ReportChildDeselected(new Stack<ITreeSelector<TVm, T>>());
        }

        public void OnChildSelected(T newValue)
        {
            if (_selectedValue == null || !_selectedValue.Equals(newValue))
            {
                if (_prevSelected != null) _prevSelected.SetIsSelected(false);

                SetSelectedChild(newValue);

                if (newValue != null)
                    LookupAsync(newValue, SearchNextLevel<TVm, T>.LoadSubentriesIfNotLoaded,
                        new TreeLookupProcessor<TVm, T>(HierarchicalResult.Related, (hr, p, c) =>
                        {
                            c.IsSelected = true;
                            _prevSelected = c;
                            return true;
                        })).Forget();
            }
        }
    }
}