using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<(T item, int priority)> Heap = new();
    private readonly Dictionary<T, int> Positions = new();

    public int Count => Heap.Count;

    public bool Contains(T item) => Positions.ContainsKey(item);

    public void Enqueue(T item, int priority)
    {
        Heap.Add((item, priority));
        int index = Heap.Count - 1;
        Positions[item] = index;
        HeapifyUp(index);
    }

    public T Dequeue()
    {
        if (Heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var root = Heap[0].item;

        var last = Heap[^1];
        Heap[0] = last;
        Heap.RemoveAt(Heap.Count - 1);
        Positions.Remove(root);

        if (Heap.Count > 0)
        {
            Positions[last.item] = 0;
            HeapifyDown(0);
        }

        return root;
    }

    public void Clear()
    {
        Heap.Clear();
        Positions.Clear();
    }

    public void UpdatePriority(T item, int newPriority)
    {
        if (!Positions.TryGetValue(item, out int index))
            return;

        float oldPriority = Heap[index].priority;
        Heap[index] = (item, newPriority);

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
            if (Heap[index].priority >= Heap[parent].priority)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int count = Heap.Count;
        while (true)
        {
            int left = 2 * index + 1;
            int right = left + 1;
            int smallest = index;

            if (left < count && Heap[left].priority < Heap[smallest].priority)
                smallest = left;
            if (right < count && Heap[right].priority < Heap[smallest].priority)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (Heap[a], Heap[b]) = (Heap[b], Heap[a]);
        Positions[Heap[a].item] = a;
        Positions[Heap[b].item] = b;
    }
}