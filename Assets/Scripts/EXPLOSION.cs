using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]


// gameobject with this is a throwaway one shot which destroys itself fast
public class EXPLOSION : MonoBehaviour
{
    public float delay = 2f;

    void Start()
    {
        Destroy(gameObject,delay);
    }

    // better to just tween between 1 and 0 opacity lol (within animator)
    public void DestroyThis()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, delay);
    }


}
