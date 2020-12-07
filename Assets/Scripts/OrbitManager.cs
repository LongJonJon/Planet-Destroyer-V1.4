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
            if (_focusDelta > focus_Delta) _focusDelta = focus_Delta;
            else _focusDelta += value;
        }
    }

    void Start () {
        solarSystemObjects = GameObject.FindObjectsOfType<OrbitingBody> ();
        foreach (OrbitingBody o in solarSystemObjects) {
            if (o.type == OrbitingBody.Significance.Sun) {
                priorloc = o.transform.position;
                focus = o;
                break;
            }
        }
    }

    void LateUpdate () {
        time += Time.deltaTime;
        foreach (OrbitingBody o in solarSystemObjects) {
            if (o == focus)
                focus.transform.position = Vector3.Lerp (priorloc, SolarSystem.transform.position, _focusDelta);
            else
                o.UpdatePos (time);
        }
        focusDelta = Time.deltaTime;
    }

    public void UpdateFocus (GameObject g) {
        Debug.Log(g.name);
        focus = g.GetComponent<OrbitingBody> ();
        if (!focus) return;

        _focusDelta = 0;
        priorloc = g.transform.position;

        foreach (OrbitingBody o in solarSystemObjects)
            o.transform.SetParent (SolarSystem.transform, true);
            
        foreach (OrbitingBody o in solarSystemObjects)
            o.ResetOrbit ();

        focus.ReflectOrbit ();
    }
}