using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Player : PlaneBase
{
    // Player movement, health and weapons in level

    // things you HAVE TO SET IN EDITOR::
    // WEAPON SLOTS, MUST HAVE PlayerShipSlot TAG

    [SerializeField]
    FXLibrary helper;
    Vector3 pos;
    private LoadoutManager lm;

    // in editor movement
    float horizontalInput;
    float verticalInput;


    // Ship is to not follow player input perfectly but move after it 
    public float shipSpeed;

    // map
    private GameObject map;
    // boundaries for player movement
    float xRange, yRange;
    public GameObject shipExplosion;

    // UI heart manager
    public HealthManager health;

    // manually count iframes
    public int iFrameCount = 10;


    public float flashDelay = 0.08f;

    // iframe, firing bool, movement bool
    public bool isHit = false, firing, moving = true;

    
    public Transform[] weaponSlots;

    public List<SpriteRenderer> renderers;

    //colors used for player iframes 
    private Color opaque = new Color(1, 1, 1, 1);
    private Color transparent = new Color(1, 1, 1, 0.7f);

    Collider[] col;
    [SerializeField]
    WeaponBase[] activeWeapons;

    [HideInInspector]
    public Shield shield;

    [HideInInspector]
    public LevelUI lui;

    public int outCollisionDMG = 75;
    WaitForFixedUpdate wf = new WaitForFixedUpdate(); // create reference only once for performance
    float pixelSize = 1f / 64f; // Match PPU


    void Awake()
    {
        firing = false;
        // useful for arranging order of lines

        Init();
    }


    // get everything relevant when spawned
    public void Init()
    { 

        // EVERY player ship will have a shield child
        shield = GetComponentInChildren<Shield>();
        health = FindFirstObjectByType<HealthManager>();

        
        col = GetComponents<Collider>();
        StartCoroutine(Movement());

        
        lui = FindAnyObjectByType<LevelUI>();




        map = GameObject.Find("Map");
        xRange = map.GetComponent<Map>().xRange;
        yRange = map.GetComponent<Map>().yRange;

        if(gameObject.CompareTag("DEBUG"))
            return;

        lm = FindAnyObjectByType<LoadoutManager>();

        //active weapons array for disabling them easily
        activeWeapons = new WeaponBase[lm.equippedWeapons.Length];
        
        // Spawn in player weapons
        ApplyWeapons();

    }

    //instantiate player weapons
    public void ApplyWeapons()
    {
        // iterate through all weaponslots and equipped weapons from LM
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            // Player can refrain from equipping items, ignore weaponslot without an item assigned
            if(lm.equippedWeapons[i] == null)
                continue;

            WeaponBase go = Instantiate(lm.equippedWeapons[i], weaponSlots[i]).GetComponent<WeaponBase>();
            //player keeps track of current weapons
            activeWeapons[i] = go;

        }

        //at the end of the loop, add all new weapons to the renderer array for uniform i-frames
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>().Where(s => !s.GetComponent<Shield>()));
    }


    // MAP RESTRICTION
    void Update()
    {
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
        if (transform.position.y < -yRange)
        {
            transform.position = new Vector3(transform.position.x, -yRange, transform.position.z);
        }

        if (transform.position.y > yRange)
        {
            transform.position = new Vector3(transform.position.x, yRange, transform.position.z);
        }
    }

    // PLAYER MOVEMENT KEYBOARD
    IEnumerator Movement()
    {
        while (moving)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            transform.Translate(horizontalInput * shipSpeed * Time.deltaTime * Vector3.right, Space.World);

            verticalInput = Input.GetAxis("Vertical");
            transform.Translate(shipSpeed * Time.deltaTime * verticalInput * Vector3.up, Space.World);


            // Pixel Perfect movement :-)
            // Not doing this causes weird distortion of the player sprite
            pos = transform.position;
            pos.x = Mathf.Round(pos.x / pixelSize) * pixelSize;
            pos.y = Mathf.Round(pos.y / pixelSize) * pixelSize;
            transform.position = pos;

            yield return wf;
        }

        // Create a simple loop
        yield return new WaitUntil(() => moving);
        StartCoroutine(Movement());
    }


    //player health will be 1 = 0.5 hearts
    //4 damage thereby would be 2 hearts and so on
    // TODO float health and get rid of hearts
    protected override void DoDamage(float damage)
    {
        //ignore damage if iframes are active
        if (isHit)
            return;

        // call it here already to 100% prevent multiple hits before iframes
        isHit = true;

        //TODO SHIELD HEARTS and hp
        // Shield completely blocks a point of damage
        if (shield.active)
        {
            shield.ShieldHit(damage);
            StartCoroutine(Iframes());
            return;
            
        }
        

        health.UpdateHearts(damage, false);

        if (health.currentHealth <= 0)
        {
            //make an explosion and rotate it randomly
            Instantiate(shipExplosion, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));

            Destroy(gameObject);
        }

        StartCoroutine(Iframes());
    }

    protected override void DoHealing(float healing)
    {
        health.UpdateHearts((int)healing, true);
    }
    

    public override void ChangeFirerate(float addition)
    {
        // call base always
        base.ChangeFirerate(addition);

        // iterate thru all and update rof multiplier
        for(int i = 0; i < activeWeapons.Length; i++)
            if(activeWeapons[i] != null)
                activeWeapons[i].ChangeFirerate(addition);
    }

    // iterate through every renderer a set amount of times
    IEnumerator Iframes()
    {
        // start from transparent
        bool flash = true;

        // ensure its always an odd number
        if (iFrameCount % 2 == 0)
            iFrameCount++;

        for (float i = 0; i < iFrameCount; i += 1)
        {
            for (int j = 0; j < renderers.Count; j++)
            {
                // Alternate between 0.5 and 1 scale to simulate flashing
                if (flash)
                {
                    renderers[j].color = opaque;
                }
                else
                {
                    renderers[j].color = transparent;
                }
            }
            flash = !flash;
            yield return new WaitForSeconds(flashDelay);
        }
        isHit = false;
    }


    // Player collision
    private void OnTriggerEnter(Collider other)
    {
        if (isHit)
            return;

        if (!other.TryGetComponent(out EnemyBase en))
            return;

        // todo fx here
        Damage(1);
        pos = (transform.position + en.transform.position) / 2;

        // direction from this to collided enemy
        Vector3 dir = transform.position - en.transform.position;

        // angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // sideways rotation for collision explosion
        Quaternion rot = Quaternion.Euler(0f, 0f, angle-45);

        Instantiate(helper.explosionCollision, pos, rot);
        en.Damage(outCollisionDMG);

    }

    public override void ToggleFiring(bool toggle)
    {
        firing = toggle;
        // iterate thru all and set firing to toggle
        for(int i = 0; i < activeWeapons.Length; i++)
            if(activeWeapons[i] != null)
                activeWeapons[i].ToggleFiring(toggle);
    }

    //TODO MAYBE SOME FX
    public void PlayerInvuln(bool toggle)
    {
        isHit = toggle;
    }
}
