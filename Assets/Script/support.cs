using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using UnityEngine;
using UnityEngine.UI;

public class support : MonoBehaviour
{
    public bool use_queue_to_track_orbit = false;

    public void Awake(){
        UniverseConstants.use_queue_to_track_orbit = use_queue_to_track_orbit;
    }

    // Function linked to the slider to select how many past points save to draw the orbits
    public static void setNPoints(float n_points){
        // Set the global variable of number of points
        UniverseConstants.n_points = (int)n_points;

        // Update the list (queue) of points of each body
        CelestialBodyV2[] list_of_bodies = GameObject.FindObjectsOfType<CelestialBodyV2>();
        foreach (CelestialBodyV2 body in list_of_bodies){
            body.orbit_points_queue.Size = (int)n_points;
        }

        // Update label
        Text txt = GameObject.Find("Orbit Points Text").GetComponent<Text>();
        txt.text = n_points + "";
    }

    public static void setLineWidth(float width){
        CelestialBodyV2.width = width;

        Text txt = GameObject.Find("Line Width Text").GetComponent<Text>();
        txt.text = width + "";
    }
}

// Queue to manage orbit point (partially take from stackoverflow)
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

    public static int n_points = 250;

    public static bool use_queue_to_track_orbit = false;

    public static float physics_time_step = Time.fixedDeltaTime;
}
