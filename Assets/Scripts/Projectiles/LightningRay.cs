using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using PrimeTween;

[RequireComponent(typeof(AudioSource))]


// Throwaway lightning object which shows up and deals damage via raycast then goes away by itself
public class LightningRay : PlayerBullet
{

    // This is a library of effects such as explosion sound effects
    [SerializeField]
    FXLibrary library;

    // default size should be larger than the screen
    [SerializeField]
    SpriteRenderer rend;

    // DOPE solution 
    [SerializeField]
    Sprite[] sprites;

    RaycastHit hit;
    Quaternion rot;
    int layerMask;


    void Awake()
    {
        // start off invisible
        rend.enabled = false;
    
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        // int be maxexclusive probably cuz ur meant to use .Length
        rend.sprite = sprites[Random.Range(0,sprites.Length)];
        // immediately snap it into pixel perfect position
        Vector3 pos = transform.position;
        float pixelSize = 1f / 64f;
        pos.x = Mathf.Round(pos.x / pixelSize) * pixelSize;
        pos.y = Mathf.Round(pos.y / pixelSize) * pixelSize;
        transform.position = pos;

        // explosion rotation is randomized 
        rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359)));
    }

    // summon THUNDER !!!
    protected override IEnumerator BulletCoroutine()
    {
        
        Debug.DrawRay(transform.position, transform.up*rend.size.y, Color.chartreuse, 2);

        // construct a new vector for runtime renderer size changing
        Vector2 v = new Vector2(rend.size.x, 0);

        // rend.enabled is tru
        rend.enabled = true;
        WaitForSeconds ws = new WaitForSeconds(0.03f);



        // This uses a small raycast to check for enemy impact every time
        Ray ray = new (transform.position, transform.up);

    
        // --
        // this section is to give the lightning a trajectory instead of making it instant
        // the renderer's y value is increased by speed until it reaches the hit target or reaches max range (offscreen)
        for (float i = 0; i < 50; i += speed)
        {
            // if max range is reached or a hit occurs, break out of the loop
            if(Physics.Raycast(ray, out hit, i, layerMask))
            {
                i = hit.distance;
                        // if its an enemy
                if(hit.transform.TryGetComponent<EnemyBase>(out EnemyBase enemy))
                {
                    print("i hit the ummm uhhh " + hit.transform);

                    // set thunder size to distance to impact
                    v.y = hit.distance;
                    rend.size = v;

                    // after effects are applied deal dmg :p
                    Instantiate(explosion, hit.point, rot);
                    enemy.Damage(damage);
                    // ITERATE THROUGH ALL OF THEM!!!!
                    // dont do it to a dead enemy
                    TryApplyingEffects(enemy);
                    break;
                }
                else
                {
                    print("i hit a non enemy within enemy layermask" + hit.transform);
                    Instantiate(explosion, hit.point, rot);

                    v.y = hit.distance;
                    rend.size = v;
                    break;
                }
            }
            
            v.y = i;
            rend.size = v;

            yield return ws;
        }
            

        // --

        // set the renderer length as the distance to target of collision
        //rend.size = new Vector2(rend.size.x, hit.distance);

        // altering only the opacity isnt possible - must use color
        Tween.Color(rend, new Color(rend.color.r, rend.color.g, rend.color.b, 0), 0.2f, Ease.Linear);
        Destroy(gameObject, 1);

    }
}
