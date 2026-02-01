using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Electric")]

// Electric stun + small dmg vulnerability effect
public class StatusElectric : StatusEffect
{
    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        base.OnApply(victim);
        
        // electric adds to incoming damage
        target.ChangeDmgMult(0.2f);
        target.ChangeSpeed(-0.2f);
    }


    public override void OnExpire()
    {
        // simply inverse the values upon expiration
        target.ChangeDmgMult(-0.2f);
        target.ChangeSpeed(0.2f);
    }

}