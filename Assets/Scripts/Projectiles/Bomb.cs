using System.Collections;
using UnityEngine;
using PrimeTween;
using Unity.VisualScripting;


// mine projectile thrown by mine launcher. Slows down and just hangs around
public class Bomb : PlayerBullet
{
    // spawns bullets upon exploding
    [SerializeField]
    private GameObject shrapnel;

    [SerializeField]
    private int shrapnelCount;

    // this uses physics and rigidbody for a slowdown effect
    [SerializeField]
    Rigidbody rb;

    // simple sprite switch animation 
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    SpriteRenderer rend;
    
    private static readonly WaitForSeconds longDelay = new(1);
    private static readonly WaitForSeconds shortDelay = new(0.2f);

    // This gives shrapnel objects a reference to a plane to not collide with
    private EnemyBase enemy;

    private bool collided = false;

    IEnumerator BootlegAnimator()
    {
        while (true)
        {   
            // 2 blinks, with delay of shortdelay each
            for(int i = 0; i < 2; i++)
            {
                rend.sprite = sprites[1];
                yield return shortDelay;
                rend.sprite = sprites[0];
                yield return shortDelay;
            }
            // wait longer between sets of blinks
            yield return shortDelay;
        }
    }

    protected override IEnumerator BulletCoroutine()
    {
        
        StartCoroutine(BootlegAnimator());
        // add start boost of speed and a small rotation
        rb.AddForce(new Vector3(0, speed, 0), ForceMode.Impulse);

        // rotation can look weird todo sort this out
        //rb.AddTorque(new Vector3(0,0, Random.Range(-0.5f,0.5f)));



        // it has a brief moment of 0 velocity because addforce works on fixedupdate
        yield return longDelay;

        // wait until drag lowers the velocity 
        yield return new WaitUntil(() => rb.linearVelocity.y < 1.2f);
        // remain at slow speed
        rb.linearDamping = 0;

    }



    void OnTriggerEnter(Collider other)
    {  
        // return if already exploded
        if(collided)
            return;
        
        // prevent the child area trigger from calling this
        if(!this.gameObject.CompareTag("Bullet"))
            return;
        // try getting the enemy base of a collider, return if none
        // pretty neat way of doing it :)
        if(!other.TryGetComponent<EnemyBase>(out enemy))
            return;

        // bomb also ignores invincible enemies
        if(enemy.invulnerable)
            return;


        collided = true;
        enemy.Damage(damage);

        Explode();
        
        Destroy(gameObject,1f);

        // only apply effects after dealing damage o_O try function checks if its dead 
        TryApplyingEffects(enemy);
        
    }

    void Explode()
    {
        BombShrapnel throwaway;
        Vector3 rot;
        Instantiate(explosion, transform.position, transform.rotation);

        rend.enabled = false;

        // we calculate this only once !!
        int shrapDmg = (int)(damage * 0.25f); 

        for(int i = 0; i < shrapnelCount; i++)
        {
            rot = new Vector3(0,0,Random.Range(0, 359));
            throwaway = Instantiate(shrapnel, transform.position, Quaternion.Euler(rot)).GetComponent<BombShrapnel>();

            // shrapnel ignores and pierces through the first enemy
            throwaway.ignoreThis = enemy;
            // bomb shrapnel scales with damage of the bomb itself
            throwaway.damage = shrapDmg;

        }
    }

    public IEnumerator HomingTime(EnemyBase target)
    {
        // stop blinkin
        StopCoroutine(BootlegAnimator());

        //lockon sprite
        rend.sprite = sprites[2];

        Vector3 dir;
        WaitForFixedUpdate wff = new();

        // todo tweak
        rb.maxLinearVelocity = 7f;
        rb.linearDamping = 0.4f;

        

        // home until collision
        while (true)
        {
            if(target != null)
                dir = target.transform.position - transform.position;
            else
                break;

            //todo tweak 2
            rb.AddForce(dir*0.1f, ForceMode.VelocityChange);

            yield return wff;
        }
        rb.linearDamping = 0.4f;

        yield return longDelay;

        rb.linearDamping = 0;
    }
}
