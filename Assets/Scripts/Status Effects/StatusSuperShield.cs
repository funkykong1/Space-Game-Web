using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/SuperShield")]

// gives the target a super strong shield for a while
public class StatusSuperShield : StatusEffect
{
    Shield shield;
    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        base.OnApply(victim);

        // does nothing if target has no shield
        if(target.TryGetComponent(out shield))
        {
            shield.ShieldInvuln(true);
        }
    }
    


    public override void OnExpire()
    {
        if(shield != null)
        {
            shield.ShieldInvuln(false);
        }
    }

}