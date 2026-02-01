using System.Collections;
using UnityEngine;
using Dreamteck.Splines;
using PrimeTween;
using System.Collections.Generic;
using System.Linq;


// This will move an enemy along a spline and then 'tween' it to a final position
// Place enemies on the map to design their spot in a level. EnemyMaster will move them away during play
// This will just get a random spline if I forgot to give it one manually

// Set spline and the spline completion event in editor!!

// Manually tweak values in editor if this is splined
// ALWAYS tweak ship coin value in editor

// Ship coin and pickup drops are set in enemymaster :p

public class BasicEnemy : EnemyBase
{
    //rotation to be used by the rotator tween after a spline
    [Range(30f, 260f)]
    public float tweenRotation = 120;

    // flag used by splinecr
    bool isSplined = true;

    // used by SPLINE COROUTINE to ensure both tweens are done before another is fired up
    bool ready = false;

    //speed to be used by the move tween after a spline
    [Range(0.5f, 15f)]
    public float tweenSpeed = 3;
    [Range(0.05f, 0.5f)]
    public float idleSpeed = 0.15f;


    //where should this enemy be when it stops moving?
    private Vector3 mapPos; private Quaternion mapRot;

    protected override void Awake()
    {
        //make a note of the original position - enables easy editing of enemy formations
        mapPos = transform.position;
        mapRot = transform.rotation;
        base.Awake();

        // if we forgot to set a spline then just get one randomly lol
        if (!spline)
        {
            print("YOU FORGOT TO SET THE CONTAINER FOR " + gameObject.name);
            
            SplineComputer[] splinez = GameObject.Find("ENTRY SPLINES").GetComponentsInChildren<SplineComputer>();
            spline = splinez [Random.Range(0,splinez.Length)];
        }

    }
    protected override void Start()
    {
        base.Start();
        follower.followSpeed = speed;
    }

    protected override IEnumerator SplineCoroutine()
    {
        follower.spline = spline;
        follower.enabled = true;

        follower.followSpeed = speed;
        //manually call rebuild as a catch all to prevent bugs
        follower.RebuildImmediate();

        //Manually set each ship to be apart from one another via offset
        //eg.. ship #1 is 0.15, #2 is 0.10 and #3 0.05!
        follower.clipFrom = offset;

        // create a random float to apply to idle sway animation
        // put it here because this spot is kind of like downtime for the script
        float rnd = Random.Range(-0.3f, 0.3f);


        //Wait until an event is called which turns this false
        yield return new WaitUntil(() => !isSplined);

        // Tween it up in here (speed based), when rotation is done might aswell make it active
        Tween.PositionAtSpeed(transform, endValue: mapPos, tweenSpeed).OnComplete(() => ready = true, false);
        //likewise for rotation
        Tween.RotationAtSpeed(transform, mapRot, tweenRotation).OnComplete(() => active = true, false);

        //Wait until both of them are complete
        yield return new WaitUntil(() => active && ready);

        print("HELLO I HAVE REACHED THE END t. " + gameObject);

        mapPos.x += rnd;

        // repeatedly tween back and forth to create an idle animation
        Tween.PositionAtSpeed(transform, mapPos, idleSpeed, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
    }

    // public so spline computer can use it
    public override void SplineComplete()
    {
        //reuse this bool to tell us when the thing leaves a spline
        isSplined = false;
        //set 'Follow' to false to avoid auto updating the position back onto the spline
        follower.follow = false;
        follower.enabled = false;
        
    }


    // called from enemy master 
    protected override void EnemyDeath()
    {
        // spawn coin
        base.EnemyDeath();
        //remove this from enemy master's current list
        master.activeEnemies.Remove(this);

        //make an explosion and rotate it randomly
        Instantiate(shipExplosion, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));


        //if the current wave is empty, tell the master to start a new one
        if (master.activeEnemies.Count <= 0)
        {
            master.Invoke(nameof(master.TrySpawnWave), 2);
        }

        Destroy(gameObject, 0.1f);
    }
}

