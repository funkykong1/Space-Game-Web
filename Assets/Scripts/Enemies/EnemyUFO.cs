using UnityEngine;



public class EnemyUFO : MonoBehaviour
{

    GameObject ring;


    void Awake()
    {
        ring = transform.Find("Ufo Ring").gameObject;
    }
    void FixedUpdate()
    {
        ring.transform.Rotate(new Vector3(0,0,1));
    }
}
