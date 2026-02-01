using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

// base class for everything which drops from enemy planes
public abstract class Pickup : MonoBehaviour  

{                                                   

    protected Vector3 rot, dir;

    // SET IN EDITOR 
    [SerializeField]
    protected Rigidbody rb;


    // if not falling, its following the player and vice versa
    protected bool falling = false;

    protected WaitForFixedUpdate wf = new();

    protected float distance = 0;
    protected Player player;

    protected Vector3 v = new(0,-2,0);

    public int value = 0;
    protected virtual void Awake()
    {
        rot = new Vector3(0, 0, Random.Range(-2, 2));
        dir = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.6f, 0.6f), 0);

        player = FindAnyObjectByType<Player>();
        
        // when it 'explodes' out of the enemy give it a higher max velocity
        rb.maxLinearVelocity = 2f;
        rb.maxAngularVelocity = 0.6f;
    }

    protected virtual void Start()
    {
        // Add a random rotation to the pickup upon spawn
        rb.AddRelativeTorque(rot, ForceMode.Impulse);
        // Random force (it flies away from the enemy upon spawn)
        rb.AddForce(dir, ForceMode.Impulse);

        StartCoroutine(Fall());
    }

    protected IEnumerator repeatFall()
    {
        while (falling)
        {
            rb.AddForce(v, ForceMode.Acceleration);
            yield return wf;
        }
    }

    //slowly drop toward player
    protected virtual IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.25f);
        rb.maxLinearVelocity = 1;
        falling = true;
        StartCoroutine(repeatFall());

    }
    protected abstract void PickupAction();

    
    public void OnTriggerEnter(Collider col)
    {

        if (col.gameObject == player.gameObject)
        {
            // TODO fx
            PickupAction();
            Destroy(gameObject);
        }


        // Only start player follow coroutine once
        if (!falling)
            return;

        if (col.gameObject.CompareTag("CoinMagnet"))
        {
            StartCoroutine(FollowPlayer());
        }
    }
    public void OnTriggerExit(Collider col)
    {
        if(col.gameObject.CompareTag("CoinMagnet"))
        {
            StartCoroutine(Fall());
        }
            
    }

    protected IEnumerator FollowPlayer()
    {
        falling = false;
        rb.maxLinearVelocity = 1.2f;
        rb.linearDamping = 0.9f;
        while (!falling)
        {
            Homing();
            yield return wf;
        }
        //rb.linearDamping = 1.5f;
    }

    public IEnumerator ForceFollowPlayer()
    {
        falling = false;
        rb.linearDamping = 3;
        
        while (true)
        {
            Homing();
            yield return wf;
        }
    }

    void Homing()
    {
        dir = player.transform.position - transform.position;
        distance = Vector3.Distance(transform.position, player.transform.position);
        // max speed scales with distance
        rb.maxLinearVelocity = 1.5f + distance/1.25f;

        rb.AddForce(dir, ForceMode.VelocityChange);
    }
    protected virtual void SellPickup()
    {
        player.lui.AddResources(value,"coin");
    }
}


