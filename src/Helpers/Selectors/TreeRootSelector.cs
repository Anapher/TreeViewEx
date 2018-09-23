using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TreeViewEx.Controls.Models;
using TreeViewEx.Helpers.TreeLookup;
using TreeViewEx.Helpers.TreeLookup.Lookup;
using TreeViewEx.Helpers.TreeLookup.Processors;
using TreeViewEx.Utilities;

namespace TreeViewEx.Helpers.Selectors
{
    public class TreeRootSelector<TVm, T> : TreeSelector<TVm, T>, ITreeRootSelector<TVm, T>
    {
        private Stack<ITreeSelector<TVm, T>> _prevPath;
        private ObservableCollection<TVm> _rootItems;

        private T _selectedValue;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        public TreeRootSelector(IEntriesHelper<TVm> entryHelper) // int rootLevel = 0,
            //params Func<T, T, HierarchicalResult>[] compareFuncs)
            : base(entryHelper)
        {
            //_rootLevel = rootLevel;
            //_compareFuncs = compareFuncs;
            //Comparers = new [] { PathComparer.LocalDefault };
        }

        //private int _rootLevel;

        public ObservableCollection<TVm> OverflowedAndRootItems
        {
            get
            {
                if (_rootItems == null) UpdateRootItems();
                return _rootItems;
            }
            set
            {
                _rootItems = value;
                OnPropertyChanged();
            }
        }


        //public int RootLevel { get { return _rootLevel; } set { _rootLevel = value; } }

        public IEnumerable<ICompareHierarchy<T>> Comparers { get; set; }

        public override void ReportChildSelected(Stack<ITreeSelector<TVm, T>> path)
        {
            var prevSelector = SelectedSelector;
            var prevSelectedValue = _selectedValue;
            _prevPath = path;

            SelectedSelector = path.Last();
            _selectedValue = path.Last().Value;
            if (prevSelectedValue != null && !prevSelectedValue.Equals(path.Last().Value))
                prevSelector.IsSelected = false;
            OnPropertyChanged(nameof(SelectedValue));
            OnPropertyChanged(nameof(SelectedViewModel));
            SelectionChanged?.Invoke(this, EventArgs.Empty);

            //WARNING: Commented out by Anapher. I don't understand why you have to load all the root items if a child gets selected
            //updateRootItems(path);
        }

        public override void ReportChildDeselected(Stack<ITreeSelector<TVm, T>> path)
        {
        }

        public async Task SelectAsync(T value)
        {
            if (_selectedValue == null || CompareHierarchy(_selectedValue, value) != HierarchicalResult.Current)
                await LookupAsync(value, RecrusiveSearch<TVm, T>.LoadSubentriesIfNotLoaded,
                    SetSelected<TVm, T>.WhenSelected, SetChildSelected<TVm, T>.ToSelectedChild);
        }

        public event EventHandler SelectionChanged;

        public ITreeSelector<TVm, T> SelectedSelector { get; private set; }

        public TVm SelectedViewModel => SelectedSelector == null ? default : SelectedSelector.ViewModel;

        public T SelectedValue
        {
            get => _selectedValue;
            set => SelectAsync(value).Forget();
        }

        public HierarchicalResult CompareHierarchy(T value1, T value2)
        {
            foreach (var c in Comparers)
            {
                var retVal = c.CompareHierarchy(value1, value2);
                if (retVal != HierarchicalResult.Unrelated)
                    return retVal;
            }

            return HierarchicalResult.Unrelated;
        }

        private async Task updateRootItemsAsync(ITreeSelector<TVm, T> selector, ObservableCollection<TVm> rootItems,
            int level)
        {
            if (level == 0)
                return;

            var rootTreeSelectors = new List<ITreeSelector<TVm, T>>();
            await selector.LookupAsync(default, BroadcastNextLevel<TVm, T>.LoadSubentriesIfNotLoaded,
                new TreeLookupProcessor<TVm, T>(HierarchicalResult.All, (hr, p, c) =>
                {
                    rootTreeSelectors.Add(c);
                    return true;
                }));

            foreach (var c in rootTreeSelectors)
            {
                rootItems.Add(c.ViewModel);
                c.IsRoot = true;
                await updateRootItemsAsync(c, rootItems, level - 1);
            }
        }

        private void UpdateRootItems(Stack<ITreeSelector<TVm, T>> path = null)
        {
            if (_rootItems == null)
                _rootItems = new ObservableCollection<TVm>();
            else _rootItems.Clear();

            if (path != null && path.Count > 0)
            {
                foreach (var p in path.Reverse())
                    if (!EntryHelper.AllNonBindable.Contains(p.ViewModel))
                        _rootItems.Add(p.ViewModel);
                _rootItems.Add(default); //Separator
            }

            updateRootItemsAsync(this, _rootItems, 2).Forget();
        }
    }
}