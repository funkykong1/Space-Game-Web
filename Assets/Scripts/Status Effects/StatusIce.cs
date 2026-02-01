using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Ice")]

// Electric stun + small dmg vulnerability effect
public class StatusIce : StatusEffect
{
    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        base.OnApply(victim);
        target.ChangeFirerate(-0.2f);
        target.ChangeSpeed(-0.2f);
    }


    public override void OnExpire()
    {
        // simply inverse the values upon expiration
        target.ChangeFirerate(0.2f);
        target.ChangeSpeed(0.2f);
    }

}