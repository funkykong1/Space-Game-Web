using UnityEngine;

public class LevelEnemyTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // this murks bullets (no timer needed)
        // only works with trigger collider users
        if(other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            return;
        }
        // ambient enemies use a separate system
        if(other.TryGetComponent(out AmbientEnemy amb))
            return;

        if (!other.TryGetComponent(out EnemyBase enemy))
            return;

        enemy.invulnerable = false;
        
        // Detonators can be safely destroyed
        if (other.TryGetComponent(out EnemyDetonator det))
            if (det.transform.position.y < 0)
                Destroy(det.gameObject);
    }
}
