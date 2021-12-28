using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    private List<Transform> rings = new List<Transform>();

    public Material activeRing, inactiveRing, finalRing;

    private int ringPassed = 0;

    private void Start()
    {
        // At the start of the level, assing inactive to all rings
        foreach(Transform t in transform)
        {
            rings.Add(t);
            t.GetComponent<MeshRenderer>().material = inactiveRing;
        }

        // Making sure we're not stupid
        if (rings.Count == 0)
        {
            Debug.Log("there is no objectives assigned on this level, make sure you put some rings under the Objective Object");
            return;
        }

        // Activate the first ring
        rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
    }


    public void NextRing()
    {
        // Pla FX on the current ring
        // ??

        // up the int
        ringPassed++;

        // If it is the final ring, let's call the Victory
        if (ringPassed == rings.Count)
        {
            Victory();
            return;
        }

        // if this is the second last, give the next ring the "Final Ring" material
        if (ringPassed == rings.Count - 1)
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = finalRing;
        }
        else
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        }

        // In both cases, we need to activate the ring!
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
    }

    private void Victory()
    {
        FindObjectOfType<GameScene>().CompleteLevel();
    }

}
