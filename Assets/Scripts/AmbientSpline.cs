using UnityEngine;
using System.Collections;

public class AmbientSpline : MonoBehaviour
{
    public int planes;
    private int planeIndex;
    public float timer = 1.5f;

    bool indexing;

    public void SplineIndexer()
    {
        if (indexing)
            return;
            
        indexing = true;
        StartCoroutine(cr());
    }

    //todo completely redo this lol

    // Whenever a plane selects this spline, start a coroutine which adds to a timer and then waits
    // First plane comes after 1 step (0.x seconds), second after 2 steps ... etc
    // planes wait until int i matches their index 
    private IEnumerator cr()
    {
        // Iterate through all the planes in queue
        for (planeIndex = 0; planeIndex < planes; planeIndex++)
        {
            yield return new WaitForSeconds(timer);
            planeIndex++;
        }
        planes = 0;
    }
}
