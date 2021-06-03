using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyV2 : MonoBehaviour
{
    public static int n_points = 250;
    public int n_points_orbit = 250;

    [Range(0.01f, 0.1f)]
    public float width = 0.05f;

    private Vector3 force, force_direction;
    public FixedSizedQueue<Vector3> orbit_points;

    private Rigidbody rb_body;

    private LineRenderer lr;

    public static  List<CelestialBodyV2> list_of_bodies;

    public bool debug_var = false;


    void Awake(){
        rb_body = this.GetComponent<Rigidbody>();

        this.GetComponent<LineRenderer>().enabled = true;
        lr = this.GetComponent<LineRenderer>();
        setRandomColorLineRenderer();

        orbit_points = new FixedSizedQueue<Vector3>(n_points);
    }

    private void setRandomColorLineRenderer(){
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        Color tmp_color =  new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0);
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(tmp_color, 1.0f), new GradientColorKey(tmp_color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.5f), new GradientAlphaKey(alpha, 0.5f) }
        );
        lr.colorGradient = gradient;
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

        orbit_points.Enqueue(this.transform.position);
    }

    public void Update(){
        lr.positionCount = orbit_points.Count;
        lr.SetPositions(orbit_points.convertToArray());
        lr.widthMultiplier = width;

        // print("orbit_points.Count = " + orbit_points.Count);
        n_points = n_points_orbit;
        orbit_points.Size = n_points;
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
