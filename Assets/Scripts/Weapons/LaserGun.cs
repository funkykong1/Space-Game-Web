using UnityEngine;


// Has to be its own script because it won't work with anim events otherwise
public class LaserGun : AnimatedWeapon
{
    int bulletsFired = 0;
    [SerializeField]
    int burstCount = 3;
    
    public void EVENT_FIRE()
    {
        bulletsFired += 1;
        if(bulletsFired >= burstCount)
            anim.SetTrigger("Reload");

        // Simultaneously instantiate and set its damage
        rot = transform.rotation.eulerAngles + new Vector3(0,0,Random.Range(-spread, spread));
        p = Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>();
        p.damage = damage;
        p.statusChance = statusChance;

    }

    public void EVENT_RELOAD()
    {
        bulletsFired = 0;
    }

    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);

        // this gains more bullets when its powered up !!!
        if(cd > 0)
            burstCount++;
        else
            burstCount--;

        anim.SetFloat("Speed", cooldownMult);
    }

}