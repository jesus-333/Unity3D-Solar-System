using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass;

    public Vector3 initial_velocity;
    private Vector3 current_velocity;
    private Vector3 force, force_direction, acceleration;

    public bool debug_var = false;

    void Awake()
    {
        current_velocity = initial_velocity;
        this.GetComponent<Rigidbody>().mass = mass;
    }

    public void UpdateVelocity(CelestialBody[] list_of_bodies, float time_step){
        // Elements for the update
        // Vector3 force, force_direction, acceleration;

        // Cycle through bodies
        for(int i = 0; i < list_of_bodies.Length; i++){
            CelestialBody other_body = list_of_bodies[i];
            if(other_body != this){
                // Distance between this body and the other body
                float square_distance = (other_body.GetComponent<Rigidbody>().position - this.GetComponent<Rigidbody>().position).sqrMagnitude;

                // Direction of the force
                force_direction = (other_body.GetComponent<Rigidbody>().position - this.GetComponent<Rigidbody>().position).normalized;

                // Force between the two body
                force = force_direction * UniverseConstants.gravitational_constant * mass * other_body.mass / square_distance;

                // Acceleration of the current body
                acceleration = force / mass;

                // Velocity update
                current_velocity += acceleration * time_step;

                if(debug_var){
                    print(this.toString());
                    print("other_body.mass = " + other_body.mass);
                    print("square_distance = "+ square_distance);
                    print(" UniverseConstants.gravitational_constant = " +  UniverseConstants.gravitational_constant);

                }
            }
        }
    }

    public void UpdatePosition(float time_step) {
        this.GetComponent<Rigidbody>().position += current_velocity * time_step;
        // this.GetComponent<Rigidbody>().AddForce(current_velocity * time_step, ForceMode.VelocityChange);
        // this.GetComponent<Rigidbody>().AddForce(acceleration, ForceMode.Acceleration);
    }

    public string toString(){
        string info = "";
        info = "mass = " + mass + "\n";
        info = "force = " + force + "\n";
        info = "force_direction = " + force_direction + "\n";
        info = "acceleration = " + acceleration + "\n";

        return info;
    }
}
