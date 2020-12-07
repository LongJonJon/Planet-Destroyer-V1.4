using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrbitingBody : MonoBehaviour {
    public enum Significance { Planet = 1, Moon = 2, Sun = 0 }
    public Significance type;
    public OrbitingBody originalOrbiting;
    public OrbitingBody orbiting;
    float distance;
    public float frequency;

    void Start () {
        OrbitingBody[] options = GetComponentsInParent<OrbitingBody> ();
        OrbitingBody parent = null;
        foreach (OrbitingBody p in options) {
            if (this.type > p.type) {
                parent = p;
                break;
            }
        }
        if (!parent) return;

        originalOrbiting = orbiting = parent;
        distance = Vector3.Distance (this.transform.position, parent.transform.position);
    }

    public void ResetOrbit () {
        if (type == Significance.Sun)
            this.transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
        else
            this.transform.SetParent (originalOrbiting.transform, true);
        orbiting = originalOrbiting;

    }

    public void ReflectOrbit (OrbitingBody o = null) {
        if (!o) {
            this.transform.SetParent (GameObject.FindObjectOfType<OrbitManager> ().SolarSystem.transform, true);
        } else {
            this.transform.SetParent (o.transform, true);
        }
        orbiting = o;

        if (originalOrbiting == null) return;
        originalOrbiting.ReflectOrbit (this);
    }

    public void UpdatePos (float time) {
        if (!orbiting) return;
        if (orbiting != originalOrbiting)
            Debug.DrawLine (this.transform.position, orbiting.transform.position, new Color (0, 255, 150));
        else
            Debug.DrawLine (this.transform.position, orbiting.transform.position, new Color (150, 255, 0));

        float x = 2 * Mathf.PI * time / ((originalOrbiting != orbiting) ? orbiting.frequency : frequency) + ((originalOrbiting != orbiting) ? Mathf.PI : 0);
        Vector3 newpos = new Vector3 (Mathf.Sin (x), 0, Mathf.Cos (x));
        this.transform.localPosition = newpos * ((originalOrbiting != orbiting) ? orbiting.distance : distance);
    }
}