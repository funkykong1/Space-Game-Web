using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBulletMoving : PlayerBullet
{
    bool moving = true;

    bool hit = false;
    // class for basic bullets which move without gimmicks
    protected override IEnumerator BulletCoroutine()
    {
        while(moving)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
            yield return new WaitForFixedUpdate();
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().emitting = false;
    }


    void OnTriggerEnter(Collider other)
    {
        
        if(other.GetComponent<LevelEnemyTrigger>())
            Destroy(gameObject);
        // try getting the enemy base of a collider, return if none
        // pretty neat way of doing it :)
        if(!other.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            return;

        // bullets wont even collide with invincible enemies
        if(enemy.invulnerable)
            return;
        
        // only collide once!!
        if(hit)
            return;

        hit = true;

        Instantiate(explosion, transform.position, transform.rotation);
        moving = false;
        Destroy(gameObject,0.5f);
        
        // only apply effects after dealing damage o_O try function checks if its dead 
        enemy.Damage(damage);
        TryApplyingEffects(enemy);
        
    }
}
