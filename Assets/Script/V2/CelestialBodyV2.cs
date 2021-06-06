using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CelestialBodyV2 : MonoBehaviour
{
    public static float width = 0.2f;

    private int tmp_index = 0;

    public Vector3 initial_velocity;
    private Vector3 force, force_direction, velocity;
    public FixedSizedQueue<Vector3> orbit_points_queue;
    private Vector3[] orbit_points_vector;

    private Rigidbody rb_body;

    private LineRenderer lr;

    public static  List<CelestialBodyV2> list_of_bodies;

    public bool debug_var = false, use_velocity = false;
    private bool use_queue_to_track_orbit = false;


    void Awake(){
        // Find the Rigidbody attached to this object and set its initial velocity
        rb_body = this.GetComponent<Rigidbody>();
        rb_body.AddForce(initial_velocity, ForceMode.VelocityChange);

        // Find the LineRenderer attached to this object and set a random color for it
        this.GetComponent<LineRenderer>().enabled = true;
        lr = this.GetComponent<LineRenderer>();
        setRandomColorLineRenderer();

        // Check if used queue or array to track the orbit
        use_queue_to_track_orbit = UniverseConstants.use_queue_to_track_orbit;

        // Declaration of queue or array used to draw the orbit
        if(use_queue_to_track_orbit) orbit_points_queue = new FixedSizedQueue<Vector3>(UniverseConstants.n_points);
        else orbit_points_vector = new Vector3[1000];
    }

    // Function to assign a random color to the line renderer
    private void setRandomColorLineRenderer(){
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        Color tmp_color =  new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0);
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(tmp_color, 1.0f), new GradientColorKey(tmp_color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.5f), new GradientAlphaKey(alpha, 0.5f) }
        );
        lr.colorGradient = gradient;
    }

    // Add the object to the list of body when it is created
    void OnEnable(){
        if(list_of_bodies == null) list_of_bodies = new List<CelestialBodyV2>();

        list_of_bodies.Add(this);
    }

    // Remove the object to the list of body when it is created
    void OnDisable(){
        list_of_bodies.Remove(this);
    }

    public void FixedUpdate(){
        // Update orbit
        foreach (CelestialBodyV2 body in list_of_bodies){
            if(body != this){
                AttractBody(body);
            }
        }

        // Save new position (used for drawin orbit)
        if(use_queue_to_track_orbit){
            orbit_points_queue.Enqueue(this.transform.position);
        } else {
            if(tmp_index < UniverseConstants.n_points){
                if(tmp_index == -1){tmp_index = 0;}
                orbit_points_vector[tmp_index] = this.transform.position;
                tmp_index++;
            } else if (tmp_index == UniverseConstants.n_points)  {
                Array.Copy(orbit_points_vector, 1, orbit_points_vector, 0, orbit_points_vector.Length - 1);
                orbit_points_vector[tmp_index - 1] = this.transform.position;;
            } else if (tmp_index > UniverseConstants.n_points){
                tmp_index = UniverseConstants.n_points - 1;
                Array.Copy(orbit_points_vector, 0, orbit_points_vector, 0, UniverseConstants.n_points);
            }
        }
    }

    public void Update(){
        // Draw orbit (past position)
        if(use_queue_to_track_orbit){ // Usign queue
            lr.positionCount = orbit_points_queue.Count;
            lr.SetPositions(orbit_points_queue.convertToArray());
        } else { // Usign array
            if(tmp_index == -1){ lr.positionCount = 0;}
            else { lr.positionCount = tmp_index;}
            lr.SetPositions(orbit_points_vector);
        }
        lr.widthMultiplier = width;
    }

    public void AttractBody(CelestialBodyV2 other_body){
        // Evalute how much this body attract the other_body

        // Obtain Rigidbody of the other_body
        Rigidbody rb_other_body = other_body.GetComponent<Rigidbody>();

        // Evaluate force direction and distance between the two body
        force_direction = rb_body.position - rb_other_body.position;
        float distance = force_direction.magnitude;

        // Evalute force applied to other_body
        float force_magnitude = rb_body.mass * rb_other_body.mass;
        force = force_direction.normalized * force_magnitude;


        if(other_body.use_velocity){
            // Evaluate change in velocity and applied
            velocity = (force / rb_other_body.mass) * UniverseConstants.physics_time_step;
            rb_other_body.AddForce(velocity, ForceMode.VelocityChange);
        } else {
            // Apply force (DEFAULT OPTION)
            rb_other_body.AddForce(force);
        }

    }
}
