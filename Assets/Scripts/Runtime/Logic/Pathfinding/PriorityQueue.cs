using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<(T item, float priority)> _heap = new();
    private readonly Dictionary<T, int> _positions = new();

    public int Count => _heap.Count;

    public bool Contains(T item) => _positions.ContainsKey(item);

    public void Enqueue(T item, float priority)
    {
        _heap.Add((item, priority));
        int index = _heap.Count - 1;
        _positions[item] = index;
        HeapifyUp(index);
    }

    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var root = _heap[0].item;

        var last = _heap[^1];
        _heap[0] = last;
        _heap.RemoveAt(_heap.Count - 1);
        _positions.Remove(root);

        if (_heap.Count > 0)
        {
            _positions[last.item] = 0;
            HeapifyDown(0);
        }

        return root;
    }

    public void UpdatePriority(T item, float newPriority)
    {
        if (!_positions.TryGetValue(item, out int index))
            return;

        float oldPriority = _heap[index].priority;
        _heap[index] = (item, newPriority);

        if (newPriority < oldPriority)
            HeapifyUp(index);
        else
            HeapifyDown(index);
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_heap[index].priority >= _heap[parent].priority)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int count = _heap.Count;
        while (true)
        {
            int left = 2 * index + 1;
            int right = left + 1;
            int smallest = index;

            if (left < count && _heap[left].priority < _heap[smallest].priority)
                smallest = left;
            if (right < count && _heap[right].priority < _heap[smallest].priority)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
        _positions[_heap[a].item] = a;
        _positions[_heap[b].item] = b;
    }
}