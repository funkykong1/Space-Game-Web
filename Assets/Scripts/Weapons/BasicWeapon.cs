using System.Collections;
using UnityEngine;


// Basic weapon without burst feature - uses animator and audiosource
// differs from other weapons in that it doesnt have a charge anim
public class BasicWeapon : WeaponBase
{
    // wizard magic o_O
    private static readonly WaitForSeconds wfs = new(0.5f);
    private static readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    protected Animator anim;
    protected AudioSource source;
    WaitForSeconds wf;
    [SerializeField]
    private float cooldownSeconds = 1.5f;
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    public void FIRE_BULLET_ANIMATOR()
    {
        //create a bullet and apply rotation, tell animation and audio things to do stuff
        source.Play();

        p = Instantiate(bullet, barrel.position, Quaternion.Euler(rot)).GetComponent<PlayerBullet>();
        p.damage = damage;
    }


    protected override IEnumerator ShootBurst()
    {
        yield return wfs;
        print("avtocannon coroutine called");

        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);

        while(firing) // just make it loop over and over
        {
            //create a bullet and apply rotation, tell animation and audio things to do stuff
            rot = transform.rotation.eulerAngles + new Vector3(0f,0f,Random.Range(-spread, spread));
            anim.SetTrigger("Fire");

            // timing between bullet animation and audio + spawning here
            yield return _waitForFixedUpdate;
            FireBullet();
            yield return wf; // time between bursts
        }
    }


    // runs base cd updater and reconstructs waitforseconds with new values
    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);
        anim.SetFloat("Speed", cooldownMult);
        // whenever cooldown changes, reconstruct this with new values :p
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
    } 
}

    

