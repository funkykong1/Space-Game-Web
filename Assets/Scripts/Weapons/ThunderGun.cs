using UnityEngine;


// Has to be its own script because it won't work with anim events otherwise
public class ThunderGun : AnimatedWeapon
{
    public void EVENT_FIRE()
    {
        // Simultaneously instantiate and set its damage
        p = Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>();
        p.damage = damage;
        p.statusChance = statusChance;
    }
}
