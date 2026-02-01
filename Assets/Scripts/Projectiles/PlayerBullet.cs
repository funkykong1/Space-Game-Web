using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// base class for all player projectiles which has the most basic values
// each bullet has atleast a damage value and an explosion (a bullet impact)
public abstract class PlayerBullet : MonoBehaviour
{
    [Header("Manually Assigned References")]
    // speed of projectile (varies on use case), // range before it stops moving
    public float speed;

    [SerializeField]
    protected GameObject explosion;
    [HideInInspector]
    public int damage;

    // status effect caused by this bullet
    // every bullet has this so player can get power ups which give status chances
    [SerializeField]
    protected List<StatusEffect> effects = new();

    [Range(0,1)]

    public float statusChance; 


    void Awake()
    {
        if(TryGetComponent<AudioSource>(out AudioSource src))
            src.pitch += Random.Range(-0.05f, 0f);
    }
    protected virtual void Start()
    {
        StartCoroutine(BulletCoroutine());
    }

    // lol
    protected virtual IEnumerator BulletCoroutine()
    {yield return null;}

    protected virtual void TryApplyingEffects(EnemyBase enemy)
    {
        // dont do it to a dead enemy!!!!!
        // todo playtest the hell out of this!!
        if(enemy.gameObject != null)
        {
            float rand;
            foreach (StatusEffect ef in effects)
            {
                rand = Random.Range(0,1f);
                // ROLL THEM DICEE on status procs (0.00-1.00)
                // guaranteed effects like plasma apply always
                if(ef.guaranteed || rand < statusChance)
                {
                    print("added efect " + ef + " to " + enemy.gameObject.name);
                    enemy.AddEffect(ef);
                }
            }
        }
    }
}
