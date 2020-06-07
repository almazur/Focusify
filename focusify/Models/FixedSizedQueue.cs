using System;
using System.Collections.Concurrent;

namespace focusify.Models
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();
        public int Size { get; private set; }
        public FixedSizedQueue(int size)
        {
            Size = size;
        }
        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }

        public T[] Take(int length)
        {
            length = Math.Min(length, base.Count); 
            T[] result = new T[length];
            Array.Copy(this.ToArray(), result, length);
            return result;
        }
    }
}