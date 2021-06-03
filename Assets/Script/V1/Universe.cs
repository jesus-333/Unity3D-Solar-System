using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    [Range(1, 10)]
    public float gravitational_constant = 6.674f;

    [Range(0f, 0.1f)]
    public float physics_time_step = 0.01f;

    private CelestialBody[] list_of_bodies;


    void Awake(){
        list_of_bodies = FindObjectsOfType<CelestialBody>();
        // Time.fixedDeltaTime = UniverseConstants.physics_time_step;
    }


    void Update(){
        UniverseConstants.gravitational_constant = gravitational_constant;

        UniverseConstants.physics_time_step = physics_time_step;
        // Time.fixedDeltaTime = UniverseConstants.physics_time_step;
    }

    void FixedUpdate(){
        for (int i = 0; i < list_of_bodies.Length; i++){
            list_of_bodies[i].UpdateVelocity(list_of_bodies, UniverseConstants.physics_time_step);
            // print("i = " + i + "\t name = " + list_of_bodies[i].name);
        }

        for (int i = 0; i < list_of_bodies.Length; i++){
            list_of_bodies[i].UpdatePosition(UniverseConstants.physics_time_step);
        }
    }
}

public static class UniverseConstants{
    public static float gravitational_constant = 6.674f;
    // public static float physics_time_step = 0.01f;
    public static float physics_time_step = Time.fixedDeltaTime;
}
