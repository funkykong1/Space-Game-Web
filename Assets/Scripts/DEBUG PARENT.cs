using UnityEngine;

public class DEBUGPARENT : MonoBehaviour
{


    // Automatically disable debug stuff when level is started via main menu
    void Awake()
    {
        if (FindAnyObjectByType<GameManager>())
        {
            gameObject.SetActive(false);
        }
    }
}
