using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingBody : MonoBehaviour {
    public enum Significance { Sun = 0, Planet = 1, Moon = 2 }
    public Significance type;
    public float frequency;
    public GameObject moonOrbit;
    public Transform originalOrbiting;
    public Transform orbiting;
    float distance;
    public float offset;

    void LateStart () {
        OrbitingBody[] options = GetComponentsInParent<OrbitingBody> ();
        OrbitingBody parent = null;
        foreach (OrbitingBody p in options)
            if (this.type > p.type) {
                parent = p;
                break;
            }

        if (!parent) return;

        if (type == Significance.Moon) {
            originalOrbiting = orbiting = parent.MoonsObject (this);
            this.transform.SetParent (originalOrbiting, true);
            this.transform.localRotation = new Quaternion ();
        } else {
            originalOrbiting = orbiting = parent.transform;
        }

        distance = Vector3.Distance (this.transform.position, parent.transform.position);
    }

    public Transform MoonsObject (OrbitingBody c) {
        if (!moonOrbit) {
            moonOrbit = new GameObject (this.name + "'s Moon Orbit");
            moonOrbit.transform.SetParent (this.transform);
            moonOrbit.transform.localPosition = new Vector3 ();
            moonOrbit.transform.localRotation = this.transform.localRotation;
            MoonOrbit mo = moonOrbit.AddComponent<MoonOrbit> () as MoonOrbit;
            mo.Setup (this, this.transform.localRotation);
            mo.AddChild (c);
            this.transform.localRotation = new Quaternion ();
        } else moonOrbit.GetComponent<MoonOrbit> ().AddChild (c);
        return moonOrbit.transform;
    }

    public void ResetOrbit () {
        switch (type) {
            case Significance.Sun:
                this.transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                break;
            case Significance.Planet:
                this.transform.SetParent (originalOrbiting, true);
                break;
            case Significance.Moon:
                // done in moon orbit sript
                break;
        }

        if (moonOrbit) moonOrbit.GetComponent<MoonOrbit> ().ResetOrbit ();

        orbiting = originalOrbiting;
    }

    public void ReflectOrbit (OrbitingBody o = null) {
        switch (type) {
            case Significance.Moon:
                MoonOrbit mo = originalOrbiting.GetComponent<MoonOrbit> ();
                this.transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                orbiting = null;
                this.transform.localRotation = mo.oRot;
                mo.ReflectOrbit (this);
                break;
            default:
                if (!o) {
                    this.transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
                    orbiting = null;
                } else {
                    this.transform.SetParent (o.transform, true);
                    orbiting = o.transform;
                }

                if (originalOrbiting)
                    originalOrbiting.GetComponent<OrbitingBody> ().ReflectOrbit (this);
                break;
        }
    }

    public void UpdatePos (float time, bool setup = false) {
        float x;
        if (setup) LateStart ();
        if (!orbiting) {
            if (type != Significance.Sun && originalOrbiting) {
                x = 2 * Mathf.PI * time / frequency + offset;
                Debug.DrawLine (originalOrbiting.transform.position, originalOrbiting.transform.position + new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x)) * distance, new Color (150, 255, 0));
            }
            return;
        }

        /* Debugging
        if (orbiting != originalOrbiting)
            Debug.DrawLine (this.transform.position, orbiting.position, new Color (0, 255, 150));
        else if (orbiting)
            Debug.DrawLine (this.transform.position, orbiting.position, new Color (150, 255, 0));
        */
        
        if (originalOrbiting != orbiting) {
            x = 2 * Mathf.PI * time / orbiting.GetComponent<OrbitingBody> ().frequency + Mathf.PI + orbiting.GetComponent<OrbitingBody> ().offset;
            this.transform.localPosition = new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x)) * orbiting.GetComponent<OrbitingBody> ().distance;
        } else {
            x = 2 * Mathf.PI * time / frequency + offset;
            this.transform.localPosition = new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x)) * distance;
        }
    }
}