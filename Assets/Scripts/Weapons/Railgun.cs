using UnityEngine;
using System.Collections;

// This has to be its own script for it to work with the animator module
public class Railgun : AnimatedWeapon
{

    // summon bullet
    public void EVENT_FIRE()
    {
        // Simultaneously instantiate and set its damage
        Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>().damage = damage;
    }

}
