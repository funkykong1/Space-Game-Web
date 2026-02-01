
using System.Collections;
using UnityEngine;
using Dreamteck.Splines;




public class AmbientEnemy : EnemyBase
{

    // This enemy is one which will only do a drive by at the player
    // I couldnt come up with a better word than ambient for it
    public int index;
    //spline offset number, 0-1, set in EDITOR!!

    [Range(3, 15)]
    public int timer = 5;

    public bool randomize = true;

    LayerMask lm;

    SplineComputer sp;
    AmbientSpline asp;

    WaitForSeconds wfs;



    protected override void Awake()
    {
        base.Awake();
        lm = LayerMask.GetMask("Player");
        
    }

    protected override IEnumerator SplineCoroutine()
    {
        WaitForFixedUpdate wf = new WaitForFixedUpdate();
        while(true)
        {

            // make it inactive (wont shoot until it's on the map and visible)
            active = false;
            // This is damageable
            invulnerable = false;

            follower.clipFrom = offset;
            follower.spline = GetNextSpline();
            asp = follower.spline.GetComponent<AmbientSpline>();
            

            // Other planes in queue + this
            int j = asp.planes + 1;
            // add this to the queue
            asp.planes = j;

            // If none are in the queue yet then skip the wait
            if (asp.planes == 1)
            {
                // Spline must have enough time between planes to process the queue
                asp.SplineIndexer();
            }

            // Wait until our index matches the one in the spline, kind of like a queue
            yield return new WaitUntil(() => j == asp.planes);

            follower.enabled = true;

            //manually call rebuild as a catch all to prevent bugs
            follower.RebuildImmediate();

            // Active is called when the enemy enters the map hitbox
            yield return new WaitUntil(() => active);

            while (active)
            {
                if (Physics.Raycast(transform.position, transform.up, 30, lm))
                {
                    Shoot();
                    active = false;
                }
                yield return wf;

            }
            // wait until this reaches the end (becomes invulnerable)
            yield return new WaitUntil(() => invulnerable);
            // wait a small delay after
            yield return new WaitForSeconds(timer);
        }

    }

    // used by the spline follower script - needs to be public
    public override void SplineComplete()
    {
        //set 'Follow' to false to avoid auto updating the position back onto the spline
        invulnerable = true;
        follower.follow = false;
        active = false;
    }

    public void DestroyAmbient()
    {
        StartCoroutine(bugswatter());
    }
    private IEnumerator bugswatter()
    {
        //if its invulnerable then kill it, otherwise wait until it is
        yield return new WaitUntil(() => invulnerable);
        master.ambientEnemies.Remove(this);
        Destroy(gameObject);
    }

    // only used by ambient enemies
    public void Shoot()
    {
        for (int i = 0; i < barrels.Count; i++)
        {
            Instantiate(bullet, barrels[i].position, barrels[i].rotation);
        }
    }

    SplineComputer GetNextSpline()
    {
        // Get the spline from enemy master
        if (randomize)
        {
            sp = master.aSplines[Random.Range(0, master.aSplines.Length)];
        }
        else
        {
            index++;

            // caps at last ambient spline
            if(index > master.aSplines.Length-1)
                index = 0;

            sp = master.aSplines[index];
        }


        return sp;

    }
    
    protected override void EnemyDeath()
    {
        base.EnemyDeath();
        //remove this from enemy master's current list
        master.ambientEnemies.Remove(this);

        //make an explosion and rotate it randomly
        Instantiate(shipExplosion, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));

        Destroy(gameObject, 0.15f);
    }
}

