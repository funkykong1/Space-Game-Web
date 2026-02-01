using UnityEngine;


// Base class for all status effects
// They inherit from this, never used directly
public abstract class StatusEffect : ScriptableObject
{
    public float duration = 5;
    public float tickInterval = 1;
    public GameObject particlePrefab;
    public PlaneBase target;
    public bool guaranteed = false;

    public int id;

    // shit effects positive
    // fire - 0
    // thunder - 1 
    // ice - 2
    // plasma - 3
    
    // good effects negative
    // energized - -1
    // super shield - -2


    // Called when the effect is applied
    // target is set here 
    public virtual void OnApply(PlaneBase effect_target)
    {
        target = effect_target;
    }


    // Called every tick (optional)
    public virtual void OnTick()
    {

    }

    // Called when the effect ends
    public virtual void OnExpire()
    {

    }
}