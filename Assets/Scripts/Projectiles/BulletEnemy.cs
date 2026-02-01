using UnityEngine;
using System.Collections;

public class BulletEnemy : MonoBehaviour
{
    public int speed;
    //default one
    public int bulletDamage=1;
    public GameObject explosion;
    bool moving = true;
    TrailRenderer trail;
    SpriteRenderer rend;
    Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<SpriteRenderer>();
        TryGetComponent<TrailRenderer>(out trail);

        GetComponent<AudioSource>().pitch += Random.Range(-0.05f, 0f);
    }
    void Start()
    {
        StartCoroutine(BulletCoroutine());
    }

    // 
    private IEnumerator BulletCoroutine()
    {
        while(moving)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
            yield return new WaitForFixedUpdate();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        //if already triggered once, (not moving, just return)
        if(!moving)
            return;
            
        // try getting the enemy base of a collider, return if none
        // pretty neat way of doing it :)
        if(!other.TryGetComponent<Player>(out Player player))
            return;

        // coroutine thing
        moving = false;


        //disable collider and renderers upon collision
        rend.enabled = false;
        
        if(trail)
            trail.emitting = false;

        player.Damage(bulletDamage);

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject,0.5f);
        
    }
}
