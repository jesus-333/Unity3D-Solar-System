using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyV2 : MonoBehaviour
{
    public int n_points;


    public Vector3 initial_velocity;
    private Vector3 current_velocity;
    private Vector3 force, force_direction;
    public FixedSizedQueue<Vector3> orbit_points;

    private Rigidbody rb_body;

    public static  List<CelestialBodyV2> list_of_bodies;

    public bool debug_var = false;


    void Awake(){
        current_velocity = initial_velocity;
        rb_body = this.GetComponent<Rigidbody>();

         orbit_points = new FixedSizedQueue<Vector3>(n_points);
    }

    void OnEnable(){
        if(list_of_bodies == null) list_of_bodies = new List<CelestialBodyV2>();

        list_of_bodies.Add(this);
    }

    void OnDisable(){
        list_of_bodies.Remove(this);
    }

    public void FixedUpdate(){
        foreach (CelestialBodyV2 body in list_of_bodies){
            if(body != this){
                AttractBody(body);
            }
        }
    }


    public void AttractBody(CelestialBodyV2 other_body){
        Rigidbody rb_other_body = other_body.GetComponent<Rigidbody>();

        force_direction = rb_body.position - rb_other_body.position;
        float distance = force_direction.magnitude;

        float force_magnitude = rb_body.mass * rb_other_body.mass;
        force = force_direction.normalized * force_magnitude;

        rb_other_body.AddForce(force);
    }


}
