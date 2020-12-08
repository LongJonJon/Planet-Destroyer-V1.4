using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class RelativeLineRender : MonoBehaviour {
    public int amount = 120;

    List<LineRenderer> lineRenderers = new List<LineRenderer> ();
    OrbitingBody[] focuses;
    OrbitingBody origin;
    OrbitManager manager;

    public void LateStart (OrbitingBody _center, OrbitingBody[] objects) {
        manager = GameObject.FindObjectOfType<OrbitManager> ();
        origin = _center;
        focuses = objects;

        // copy base line renderer
        LineRenderer thisLr = this.GetComponent<LineRenderer> ();
        UnityEditorInternal.ComponentUtility.CopyComponent (thisLr);
        // create the line renderers
        GameObject goLr;
        foreach (OrbitingBody o in objects) {
            goLr = new GameObject (o.name + " LineRender");
            goLr.transform.SetParent (this.transform, true);
            goLr.transform.localPosition = Vector3.zero;

            LineRenderer lr = goLr.AddComponent<LineRenderer> () as LineRenderer;
            UnityEditorInternal.ComponentUtility.PasteComponentValues (lr);
            lineRenderers.Add (lr);
            if (o.type == OrbitingBody.Significance.Sun) lr.enabled = false;
        }
        thisLr.enabled = false;

        // set up previous line renders
        Quaternion oldrot = manager.SolarSystem.transform.rotation;
        manager.SolarSystem.transform.rotation = new Quaternion ();
        this.transform.position = origin.transform.position;
        for (var i = -amount; i < 0; i++) {
            manager.TimeStep (i * Time.deltaTime);
            UpdatePos (true);
        }
        manager.SolarSystem.transform.rotation = oldrot;
    }

    public void UpdatePos (bool skip = false) {
        Quaternion oldrot = new Quaternion ();
        if (!skip) {
            oldrot = manager.SolarSystem.transform.rotation;
            manager.SolarSystem.transform.rotation = new Quaternion ();
            this.transform.position = origin.transform.position;
        }

        for (var o = 0; o < focuses.Length; o++) {
            if (focuses[o].type == OrbitingBody.Significance.Sun) continue;
            if (!focuses[o]) continue; // incase if planet was destroyed
            Vector3[] p = new Vector3[lineRenderers[o].positionCount];
            lineRenderers[o].GetPositions (p);

            Array.Resize (ref p, p.Length + 1);
            p[p.Length - 1] = focuses[o].transform.position - origin.transform.position;

            if (p.Length > amount) {
                Vector3[] _p = new Vector3[amount];
                Array.Copy (p, p.Length - amount, _p, 0, amount);
                p = _p;
            }

            lineRenderers[o].positionCount = p.Length;
            lineRenderers[o].SetPositions (p);
        }

        if (!skip)
            manager.SolarSystem.transform.rotation = oldrot;
    }
}