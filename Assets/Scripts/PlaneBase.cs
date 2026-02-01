using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Base class shared both by enemies and players
// handles status effects and multipliers to incoming values
public abstract class PlaneBase : MonoBehaviour
{
    // public methods apply multipliers before passing them to an internal one
    public void Damage(float amount)
    {
        if(!invulnerable)
            DoDamage(amount * damageMultiplier);
    }    

    public void Heal(float amount)
    {
        // do we really need a healing multiplier?
        DoHealing(amount);
    }

    // inheritors implement these
    protected abstract void DoDamage(float realAmount);
    protected abstract void DoHealing(float realHealing);


    public virtual void ToggleFiring(bool toggle)
    {
        
    }

    public bool invulnerable;
    // Values used by planes to modulate incoming dmg n stuff
    // incoming dmg mult
    [SerializeField]
    protected float damageMultiplier = 1;
    // weapon rof mult
    [SerializeField]
    protected float firerateMultiplier = 1;
    [SerializeField]
    protected float speedMultiplier = 1;
    //
    
    // Unfiltered values not directly used by planes
    private float rawDmgMult = 1;
    private float rawRofMult = 1;
    private float rawSpeedMult = 1;



    // I really really dont want these to be fucking public the inspector will be a metre long
    // top values for adjusted stats
    [SerializeField]
    private readonly float speedMultCap = 2f;
    [SerializeField]
    private readonly float damageMultCap = 2f;
    [SerializeField]
    private readonly float firerateMultCap = 2f;
    // bottom values for adjusted stats
    [SerializeField]
    private readonly float speedMultFloor = 0.2f;
    [SerializeField]
    // this is damage resistance
    private readonly float damageMultFloor = 0.2f;
    [SerializeField]
    private readonly float firerateMultFloor = 0.2f;



    // Functions used to change values and stay within limits
    // always call base.ChangeX from child
    public virtual void ChangeDmgMult(float addition)
    {
        rawDmgMult += addition;
        firerateMultiplier = CrunchThaNumbers(rawDmgMult, damageMultFloor, damageMultCap);
    }

    public virtual void ChangeFirerate(float addition)
    {
        rawRofMult += addition;
        firerateMultiplier = CrunchThaNumbers(rawRofMult, firerateMultFloor, firerateMultCap);
    }

    public virtual void ChangeSpeed(float addition)
    {
        rawSpeedMult += addition;
        speedMultiplier = CrunchThaNumbers(rawSpeedMult, speedMultFloor, speedMultCap);
    }

    private float CrunchThaNumbers(float raw, float floor, float cap)
    {
        // if bigger than cap, set as cap
        if(raw > cap)
            return cap;
        // if smaller than floor, set as floor
        else if(raw < floor)
            return floor;
        // otherwise just use raw value
        else
            return raw;
    }



    public float speed;

    // <<<< status manager >>>>
    
    // string list containing names of status effects - dont apply fire effect twice etc
    [SerializeField]
    public List<int> statusList = new();
    
    // Some enemies, like bosses should be immune to some effects
    [SerializeField]
    private List<int> immunityList = new();


    // this is called by a bullet hitting the target
    public void AddEffect(StatusEffect effect)
    {
        // return if enemy is immune to effect or unkillable at the moment
        if(invulnerable || immunityList.Contains(effect.id))
        {
            print("Blocked status effect named " + effect.name);
            return;
        }
        else
            StartCoroutine(RunEffect(effect));
    }

    // each applied status creates a coroutine for the given duration (data taken from scriptableobject)
    private IEnumerator RunEffect(StatusEffect effect)
    {
        GameObject fx = null;

        // spawn VFX if status has it
        if (effect.particlePrefab != null)
            // only spawn fx if it doesnt exist yet
            if(!statusList.Contains(effect.id))
            {
                fx = Instantiate(effect.particlePrefab, transform);
                // particle fx has id corresponding to the status effect
                fx.GetComponent<StatusParticles>().id = effect.id;
            }

        // its a scriptableobject so pass this as a parameter
        effect.OnApply(this);
        
        statusList.Add(effect.id);
        WaitForSeconds wfs = new(effect.tickInterval);
        float elapsed = 0;
        while (elapsed < effect.duration)
        {
            // prevents irritating errors from dying stuff taking tick dmg
            if(gameObject == null)
                yield break;

            // stop effect if its on an invulnerable thing
            // negative id means its a positive effect
            if(invulnerable && effect.id >= 0)
                break;
            

            // tick effect here
            effect.OnTick();
            yield return wfs;
            elapsed += effect.tickInterval;
        }

        // after effect is over
        statusList.Remove(effect.id);

        // do it again for good measure
        if(gameObject == null)
            yield break;
        
        // this applies a reverse of the debuff to restore values
        effect.OnExpire();
        // if the status spawned vfx and the target noone has stacks, remove effect
        if(!statusList.Contains(effect.id) && effect.particlePrefab != null)
            // loop thru all of them and find the one with matching id
            foreach(StatusParticles sp in transform.GetComponentsInChildren<StatusParticles>())
                if(sp.id == effect.id)
                    Destroy(sp.gameObject);

        
    }

}
