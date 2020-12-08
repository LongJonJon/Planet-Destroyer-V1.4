using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour {
    public GameObject SolarSystem;
    public float time = 0;
    OrbitingBody[] solarSystemObjects;
    private OrbitingBody focus;
    Vector3 priorloc = new Vector3 ();
    public float focus_Delta = 3;
    protected float _focusDelta;
    float focusDelta {
        get {
            return _focusDelta;
        }
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
                focus = o;
            }
            o.UpdatePos (0);
        }

        RelativeLineRender rlr = GameObject.FindObjectOfType<RelativeLineRender> ();
        if (rlr) rlr.LateStart (focus, solarSystemObjects);
    }

    void LateUpdate () {
        time += Time.deltaTime;

        // multithreading?
        RelativeLineRender rlr = GameObject.FindObjectOfType<RelativeLineRender> ();
        foreach (OrbitingBody o in solarSystemObjects) {
            if (o == focus)
                focus.transform.position = Vector3.Lerp (priorloc, SolarSystem.transform.position, focusDelta);
            else
                o.UpdatePos (time);
        }
        if (rlr) rlr.UpdatePos ();

        focusDelta = Time.deltaTime;
    }

    public void UpdateFocus (GameObject g) {
        focus = g.GetComponent<OrbitingBody> ();
        if (!focus) return;

        focusDelta = -1;
        priorloc = g.transform.position;

        foreach (OrbitingBody o in solarSystemObjects)
            o.transform.SetParent (SolarSystem.transform, true);

        foreach (OrbitingBody o in solarSystemObjects)
            o.ResetOrbit ();

        focus.ReflectOrbit ();
    }

    public void TimeStep (float _time) {
        foreach (OrbitingBody o in solarSystemObjects)
            o.UpdatePos (_time, true);
    }
}