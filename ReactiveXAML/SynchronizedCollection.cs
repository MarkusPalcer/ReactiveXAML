// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizedCollection.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   A collection which listens for change messages and updates itself accordingly.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReactiveXAML
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Threading;

    public class SynchronizedCollection<T> : INotifyCollectionChanged, IEnumerable<T>, IObserver<SynchronizedCollection<T>.ChangeEvent>
    {
        private readonly ObservableCollection<T> underlyingCollection;

        private readonly Dispatcher dispatcher;

        public SynchronizedCollection()
        {
            underlyingCollection = new ObservableCollection<T>();
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public SynchronizedCollection(IEnumerable<T> items)
        {
            underlyingCollection = new ObservableCollection<T>(items);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public SynchronizedCollection(IList<T> items)
        {
            underlyingCollection = new ObservableCollection<T>(items);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.underlyingCollection.CollectionChanged += value;
            }

            remove
            {
                this.underlyingCollection.CollectionChanged -= value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.underlyingCollection.GetEnumerator();
        }

        public void OnNext(ChangeEvent value)
        {
            dispatcher.Invoke(() => this.ProcessEvent(value));
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        private void ProcessEvent(ChangeEvent change)
        {
            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    underlyingCollection.Clear();
                    foreach (var item in change.NewItems)
                    {
                        underlyingCollection.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Add:
                    if (change.NewStartingIndex < 0)
                    {
                        Array.ForEach(change.NewItems, underlyingCollection.Add);
                    }
                    else
                    {
                        var index = change.NewStartingIndex;
                        foreach (var item in change.NewItems)
                        {
                            underlyingCollection.Insert(index, item);
                            index++;
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    underlyingCollection.Move(change.OldStartingIndex, change.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (change.OldStartingIndex < 0)
                    {
                        Array.ForEach(change.OldItems, item => underlyingCollection.Remove(item));
                    }
                    else
                    {
                        for (int i = 0; i < change.OldItems.Length; i++)
                        {
                            underlyingCollection.RemoveAt(change.OldStartingIndex);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (change.OldStartingIndex < 0)
                    {
                        var replaceActions = change.OldItems.Zip(change.NewItems, (x, y) => new Tuple<T, T>(x, y));
                        foreach (var tuple in replaceActions)
                        {
                            var index = underlyingCollection.IndexOf(tuple.Item1);
                            underlyingCollection.Remove(tuple.Item1);
                            underlyingCollection.Insert(index, tuple.Item2);
                        }
                    }
                    else
                    {
                        var index = change.OldStartingIndex;
                        foreach (var item in change.NewItems)
                        {
                            underlyingCollection.RemoveAt(index);
                            underlyingCollection.Insert(index, item);
                            index++;
                        }
                    }

                    break;
            }
        }

        public class ChangeEvent
        {
            private ChangeEvent()
            {
            }

            public NotifyCollectionChangedAction Action { get; set; }

            public T[] NewItems { get; set; }

            public T[] OldItems { get; set; }

            public int NewStartingIndex { get; set; }

            public int OldStartingIndex { get; set; }

            public static ChangeEvent Replace(int index, T newItem)
            {
                return Replace(index, new[] { newItem });
            }

            public static ChangeEvent Replace(int startingIndex, T[] newItems)
            {
                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Replace,
                               OldStartingIndex = startingIndex,
                               NewItems = newItems
                           };
            }

            public static ChangeEvent Replace(T oldItem, T newItem)
            {
                return Replace(new[] { oldItem }, new[] { newItem });
            }

            public static ChangeEvent Replace(T[] oldItems, T[] newItems)
            {
                if (oldItems.Length != newItems.Length)
                {
                    throw new ArgumentException("Both lists need to be the same length.");
                }

                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Replace,
                               NewItems = newItems,
                               OldItems = oldItems,
                               OldStartingIndex = -1
                           };
            }

            public static ChangeEvent ReplaceAll(T[] newList)
            {
                return new ChangeEvent { Action = NotifyCollectionChangedAction.Reset, NewItems = newList };
            }

            public static ChangeEvent Add(T item)
            {
                return Add(new[] { item });
            }

            public static ChangeEvent Add(T[] items)
            {
                return new ChangeEvent { Action = NotifyCollectionChangedAction.Add, NewItems = items, NewStartingIndex = -1 };
            }

            public static ChangeEvent Insert(T item, int index)
            {
                return Insert(new[] { item }, index);
            }

            public static ChangeEvent Insert(T[] items, int index)
            {
                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Add,
                               NewItems = items,
                               NewStartingIndex = index
                           };
            }

            public static ChangeEvent Move(int fromIndex, int toIndex)
            {
                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Move,
                               NewStartingIndex = toIndex,
                               OldStartingIndex = fromIndex
                           };
            }

            public static ChangeEvent RemoveAt(int index)
            {
                return RemoveAt(index, 1);
            }

            public static ChangeEvent RemoveAt(int index, int count)
            {
                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Remove,
                               OldStartingIndex = index,
                               OldItems = Enumerable.Repeat(default(T), count).ToArray()
                           };
            }

            public static ChangeEvent Remove(T item)
            {
                return Remove(new[] { item });
            }

            private static ChangeEvent Remove(T[] items)
            {
                return new ChangeEvent
                           {
                               Action = NotifyCollectionChangedAction.Remove,
                               OldStartingIndex = -1,
                               OldItems = items
                           };
            }
        }
    }
}