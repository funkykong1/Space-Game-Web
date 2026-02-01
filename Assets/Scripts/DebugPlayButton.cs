using UnityEngine;

public class DebugPlayButton : MonoBehaviour
{

    void OnMouseDown()
    {
        FindAnyObjectByType<EnemyMaster>().StartLevel();
    }
}
