using System;
using System.Collections.Generic;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

class Heap<T> where T : IHeapItem<T>
{
    T[] _Items;
    public int Count { get; private set; }

    public Heap(int MaxHeapSize)
    {
        _Items = new T[MaxHeapSize];
    }

    public void AddItem(T Item)
    {
        if (!EqualityComparer<T>.Default.Equals(Item, default(T)))
        {
            Item.HeapIndex = Count;
            _Items[Count] = Item;
            SortUp(Item);
            Count += 1;
        }
    }

    void SortUp(T Item)
    {
        int parentIndex = (Item.HeapIndex - 1) / 2;
        while (Item.CompareTo(_Items[parentIndex]) > 0)
        {
            Swap(Item, _Items[parentIndex]);
            parentIndex = (Item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T ItemA, T ItemB)
    {
        _Items[ItemA.HeapIndex] = ItemB;
        _Items[ItemB.HeapIndex] = ItemA;
        int itemAIndex = ItemA.HeapIndex;
        ItemA.HeapIndex = ItemB.HeapIndex;
        ItemB.HeapIndex = itemAIndex;
    }

    public T GetFirstItem()
    {
        if (Count > 1)
        {
            T result = _Items[0];
            Count -= 1;
            _Items[0] = _Items[Count];
            _Items[0].HeapIndex = 0;
            SortDown(_Items[0]);
            return result;
        }
        else if (Count == 1)
        {
            Count = 0;
            return _Items[0];
        }
        return default(T);
    }

    void SortDown(T Item)
    {
        int childIndexLeft = Item.HeapIndex * 2 + 1;
        int childIndexRight = Item.HeapIndex * 2 + 2;
        int swapIndex;

        while (childIndexLeft < Count)
        {
            if (childIndexRight < Count &&
                _Items[childIndexLeft].CompareTo(_Items[childIndexRight]) < 0)
            {
                swapIndex = childIndexRight;
            }
            else
            {
                swapIndex = childIndexLeft;
            }

            if (Item.CompareTo(_Items[swapIndex]) < 0)
            {
                Swap(Item, _Items[swapIndex]);
                childIndexLeft = Item.HeapIndex * 2 + 1;
                childIndexRight = Item.HeapIndex * 2 + 2;
            }
            else
            {
                return;
            }
        }
    }

    public void UpdateItem(T Item)
    {
        if (!EqualityComparer<T>.Default.Equals(Item, default(T)) && Item.HeapIndex < Count)
        {
            SortUp(Item);
        }
    }

    public bool Contains(T Item)
    {
        if (!EqualityComparer<T>.Default.Equals(Item, default(T)) && Item.HeapIndex < Count)
        {
            return Equals(_Items[Item.HeapIndex], Item);
        }
        return false;
    }

    public void Reset()
    {
        Count = 0;
    }
}
