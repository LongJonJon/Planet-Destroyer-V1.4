using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingBody : MonoBehaviour {
    public enum Significance { Sun = 0, Planet = 1, Moon = 2 }
    public Significance type;
    public float frequency;
    float _radius;
    public float radius { get { return _radius; } }
    public GameObject moonOrbit;
    public Transform originalOrbiting;
    public Transform orbiting;
    float distance;
    public float offset;

    void LateStart () {
        OrbitingBody[] options = GetComponentsInParent<OrbitingBody> ();
        OrbitingBody parent = null;
        foreach (OrbitingBody p in options)
            if (type > p.type) {
                parent = p;
                break;
            }

        if (!parent) return;

        if (type == Significance.Moon) {
            originalOrbiting = orbiting = parent.MoonsObject (this);
            transform.SetParent (originalOrbiting, true);
            transform.localRotation = new Quaternion ();
        } else {
            originalOrbiting = orbiting = parent.transform;
        }

        distance = Vector3.Distance (transform.position, parent.transform.position);
        _radius = GetComponent<SphereCollider> ().radius;
    }

    public Transform MoonsObject (OrbitingBody c) {
        if (!moonOrbit) {
            moonOrbit = new GameObject (name + "'s Moon Orbit");
            moonOrbit.transform.SetParent (transform);
            moonOrbit.transform.localPosition = new Vector3 ();
            moonOrbit.transform.localRotation = transform.localRotation;
            MoonOrbit mo = moonOrbit.AddComponent<MoonOrbit> () as MoonOrbit;
            mo.Setup (this, transform.localRotation);
            mo.AddChild (c);
            transform.localRotation = new Quaternion ();
        } else moonOrbit.GetComponent<MoonOrbit> ().AddChild (c);
        return moonOrbit.transform;
    }

    public void ResetOrbit () {
        switch (type) {
            case Significance.Sun:
                transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                break;
            case Significance.Planet:
                transform.SetParent (originalOrbiting, true);
                break;
        }

        if (moonOrbit) moonOrbit.GetComponent<MoonOrbit> ().ResetOrbit ();
        orbiting = originalOrbiting;
    }

    public void ReflectOrbit (OrbitingBody o = null) {
        orbiting = null;
        switch (type) {
            case Significance.Moon:
                MoonOrbit mo = originalOrbiting.GetComponent<MoonOrbit> ();
                transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                transform.localRotation = mo.oRot;
                mo.ReflectOrbit (this);
                break;
            default:
                if (!o) transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                else {
                    transform.SetParent (o.transform, true);
                    orbiting = o.transform;
                }

                if (originalOrbiting)
                    originalOrbiting.GetComponent<OrbitingBody> ().ReflectOrbit (this);
                break;
        }
    }

    public void UpdatePos (float time, bool setup = false) {
        if (setup) LateStart ();
        if (!orbiting) return;

        /* Debugging
        if (orbiting != originalOrbiting)
            Debug.DrawLine (transform.position, orbiting.position, new Color (0, 255, 150));
        else if (orbiting)
            Debug.DrawLine (transform.position, orbiting.position, new Color (150, 255, 0));
        */

        float x;
        if (originalOrbiting != orbiting) {
            OrbitingBody o = orbiting.GetComponent<OrbitingBody> ();
            x = 2 * Mathf.PI * time / o.frequency + o.offset + Mathf.PI;
            transform.localPosition = new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x)) * o.distance;
        } else {
            x = 2 * Mathf.PI * time / frequency + offset;
            transform.localPosition = new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x)) * distance;
        }
    }
}