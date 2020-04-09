using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * LimitedQueue -
 * Represents a queue which has a limited size. 
 * New items are added at the top, and excess items are removed from the bottom.
 * Items added are Added to the end of the inner queue, but for all intents and purposes
 * outside the class (such as foreach and ToArray());
 * =================================================================================== */

/// <summary>
/// Represents a linked last in, first out collection of objects, where objects that exceed a limited capacity are removed. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class LimitedQueue<T> : IEnumerable, IEnumerable<T>, IList<T>
{
    private class Link
    {
        public T Value;
        public Link Next;
        public Link Previous;

        //adding items to a limited queue puts them at the top, so can always know what's next.
        public Link(T Value, Link Next)
        {
            this.Value = Value;
            this.Next = Next;
        }

        public Link(T Value, Link Next, Link Previous)
        {
            this.Value = Value;
            this.Next = Next;
            this.Previous = Previous;
        }
    }

    public uint QueueSize
    {
        get
        {
            return queueSize;
        }
        set
        {
            queueSize = value;
            CheckItemOverLimit();
        }
    }

    private uint queueSize;
    private Link First;
    private Link Last;

    public event Action<T> OnItemEjected = (i) => { };

    /// <summary>
    /// Create a new LimitedQueue. Items not mandatory. 
    /// </summary>
    /// <param name="QueueSize"></param>
    /// <param name="Items"></param>
    public LimitedQueue(uint QueueSize, params T[] Items)
    {
        this.QueueSize = QueueSize;
        foreach (T item in Items)
            Add(item);
    }

    public T[] ToArray()
    {
        T[] res = new T[_count];
        Link current = First;
        for (int i = 0; i < _count; i++)
        {
            res[i] = current.Value;
            current = current.Next;
        }
        return res;
    }

    #region IEnumerable
    private class LimitedQueueEnumerator : IEnumerator, IEnumerator<T>
    {
        private Link CurrentLink;
        private Link InitialLink;

        public LimitedQueueEnumerator(Link First)
        {
            InitialLink = new Link(default(T), First); ;
            CurrentLink = InitialLink;
        }

        public bool MoveNext()
        {
            CurrentLink = CurrentLink.Next;
            return null != CurrentLink;
        }

        public void Reset()
        {
            CurrentLink = InitialLink;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }
        public T Current
        {
            get
            {
                return CurrentLink.Value;
            }
        }
        public void Dispose()
        {
            InitialLink = null;
            CurrentLink = null;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new LimitedQueueEnumerator(First);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new LimitedQueueEnumerator(First);
    }
    #endregion
    #region IList

    /// <summary>
    /// Add multiple items to the queue. 
    /// </summary>
    /// <param name="Items">A params array or items to add. </param>
    public void Add(params T[] Items)
    {
        foreach (T item in Items)
            Add(item);
    }

    /// <summary>
    /// Add
    /// </summary>
    /// <param name="Item"></param>
    public void Add(T Item)
    {
        Link n = new Link(Item, First);
        if (null != First)
            First.Previous = n;
        First = n;
        _count++;
        CheckItemOverLimit();
        if (1 == _count)
            Last = First;
    }

    private void CheckItemOverLimit()
    {
        while (_count > queueSize)
        {
            T eject = Last.Value;
            Last = Last.Previous;
            Last.Next = null;
            _count--;
            OnItemEjected(eject);
        }
    }

    /// <summary>
    /// Get or set an item from or to a position in the queue. 
    /// </summary>
    /// <param name="index">The index at which to get or set.</param>
    public T this[int index]
    {
        get
        {
            if (index > _count)
            {
                throw new IndexOutOfRangeException("This queue contains " + _count + " items. Provided index: " + index);
            }
            Link current = First;
            for (int i = 0; i < index; i++)
                current = current.Next;
            return current.Value;
        }
        set
        {
            if (index > _count)
            {
                throw new IndexOutOfRangeException("This queue contains " + _count + " items. Provided index: " + index);
            }
            Link current = First;
            for (int i = 0; i < index; i++)
                current = current.Next;
            current.Value = value;
        }
    }

    /// <summary>
    /// Clears the queue. 
    /// </summary>
    public void Clear()
    {
        First = null;
        Last = null;
        _count = 0;
    }

    /// <summary>
    /// Eject every item in the queue (invoking the OnItemEjected callback for each of them) and clears the queue. 
    /// </summary>
    public void EjectAll()
    {
        Link current = Last;
        while (null != current)
        {
            OnItemEjected(current.Value);
            current = current.Previous;
        }
        Clear();
    }

    /// <summary>
    /// This method returns whether a given item exists in the queue. 
    /// </summary>
    /// <param name="Item">The item to check for. </param>
    /// <returns>True if a match is found. Otherwise, false. </returns>
    public bool Contains(T Item)
    {
        Link current = First;
        while (null != current)
        {
            if (current.Value.Equals(Item))
                return true;
            current = current.Next;
        }
        return false;
    }

    /// <summary>
    /// The amount of items in the queue. 
    /// </summary>
    public int Count
    {
        get
        {
            return _count;
        }
    }

    private int _count;

    /// <summary>
    /// Insert an item into the queue at a given index. 
    /// </summary>
    /// <param name="index">The index at which to insert the item. </param>
    /// <param name="Item">The item to insert. </param>
    public void Insert(int index, T Item)
    {
        if (0 == index)
        {
            Add(Item);
            return;
        }
        if (_count < index - 1 || index < 0)
        {
            throw new IndexOutOfRangeException("This queue contains " + _count + " items. Provided index: " + index);
        }
        Link current = First;
        for (int i = 0; i < index; i++)
            current = current.Next;
        Link n = new Link(Item, current, current.Previous);
        n.Previous.Next = n;
        current.Previous = n;
        _count++;
        CheckItemOverLimit();
        if (null == n.Next)
            Last = n;
    }
    /// <summary>
    /// This method eturns the index of the first instance of this item in the queue.
    /// </summary>
    public int IndexOf(T Item)
    {
        Link current = First;
        int i = 0;
        while (null != current)
        {
            if (current.Value.Equals(Item))
                return i;
            i++;
        }
        return -1;
    }
    /// <summary>
    /// Remove the first and only the first instance that equals the given value. Returns false if no instance was found.
    /// </summary>
    /// <param name="Item">The item to remove. </param>
    /// <returns>True if an item was found to be removed from the queue. Otherwise, false. </returns>
    public bool Remove(T Item)
    {
        Link current = First;
        while (null != current)
        {
            if (current.Value.Equals(Item))
            {
                if (null != current.Previous)
                    current.Previous.Next = current.Next;
                if (null == current.Next)
                {
                    Last = current.Previous;
                }
                else current.Next.Previous = current.Previous;
                _count--;
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    /// <summary>
    /// Remove all occurences of a given item in the queue. Returns false if no instance was found.
    /// </summary>
    /// <param name="Item"></param>
    /// <returns>True if any items were removed. Otherwise, false. </returns>
    public bool RemoveAll(T Item)
    {
        bool removed = false;
        Link current = First;
        while (null != current)
        {
            if (current.Value.Equals(Item))
            {
                if (null != current.Previous)
                    current.Previous.Next = current.Next;
                if (null == current.Next)
                {
                    Last = current.Previous;
                }
                else current.Next.Previous = current.Previous;
                _count--;
                removed = true;
            }
            current = current.Next;
        }
        return removed;
    }

    /// <summary>
    /// Remove an item at a given index.
    /// </summary>
    /// <param name="index">The index to remove at. </param>
    public void RemoveAt(int index)
    {
        if (_count < index - 1 || index < 0)
        {
            throw new IndexOutOfRangeException("This queue contains " + _count + " items. Provided index: " + index);
        }
        Link current = First;
        for (int i = 0; i < index; i++)
            current = current.Next;
        if (null == current.Previous)
            First = current.Next;
        else current.Previous.Next = current.Next;
        if (null == current.Next)
            Last = current.Previous;
        else current.Next.Previous = current.Previous;
        _count--;

    }

    /// <summary>
    /// Removes an item at a given index and returns that item as an out variable. 
    /// </summary>
    /// <param name="index">The index to remove at. </param>
    /// <param name="Item">The removed item.</param>
    public void RemoveAt(int index, out T Item)
    {
        if (_count < index - 1)
        {
            throw new IndexOutOfRangeException("This queue contains " + _count + " items. Provided index: " + index);
        }
        Link current = First;
        for (int i = 0; i < index; i++)
            current = current.Next;
        if (null == current.Previous)
            First = current.Next;
        else current.Previous.Next = current.Next;
        if (null == current.Next)
            Last = current.Previous;
        else current.Next.Previous = current.Previous;
        Item = current.Value;
        _count--;
    }

    /// <summary>
    /// This collection is never read-only. Returns false.
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Copy the queue to an array at a given starting index. 
    /// </summary>
    /// <param name="array">The target array. </param>
    /// <param name="index">The starting index. </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the starting index plus the item count in the queue is greated than the target array's capacity. </exception>
    public void CopyTo(T[] array, int index)
    {
        if (index + _count < array.Length)
        {
            throw new ArgumentOutOfRangeException("index", new IndexOutOfRangeException("Array is too small for this operation."));
        }
        ToArray().CopyTo(array, index);
    }

    #endregion

    /// <summary>
    /// Move a value to the top of the queue. If it is not in the queue, it will be added to the queue.
    /// </summary>
    /// <param name="Item"></param>
    public void MoveToTop(T Item)
    {
        Remove(Item);
        Add(Item);
    }

    /// <summary>
    /// Move an item at the given index to the top of the queue.
    /// </summary>
    /// <param name="index"></param>
    public void MoveToTopAt(int index)
    {
        T item;
        RemoveAt(index, out item);
        Add(item);
    }

    /// <summary>
    /// Find a value in the queue based on a predicate function. 
    /// </summary>
    /// <param name="Predicate">The function to check if the value is acceptable</param>
    /// <returns>The first value to match the predicate, or null if none match. </returns>
    public T Find(Predicate<T> Predicate)
    {
        Link current = First;
        while (null != current)
        {
            if (Predicate(current.Value))
            {
                return current.Value;
            }
            current = current.Next;
        }
        return default(T);
    }
}