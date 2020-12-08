using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonOrbit : MonoBehaviour {
    public OrbitingBody parent;
    public List<OrbitingBody> children = new List<OrbitingBody> ();
    public Quaternion oRot;
    public void Setup (OrbitingBody _parent, Quaternion _oRot) {
        parent = _parent;
        oRot = _oRot;
    }

    public void AddChild (OrbitingBody c) => children.Add (c);

    public void ResetOrbit () {
        this.transform.localRotation = oRot;
        this.transform.localPosition = Vector3.zero;

        foreach (OrbitingBody c in children) {
            c.transform.SetParent (this.transform, true);
            c.transform.localRotation = new Quaternion ();
        }
    }

    public void ReflectOrbit (OrbitingBody o) {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = oRot;
        parent.ReflectOrbit (o);
    }
}