using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [ExecuteInEditMode]
public class OrbitVisualizer : MonoBehaviour
{
    public int n_points = 1000;

    [Range(0.01f, 1f)]
    public float width = 0.5f;

    void Update () {
        drawOrbit();
    }

    void drawOrbit(){
        // List with all the bodies in their current position
        CelestialBody[] list_of_bodies = FindObjectsOfType<CelestialBody> ();

        // List of bodies to simulate motion
        SimulateBody[] sim_bodies = new SimulateBody[list_of_bodies.Length];

        // Point to draw the orbit for each body
        Vector3[][] points_to_draw = new Vector3[list_of_bodies.Length][];

        // Inizialization
        for(int i = 0; i < list_of_bodies.Length; i++){
            // Copy of the bodies for simulation (i.e. don't move original body)
            sim_bodies[i] = new SimulateBody(list_of_bodies[i]);
            // print(sim_bodies[i].toString());

            // For each body create an array of points to draw
            points_to_draw[i] = new Vector3[n_points];
        }

        // Simulate the orbits for n_points
        points_to_draw = simulateOrbitForNSteps(sim_bodies, points_to_draw);

        // Draw the orbit for each body
        for(int i = 0; i < list_of_bodies.Length; i++){
            LineRenderer tmp_lineRenderer = list_of_bodies[i].GetComponent<LineRenderer>();

            tmp_lineRenderer.positionCount = points_to_draw[i].Length;
            tmp_lineRenderer.SetPositions (points_to_draw[i]);
            tmp_lineRenderer.widthMultiplier = width;
        }
    }


    // Given the 2D array (already initialized) simulate the orbit of bodies in the array sim_bodies for n steps with n = n_points
    Vector3[][] simulateOrbitForNSteps(SimulateBody[] sim_bodies, Vector3[][] points_to_draw){
        // Simulate n_points step
        for(int j = 0; j < n_points; j++){

            // Update velocity of the simulation bodies
            for (int i = 0; i < sim_bodies.Length; i++){ sim_bodies[i].UpdateVelocity(sim_bodies, UniverseConstants.physics_time_step);}

            // Update each position based on the actual velocity
            for (int i = 0; i < sim_bodies.Length; i++){
                // Update position
                sim_bodies[i].UpdatePosition(UniverseConstants.physics_time_step);

                // Save new position
                points_to_draw[i][j] = sim_bodies[i].position;
            }
        }

        return points_to_draw;
    }
}

class SimulateBody {
    public Vector3 position;
    public Vector3 current_velocity;
    public float mass;

    public SimulateBody (CelestialBody body) {
        position = body.transform.position;
        current_velocity = body.initial_velocity;
        mass = body.mass;
    }

    // Same code of CelestialBody
    public void UpdateVelocity(SimulateBody[] list_of_bodies, float time_step){
        Vector3 force, force_direction, acceleration;
        for(int i = 0; i < list_of_bodies.Length; i++){
            SimulateBody other_body = list_of_bodies[i];
            if(other_body != this){
                float square_distance = (other_body.position - this.position).sqrMagnitude;
                force_direction = (other_body.position - this.position).normalized;
                force = force_direction * UniverseConstants.gravitational_constant * mass * other_body.mass / square_distance;
                acceleration = force / mass;
                current_velocity += acceleration * time_step;
            }
        }
    }

    // Same code of CelestialBody
    public void UpdatePosition(float time_step) {
        this.position += current_velocity * time_step;
    }
}
