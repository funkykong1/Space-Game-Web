using UnityEngine;
using System.Collections;



//thing which every weapon inherits from. allows us to configure the weapon regardless of its type
//This is present in every weapon gameobject. Override bullet shooting via another script 
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Manually Assigned References")]
    // prefab reference
    public GameObject bullet;
    protected Transform barrel;


    // Every weapon has a builtin cooldown (seconds or charge anim)
    // only change cooldown multiplier
    public float spread=0, cooldownMult=1;
    // every weapon applies its damage to the bullets it creates upon instantiation
    public int damage;

    [SerializeField]
    protected float statusChance = 0.2f;

    // temp reference to instantiated bullet for value adjustment
    protected PlayerBullet p;

    [Header("Firing is automated")]
    public bool firing=false;
    // create a vector3 here instead of doing it repeatedly later
    protected Vector3 rot;



    protected virtual void Awake()
    {
        barrel = transform.GetChild(0);
        
    }

    // this is called by player at start
    public virtual void ToggleFiring(bool toggle)
    {
        // this can awkwardly cause double coroutines if called twice
        if(firing == toggle)
            return;

        firing = toggle;
        // stop coroutines incase weapon uses it instead of animations
        if (toggle)
        {
            StopAllCoroutines();
            StartCoroutine(ShootBurst());
        }
    }

    // make it virtual cuz not all of the guns will use it
    // it also doesnt work with animation events cuz fuck u
    protected virtual void FireBullet()
    {}
    

    // this is public but the thing it calls isnt. firerate is modified via player -> this
    public virtual void ChangeFirerate(float cd)
    {
        cooldownMult += cd;
    }

    //use a coroutine to handle shooting
    protected virtual IEnumerator ShootBurst()
    {yield return null;}


    
}

