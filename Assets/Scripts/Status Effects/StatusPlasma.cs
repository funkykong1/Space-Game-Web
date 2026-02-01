using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Plasma")]

// Electric stun + small dmg vulnerability effect
public class StatusPlasma : StatusEffect
{
    public float explosionDamage = 50f;
    public GameObject explosion;
    // Called when the effect is applied
    public override void OnApply(PlaneBase victim)
    {
        base.OnApply(victim);
            // this interacts with ice to create an explosion
            if(target.statusList.Contains(2))
            {
                Instantiate(explosion, target.transform);
                target.Damage(explosionDamage);
            }
        }
    }
