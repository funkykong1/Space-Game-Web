using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Fire")]
// damage over time effect
public class StatusFire : StatusEffect
{
    public float tickDamage;

    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        base.OnApply(victim);
    }

    // Called every tick (optional)
    public override void OnTick()
    {
        target.Damage(tickDamage);
    }

}