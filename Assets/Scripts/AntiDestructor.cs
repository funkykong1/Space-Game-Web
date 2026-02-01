using UnityEngine;


// Dont destroy the object with this script
public class AntiDestructor : MonoBehaviour
{

    private static GameObject instance;
    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
}
