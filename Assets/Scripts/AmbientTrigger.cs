using UnityEngine;

public class AmbientTrigger : MonoBehaviour
{
    // Ambient enemies only shoot at the player while in the map
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out AmbientEnemy enemy))
            return;

        enemy.active = true;
    }
}
