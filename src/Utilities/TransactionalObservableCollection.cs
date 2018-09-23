using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace TreeViewEx.Utilities
{
    /// <summary>
    ///     A <see cref="ObservableCollection{T}" /> that implements transactional methods like <see cref="AddRange" /> or
    ///     <see cref="RemoveRange" />.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    //http://stackoverflow.com/questions/7687000/fast-performing-and-thread-safe-observable-collection
    public class TransactionalObservableCollection<T> : ObservableCollection<T>
    {
        private bool _isCollectionChangedNotificationSuspended;

        /// <summary>
        ///     Initializes a new instance of <see cref="TransactionalObservableCollection{T}" />.
        /// </summary>
        public TransactionalObservableCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionalObservableCollection{T}" /> class that contains elements
        ///     copied from the specified collection.
        /// </summary>
        /// <param name="collection"></param>
        public TransactionalObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionalObservableCollection{T}" /> class that contains elements
        ///     copied from the specified list.
        /// </summary>
        /// <param name="list"></param>
        public TransactionalObservableCollection(List<T> list) : base(list)
        {
        }

        /// <summary>
        ///     This event is overriden CollectionChanged event of the observable collection.
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        ///     Adds the elements of the specified collection to the end of the <see cref="TransactionalObservableCollection{T}" />
        ///     .
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the
        ///     <see cref="TransactionalObservableCollection{T}" />. The collection itself cannot be null, but it can contain
        ///     elements that
        ///     are null, if type <see cref="T" /> is a reference type.
        /// </param>
        public void AddRange(IEnumerable<T> collection)
        {
            SuspendCollectionChangeNotification();
            try
            {
                foreach (var item in collection)
                    InsertItem(Count, item);
            }
            finally
            {
                NotifyChanges();
            }
        }

        /// <summary>
        ///     Remove the elements of the specified collection from the <see cref="TransactionalObservableCollection{T}" />
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be removed from the
        ///     <see cref="TransactionalObservableCollection{T}" />.
        /// </param>
        public void RemoveRange(IEnumerable<T> collection)
        {
            SuspendCollectionChangeNotification();
            try
            {
                foreach (var item in collection)
                    Remove(item);
            }
            finally
            {
                NotifyChanges();
            }
        }

        /// <summary>
        ///     Suspends collection changed notification.
        /// </summary>
        public void SuspendCollectionChangeNotification()
        {
            _isCollectionChangedNotificationSuspended = true;
        }

        /// <summary>
        ///     Resumes collection changed notification.
        /// </summary>
        public void ResumeCollectionChangeNotification()
        {
            _isCollectionChangedNotificationSuspended = false;
        }

        /// <summary>
        ///     Raises collection change event.
        /// </summary>
        public void NotifyChanges()
        {
            ResumeCollectionChangeNotification();
            var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(arg);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Recommended is to avoid reentry 
            // in collection changed event while collection
            // is getting changed on other thread.
            using (BlockReentrancy())
            {
                if (!_isCollectionChangedNotificationSuspended)
                {
                    var eventHandler = CollectionChanged;
                    if (eventHandler == null)
                        return;

                    // Walk thru invocation list.
                    var delegates = eventHandler.GetInvocationList();

                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                        // If the subscriber is a DispatcherObject and different thread.
                        if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess())
                            dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                        else
                            handler(this, e);
                }
            }
        }
    }
}