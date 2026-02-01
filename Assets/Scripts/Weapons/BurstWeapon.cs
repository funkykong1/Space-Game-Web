
using System.Collections;
using UnityEngine;


// Basic weapon with burst - audio and animator used by gunshot which this spawns
public class BurstWeapon : WeaponBase
{
    [Header("More manual stuff")]
    public GameObject gunShot;
    // burst weapon bullet indexer // how many bullets in a single burst
    private int burstBullets = 0; public int burstCount = 12;
    // time between shots
    public float fireRate;
    [SerializeField]
    private float cooldownSeconds = 1.5f;
    WaitForSeconds wf;
    


    protected override void Awake()
    {
        base.Awake();
        burstBullets = 0;
    }


    protected override void FireBullet()
    {
        //Create bullet spread by creating and modifying euler angles and apply those to the bullet
        rot = transform.rotation.eulerAngles + new Vector3(0,0,Random.Range(-spread, spread));

        // Gun will either have a separate gunshot prefab or built in animation in the sprite sheet
        Instantiate(gunShot, barrel, false); 
        p = Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>();
        p.damage = damage;
    
    }

    //use a coroutine to handle shooting
    protected override IEnumerator ShootBurst()
    {
        yield return new WaitForSeconds(0.5f);


        // construct this at the start of the loop
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
        
        while(firing) // just make it loop over and over
        {
            yield return wf; // time between bursts
            for(burstBullets = 0; burstBullets < burstCount; burstBullets++)
            {
                FireBullet();
                yield return new WaitForSeconds(fireRate); // time between shots
            }
        }
        
    }
    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);
        // this gains more bullets when its powered up !!!
        if(cd > 0)
            burstCount += 2;
        else
            burstCount -= 2;

        // whenever cooldown changes, reconstruct this with new values :p
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
    } 
}
