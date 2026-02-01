using UnityEngine;


public class IceGun : WeaponBase
{

    [SerializeField]
    // This doesnt use animations
    private int cooldownSeconds;
    WaitForSeconds wf;
    float counter;
    
    protected override void FireBullet()
    {
        // Simultaneously instantiate and set its damage
        p = Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>();
        p.damage = damage;
        p.statusChance = statusChance;
    }

    void Update()
    {
        if(firing)
        {
            counter += Time.deltaTime;
            if(counter >= cooldownSeconds)
            {
                FireBullet();
                counter = 0;
            }
                
        }
    }
    // protected override IEnumerator ShootBurst()
    // {
    //     // construct at the beginning of loop
    //     wf = new WaitForSeconds(cooldownSeconds * cooldownMult);

    //     while(firing) // just make it loop over and over
    //     {
    //         FireBullet();
    //         yield return wf; // time between bursts
    //     }
    // }


    // runs base cd updater and reconstructs waitforseconds with new values
    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);
        // whenever cooldown changes, reconstruct this with new values :p
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
    } 
}
