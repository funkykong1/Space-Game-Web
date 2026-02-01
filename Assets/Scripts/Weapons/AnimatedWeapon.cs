using UnityEngine;
using System.Collections;


// BASE class for all guns with animator. Each one of them has to be its own script with atleast an animation event fire thing
public class AnimatedWeapon : WeaponBase
{
    protected Animator anim;

    
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        barrel = transform.GetChild(0);
    }


    // doesnt work with animation events
    protected override void FireBullet(){}


    public override void ToggleFiring(bool toggle)
    {
        firing = toggle;

        StartCoroutine(ShootBurst());
    }

    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);
        anim.SetFloat("Speed", cooldownMult);
    }

    // recharge always plays. Transition to firing only goes through if firing is tru
    protected override IEnumerator ShootBurst()
    {
        // this gives a bit of breathing room for scene transitions and switching on and off firing
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Firing", firing);


    }
}
