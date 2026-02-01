using UnityEngine;
using System.Collections;

public class BombArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // THIS IS a collider around the bomb which acts as a trigger for homing
        if(!other.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            return;

        if(enemy.invulnerable)
            return;

        Bomb bomba = GetComponentInParent<Bomb>();
        bomba.StartCoroutine(bomba.HomingTime(enemy));
        
    }
}
