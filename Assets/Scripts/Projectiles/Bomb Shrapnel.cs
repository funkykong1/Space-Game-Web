using System.Collections;
using UnityEngine;
using PrimeTween;


// modifiable class for bullets spawned by bomb explosions
public class BombShrapnel : PlayerBullet
{

    [SerializeField]
    Rigidbody rb;

    // set in editor
    public BoxCollider box;

    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    SpriteRenderer rend;

    public PlaneBase ignoreThis;

    private bool hit = false;

    private static readonly WaitForSeconds shortDelay = new(0.2f);
    void Awake()
    {
        // add small variance to start speed and lifetime
        speed *= Random.Range(0.9f,1.1f);
        
        // spawn shrapnel in a small radius around the bomb (rotation set by gun)
        transform.localPosition += new Vector3(0, 0.5f, 0);
    }

    protected override void Start()
    {
        rb.AddRelativeForce(new Vector3(0, speed, 0), ForceMode.Impulse);
        StartCoroutine(BootlegAnimator());
    }

    void OnTriggerEnter(Collider other)
    {
        // try getting the enemy base of a collider, return if none
        // pretty neat way of doing it :)
        if(!other.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            return;
        
        // pierce thru first enemy 
        if(enemy == ignoreThis)
            return;

        // bullets wont even collide with invincible enemies
        if(enemy.invulnerable)
            return;

        // only collide once!!
        if(hit)
            return;
        hit = true;

        rend.enabled = false;

        Instantiate(explosion, transform.position, transform.rotation);
        enemy.Damage(damage);
        
        Destroy(gameObject,0.5f);

        // only apply effects after dealing damage o_O try function checks if its dead 
        TryApplyingEffects(enemy);
    }

    IEnumerator BootlegAnimator()
    {
        while (true)
        {
            if(rend.sprite == sprites[0])
                rend.sprite = sprites[1];
            else
                rend.sprite = sprites[0];
            yield return shortDelay;
        }
        
    }
}
