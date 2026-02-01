using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Energized")]

// Energized effect adds firerate boost
public class StatusEnergized : StatusEffect
{
    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        target = victim;
        target.ChangeFirerate(0.3f);
    }


    public override void OnExpire()
    {
        // simply inverse the values upon expiration
        target.ChangeFirerate(-0.3f);
    }

}