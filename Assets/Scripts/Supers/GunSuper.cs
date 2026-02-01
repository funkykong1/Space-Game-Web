using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

/**

Base for supers which shoot something
Uses animator and audiosource
haz to be a separate gameobject - child of the player!!
**/
public class GunSuper : WeaponBase, Super
{

    WaitForSeconds wf;

    [SerializeField]
    // use both of these to control rate of fire!!
    private float cooldownSeconds = 0.8f, elapsed = 0f, duration = 5f;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ApplySuper(float duration)
    {
        StartCoroutine(ShootSuper());
    }

    public void StopSuper()
    {
        StopCoroutine(ShootSuper());
        anim.SetTrigger("Reload");
        anim.speed = 0;
    }

    protected IEnumerator ShootSuper()
    {

        anim.speed = 1;
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
        Vector3 rot;
        while(elapsed < duration) // Loop until duration has passed
        {
            //create a bullet and apply rotation, tell animation and audio things to do stuff
            rot = transform.rotation.eulerAngles + new Vector3(0f,0f,Random.Range(-spread, spread));

            // shooot
            anim.SetTrigger("Fire");
            yield return new WaitForFixedUpdate();

            // this doesn't use animation events
            Instantiate(bullet, barrel.position, Quaternion.Euler(rot));

            yield return wf;

            elapsed += cooldownSeconds * cooldownMult;

        }
        elapsed = 0;
    }

    public override void ChangeFirerate(float cd)
    {
        base.ChangeFirerate(cd);
        // whenever cooldown changes, reconstruct this with new values :p
        wf = new WaitForSeconds(cooldownSeconds * cooldownMult);
    } 
}
