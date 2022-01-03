using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> queue;
        
        public T Top
        {
            get => queue[0];
        }
        public int Count
        {
            get => queue.Count;
        }
        public PriorityQueue()
        {
            queue = new List<T>();
        }
        public void Enqueue(T item)
        {
            queue.Add(item);
            AdjustUp(queue.Count - 1);
        }
        public T Dequeue()
        {
            if(queue.Count == 0) return default(T);
            T temp = queue[0];
            queue[0] = queue[Count - 1];
            queue.RemoveAt(Count - 1);
            AdjustDown(0);
            return temp;
        }
        public bool Contains(T item)
        {
            return queue.Contains(item);
        }
        public bool Exists(Predicate<T> match)
        {
            return queue.Exists(match);
        }
        private void AdjustUp(int child)
        {
            int parent = (child - 1) / 2;
            while(parent >= 0)
            {
                if(queue[parent].CompareTo(queue[child]) != 0)
                {
                    T temp = queue[parent];
                    queue[parent] = queue[child];
                    queue[child] = temp;
                    child = parent;
                    parent = (child - 1) / 2;
                }
                else break;
            }
        }
        private void AdjustDown(int parent)
        {
            int child = parent * 2 + 1;
            while(child < queue.Count)
            {
                if(child + 1 < queue.Count && queue[child].CompareTo(queue[child + 1]) != 0) ++child;
                if(queue[parent].CompareTo(queue[child]) != 0)
                {
                    T temp = queue[parent];
                    queue[parent] = queue[child];
                    queue[child] = temp;
                    parent = child;
                    child = parent * 2 + 1;
                }
                else break;
            }
        }
    }
}