using System.Collections;
using UnityEngine;
using Dreamteck.Splines;
using PrimeTween;
using System.Collections.Generic;
using System.Linq;


// Abstract enemybase class containing common factors between enemies such as health 
public abstract class EnemyBase : PlaneBase
{

    [Header("Manually Assigned References")]
    public int maxHP = 100;
    //explosion which is spawned when the ship dies
    public GameObject shipExplosion, bullet;
    //store the spline to be used in here
    public SplineComputer spline;


    //  spline offset number, 0-1, set in EDITOR!!
    // so that they form a conga line instead of doing mitosis at end
    [Range(0.0f, 1.0f)]
    public float offset;

    [Range(-1, 100)]
    public int enemyValue = -1;
    protected bool colorRunning = false;



    
    [Header("Automated References")]

    //100 should be the baseline I think
    public int currentHP;
    protected SplineFollower follower;

    // active is true when this is fully in position on the map, won't fire unless firing && in pos
    public bool  active = false, firing = true;


    [SerializeField]
    protected List<Transform> barrels;
    [SerializeField]
    protected List<SpriteRenderer> rend;


    //THESE are for the damage color tick
    protected Color clr = new Color(1, 0.55f, 0.55f, 1);

    //this thing's handler
    protected EnemyMaster master;

    
    // if this has a shield then this will be used- otherwise it's moot
    [SerializeField]
    protected Shield shield;

    public float pickupChance = 0.15f;

    protected virtual void Awake()
    {

        if (enemyValue == -1)
        {
            enemyValue = Random.Range(5, 15);
        }

        // debug todo turn on
        //invulnerable = true;

        rend.Add(GetComponent<SpriteRenderer>());
        rend.AddRange(GetComponentsInChildren<SpriteRenderer>().Where(s => !s.GetComponent<Shield>()));
        follower = GetComponent<SplineFollower>();


        //manually get each barrel for multi gun enemies
        foreach (Transform child in transform)
        {
            if (child.CompareTag("EnemyBarrel"))
                barrels.Add(child);
                
            if(child.GetComponent<Shield>())
                shield = child.GetComponent<Shield>();
        }

    }


    protected virtual void Start()
    {
        currentHP = maxHP;
        master = GameObject.Find("Enemy Master").GetComponent<EnemyMaster>();
    }


    public virtual void Activate()
    {
        StartCoroutine(SplineCoroutine());
    }

    // Virtual so that nonsplined enemies wont have to declare nothing
    // Splines will be different between ambient and standard enemies
    protected virtual IEnumerator SplineCoroutine()
    {
        yield return null;
    }

    // SET IN SPLINE COMPUTER
    public virtual void SplineComplete()
    {
        return;
    }


    // called by enemymaster - active set by spline ending
    public void EnemyShoot()
    {
        if (!active || !firing)
            return;

        for (int i = 0; i < barrels.Count; i++)
        {
            Instantiate(bullet, barrels[i].position, barrels[i].rotation);
        }
    }
    
    protected override void DoHealing(float amount)
    {
        // todo overhaul this
        currentHP =+ (int)amount;
    }

    public override void ToggleFiring(bool toggle)
    {
        firing = toggle;
    }

    public override void ChangeFirerate(float addition)
    {
        base.ChangeFirerate(addition);
    }

    protected override void DoDamage(float damage)
    {
        if (invulnerable)
        {
            // todo fx?
            return;
        }

        // deal shield hp amount of damage then DIP
        // if it exists lol
        if (shield != null)
        {
            if(shield.health > 0)
            {
                shield.ShieldHit(damage);
                return;
            }
        }   

        // prevent stuff from dying and spawning coins twice
        if(currentHP <= 0)
            return;
        

        currentHP -= (int)damage;
        if (currentHP <= 0)
        {
            EnemyDeath();
        }


        //color red upon taking damage
        if (colorRunning)
        {
            StopCoroutine(ColorRoutine());
            colorRunning = false;
        }

        StartCoroutine(ColorRoutine());


    }

    protected virtual void EnemyDeath()
    {
        SpawnCoins(enemyValue, transform.position);

        float rnd = Random.Range(0,1f);

        // if rnd less than pickupchance, spawn a random pickup
        if(rnd < pickupChance)
            Instantiate(master.pickups[Random.Range(0,master.pickups.Length)], transform.position, Quaternion.identity);
    }

    protected virtual void SpawnCoins(int value, Vector3 target)
    {
        int coin = 0;
        int coinVal = 1;
        for (int i = 0; i < value; i += coinVal)
        {
            int rand = Random.Range(0, 4);

            // Game will spawn a big coin based on total ship value remaining and also random chance
            // Chance is skewed towards spawning big coins in general (66%)

            if (value - i > 1 && rand != 0)
            {
                coin = Random.Range(0, 3);
                Instantiate(master.coins[coin], target, Quaternion.identity);
                coinVal = master.coins[coin].value;
            }
            // If the 33% is rolled or remaining value is 1, spawn a small coin
            else
            {
                // Int Random Range is max exclusive - coins.Length is 7 but the 7 is excluded
                coin = Random.Range(2, 7);
                Instantiate(master.coins[coin], target, Quaternion.identity);
                coinVal = 1;
            }
        }
    }

    protected IEnumerator ColorRoutine()
    {
        colorRunning = true;

        // Use the primary renderer as reference
        rend[0].color = clr;

        while (clr.g < 1)
        {
            for (int j = 0; j < rend.Count; j++)
            {

                clr.b += Time.fixedDeltaTime * 2;
                clr.g += Time.fixedDeltaTime * 2;
                rend[j].color = clr;
            }
            yield return new WaitForFixedUpdate();
        }

        colorRunning = false;
        clr = new Color(1, 0.55f, 0.55f, 1);
        //yield return rend.color = new Color(1, 1, 1, 1);
    }

}

