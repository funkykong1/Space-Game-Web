using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RailgunBullet : PlayerBullet

// INSTANT, piercing ray of light
// Creates a particle effect at every entry and exit point of each hit enemy
{

    RaycastHit[] hits;

    RaycastHit[] exitHits;
    [SerializeField]
    List<Transform> hitList = new();
    EnemyBase enemy;

    void Awake()
    {
        AudioSource src = GetComponent<AudioSource>();

        // pitch randomization
        float rnd = Random.Range(-0.1f, 0.01f);
        src.pitch += rnd;

        // slight delay
        src.PlayDelayed(rnd*3);
    }
    // animation event
    public void RAILGUN_SHOT_DISAPPEARING_ACT()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }


    protected override IEnumerator BulletCoroutine()
    {
        // get all the raycast hits
        hits = Physics.RaycastAll(transform.position, transform.up, 35, LayerMask.GetMask("Enemy"));
        
        if(hits.Length > 0)
        {
            
            RaycastHit hit;

            // todo dick around with this if we ever make the make the playing field variable
            //Vector3 dir = new Vector3();

            // iterate through every hit enemy
            for (int i = 0; i < hits.Length; i++)
            {
                
                hit = hits[i];

                // hit the enemy only once
                if(hitList.Contains(hit.transform))
                    continue;
                
                hitList.Add(hit.transform);

                

                enemy = hit.transform.GetComponent<EnemyBase>();

                Instantiate(explosion, hit.point, Quaternion.identity);

                // create a piercing beam behind the hit object which creates an exit particle effect
                // its just a beam which spawns behind the hit enemy and points downwards
                exitHits = Physics.RaycastAll(new Vector3(hit.point.x, hit.point.y + 5, hit.point.z), Vector3.down, 6);

                // Iterate through all enemies hit by the 'exit beam'
                for (int j = 0; j < exitHits.Length; j++)
                {
                    // ignore everything except the enemy in question, create a cool particle effect at the point of exit
                    if(exitHits[j].transform == hit.transform)
                    {
                        // break out of it because multiple colliders can create too many effects
                        Instantiate(explosion, exitHits[j].point, Quaternion.Euler(new Vector3(0,0,180)));
                        break;
                    }
                        
                }


                enemy.Damage(damage);

            }
        }
    
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

}
