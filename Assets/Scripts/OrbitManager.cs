using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour {
    public bool paused = false;
    public bool lock2Sun = false;
    public GameObject SolarSystem;
    float time = 0;
    OrbitingBody[] solarSystemObjects;
    OrbitingBody sun;
    public Vector3 sundelta;
    OrbitingBody focus;
    Vector3 priorloc = new Vector3 ();
    public float focus_Delta = 3;
    protected float _focusDelta;
    float focusDelta {
        get { return _focusDelta; }
        set {
            if (value < 0) _focusDelta = 0; // reset
            else if (_focusDelta > focus_Delta) _focusDelta = focus_Delta; // max
            else _focusDelta += value; // add
        }
    }

    void Start () {
        solarSystemObjects = GameObject.FindObjectsOfType<OrbitingBody> ();
        foreach (OrbitingBody o in solarSystemObjects) {
            o.offset = Random.Range (0, o.frequency);
            if (!focus && o.type == OrbitingBody.Significance.Sun) {
                priorloc = o.transform.position;
                sun = focus = o;
            }
            o.UpdatePos (0, true);
        }

        RelativeLineRender rlr = GameObject.FindObjectOfType<RelativeLineRender> ();
        if (rlr) rlr.LateStart (focus, solarSystemObjects);
        Debug.Log ("Time: " + Time.realtimeSinceStartup);
    }

    void LateUpdate () {
        if (paused) return;
        time += Time.deltaTime;

        // multithreading?
        RelativeLineRender rlr = GameObject.FindObjectOfType<RelativeLineRender> ();
        foreach (OrbitingBody o in solarSystemObjects) {
            if (o != focus)
                o.UpdatePos (time);
        }
        if (rlr) rlr.UpdatePos ();

        focusDelta = Time.deltaTime;
        if (lock2Sun) Lock2Sun ();
        else sundelta = sun.transform.position - SolarSystem.transform.position;

        // fix fittering that occurs with this
        focus.transform.position = Vector3.Lerp (focus.transform.position, SolarSystem.transform.position, focusDelta);
    }

    void Lock2Sun () {
        Vector3 oldpos = SolarSystem.transform.position;
        SolarSystem.transform.rotation = Quaternion.identity;
        SolarSystem.transform.position = Vector3.zero;
        SolarSystem.transform.eulerAngles = Vector3.up * (Quaternion.LookRotation (sundelta).eulerAngles.y - Quaternion.LookRotation (sun.transform.position).eulerAngles.y);
        SolarSystem.transform.position = oldpos;
    }

    public void UpdateFocus (GameObject g) {
        focus = g.GetComponent<OrbitingBody> ();
        if (!focus) return;
        if (focus.type == OrbitingBody.Significance.Sun) lock2Sun = false;
        else lock2Sun = true;
        focusDelta = -1;
        priorloc = g.transform.position;
        sundelta = sun.transform.position - g.transform.position;
        foreach (OrbitingBody o in solarSystemObjects)
            o.transform.SetParent (SolarSystem.transform, true);

        foreach (OrbitingBody o in solarSystemObjects)
            o.ResetOrbit ();

        focus.ReflectOrbit ();
    }

    public void TimeStep (float _time) {
        foreach (OrbitingBody o in solarSystemObjects)
            o.UpdatePos (_time + time);
    }
}