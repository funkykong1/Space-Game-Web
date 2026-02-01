using UnityEngine;
using System.Collections;
using PrimeTween;
public class CameraThing : MonoBehaviour
{
    Tween tween;

    [SerializeField]
    bool cameraMoving = false;
    [SerializeField]
    [Range(10, 40)]float cameraSpeed=10;
    public void MoveCamera(Transform dest)
    {
        if(cameraMoving)
            return;
        StartCoroutine(internalcameramove(dest));
    }
    // cant call coroutines from unity events 
    public IEnumerator internalcameramove(Transform destination)
    {
        cameraMoving = true;
        Vector3 dest = new(destination.position.x, destination.position.y, transform.position.z);
        //temporarily make the camera a bit slower while tweening it
        cameraSpeed -= 2;
        tween = Tween.PositionAtSpeed(transform,dest,cameraSpeed, Ease.InOutSine).OnComplete(() => cameraMoving = false);
        yield return new WaitForSeconds(0.3f);
        cameraSpeed += 2;
    }
}
