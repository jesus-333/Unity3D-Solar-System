using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

// Queue to manage orbit point (take from stackoverflow)
public class FixedSizedQueue<T> : ConcurrentQueue<T>
{
    private readonly object syncObject = new object();

    public int Size;

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

    public T[] convertToArray(){
        T[] array = new T[Size];
        T tmp;
        for(int i = 0; i < Size; i++){
            base.TryDequeue(out tmp);
            array[i] = tmp;
            base.Enqueue(tmp);
        }

        return array;
    }
}

public static class UniverseConstants{
    public static float gravitational_constant = 6.674f;
    // public static float physics_time_step = 0.01f;
    public static float physics_time_step = Time.fixedDeltaTime;
}
