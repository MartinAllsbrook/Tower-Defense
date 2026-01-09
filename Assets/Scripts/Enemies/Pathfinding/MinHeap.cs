using System;
using System.Collections.Generic;

/// <summary>
/// A generic min-heap (priority queue) implementation for efficient retrieval of the minimum element.
/// Useful for A* and Theta* pathfinding algorithms to manage the open set efficiently.
/// </summary>
/// <typeparam name="T">The type of elements in the heap</typeparam>
public class MinHeap<T>
{
    private List<T> heap;
    private Comparison<T> comparison;

    /// <summary>
    /// Gets the number of elements in the heap
    /// </summary>
    public int Count => heap.Count;

    /// <summary>
    /// Creates a new MinHeap with a custom comparison function
    /// </summary>
    /// <param name="comparison">Function that returns negative if first < second, 0 if equal, positive if first > second</param>
    public MinHeap(Comparison<T> comparison)
    {
        this.heap = new List<T>();
        this.comparison = comparison;
    }

    /// <summary>
    /// Adds an element to the heap
    /// Time complexity: O(log n)
    /// </summary>
    public void Add(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }

    /// <summary>
    /// Removes and returns the minimum element from the heap
    /// Time complexity: O(log n)
    /// </summary>
    public T RemoveMin()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        T min = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 0)
            HeapifyDown(0);

        return min;
    }

    /// <summary>
    /// Returns the minimum element without removing it
    /// Time complexity: O(1)
    /// </summary>
    public T Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        return heap[0];
    }

    /// <summary>
    /// Checks if the heap contains a specific element
    /// Time complexity: O(n)
    /// </summary>
    public bool Contains(T item)
    {
        return heap.Contains(item);
    }

    /// <summary>
    /// Removes all elements from the heap
    /// </summary>
    public void Clear()
    {
        heap.Clear();
    }

    /// <summary>
    /// Updates the position of an element in the heap after its priority has changed
    /// Call this when you modify an element's priority value
    /// Time complexity: O(n) for finding + O(log n) for reheapifying
    /// </summary>
    public void UpdatePriority(T item)
    {
        int index = heap.IndexOf(item);
        if (index == -1)
            return;

        // Try heapifying up first
        int parent = (index - 1) / 2;
        if (index > 0 && comparison(heap[index], heap[parent]) < 0)
        {
            HeapifyUp(index);
        }
        else
        {
            HeapifyDown(index);
        }
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparison(heap[index], heap[parentIndex]) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int smallest = index;
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;

            if (leftChild < heap.Count && comparison(heap[leftChild], heap[smallest]) < 0)
                smallest = leftChild;

            if (rightChild < heap.Count && comparison(heap[rightChild], heap[smallest]) < 0)
                smallest = rightChild;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}
