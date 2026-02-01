using UnityEngine;
using PrimeTween;
using System.Collections;

[RequireComponent(typeof(SpriteMask))]
[RequireComponent(typeof(SpriteRenderer))]


// This copies the sprite of whatever its parent is and applies a translucent shield effect over it
// This absorbs damage completely and is shut down when health reaches 0
public class Shield : MonoBehaviour
{


    [Header("Manually Assigned References")]
    public float health = 0;
     public float maxHealth = 2;

    // default 1.06 for a very slight field around the target
    // I think this should be in increments of 1/64 or 1/32?
    //private readonly float scale = 1.064f;

    // colour alpha values. all colour values are 0-1f
    public float opaque = 0.85f, trans = 0.25f;


    [Header("Automatic References")]
    public bool active = false;
    SpriteRenderer parent, rend;

    // cardboard cutout to which the shield sprite should adhere to
    SpriteMask mask;
    // we only ever want one of the color tweens alive at a time so create a reference for it
    Tween tween;
    // variable color 
    Color clr;

    public bool invuln;
    

    float timer = 0;
    void Awake()
    {
        // get the goods
        parent = transform.parent.GetComponent<SpriteRenderer>();
        rend = GetComponent<SpriteRenderer>();
        mask = GetComponent<SpriteMask>();
        // get color on start
        //clr = rend.color;
        clr = new(0.5f, 0.9f, 1, opaque);

        rend.color = clr;

        if(health > 0)
            ShieldToggle(true);
        else
        {
            // plane with possibility of shield but no hp
            clr.a = 0;
            TweenColor(clr, 0.1f);
        }
            
    }
    void Update()
    {
        // fading timer is inactive if shield is off or invulnerable
        if(invuln || !active)
            return;

        timer += Time.deltaTime;

        if (timer >= 5)
        {
            timer = 0;

            // fade to a transparent look over time
            clr.a = trans;
            TweenColor(clr, 2f);
        }
    }
    void LateUpdate()
    {
    if (active)
        if (mask.sprite != parent.sprite)
        {
            mask.sprite = parent.sprite;
        }
    }


    // thing's shield is activated or depleted
    public void ShieldToggle(bool toggle)
    {
        active = toggle;
        // fade shield in or out fast
        shild_toggel_visual_fx(toggle);
    }

    void shild_toggel_visual_fx(bool tog)
    {
        if(tog)
        {
            clr.a = opaque;
        }
        else
        {
            clr.a = 0;
        }
        TweenColor(clr, 0.5f);
    }

    //instantly stop fading and set the opacity to max
    public void ShieldHit(float damage)
    {
        if(active)
        {
            
            // shield doesn't change if invulnerable
            if(!invuln && health > 0)
            {
                // stop previous hit coroutine if its hit many times
                StopAllCoroutines();
                StartCoroutine(internal_Shield_Hit(damage));
            }
        }
    }

    
    private IEnumerator internal_Shield_Hit(float damage)
    {

        // if this is on an enemy, nerf the machine gun lol
        if(GetComponentInParent<EnemyBase>())
        {
            if(damage > 20)
                health -= 1;
            else
                health -= 0.25f;
        }
        // if its a player just do 1 damage
        else
        {
            health--;
        }

        // visual for impact
        HitTweener();

        // shield turns off (only do once)
        if (health <= 0)
        {
            if(active)
                ShieldToggle(false);
            health = 0;
            yield break;
        }

        // shield is still up and tweens back to original state
        else
        {
            // wait for a small amount before tweening to the standard opaque color
            yield return new WaitForSeconds(0.3f);
            TweenColor(clr, 0.2f, Ease.Linear);
        }
    }


    // visual for bullet impact on shield
    // also used by enemymaster to make enemy shields visible when they enter
    public void HitTweener()
    {
        // extra long time before fading cuz of tweens
        timer = -2;


        // make it really opaque really fast at the point of impact
        clr.a = opaque + 0.15f;
        // make it darker blue by removing some green
        clr.g = 0.4f;

        // impact colour change is instant
        rend.color = clr;

        // restore og values
        clr.g = 0.8f;
        clr.b = 0.8f;

        // if player is subject to a nonstop super fast barrage of hits then dont try tweening 1000 times
        if (rend.color.a < opaque + 0.15f)
        {
            TweenColor(clr, 0.2f, Ease.Linear);
        }
    }

    public void ShieldAdd(int hp)
    {
        timer = -1;
        health += hp;
        if (!active)
            ShieldToggle(true);
        else
        {
        // temporarily make shield more pronounced when some is added to it
            clr.g = 1f;
            clr.a = 1;
            TweenColor(clr, 0.2f, Ease.Linear);
        }

    }

    // tween into a given color and kill previous tween if alive
    private void TweenColor(Color target, float time, Ease ease = Ease.Linear)
    {
        if (tween.isAlive)
            tween.Stop();

        tween = Tween.Color(rend, target, time, ease);
        clr.a = opaque;
        clr.g = 0.8f;
        clr.b = 0.8f;
    }

    // call this twice as a toggle from outside sources
    // >invuln (yay)
    // >wait
    // >invuln (nay)
    public void ShieldInvuln(bool toggle)
    {
        if (toggle)
        {
            invuln = true;
            clr.a = opaque + 0.15f;
            clr.g = 0.9f;
            clr.b = 0.8f;
            TweenColor(clr, 0.2f, Ease.Linear);
        }
        else
        {
            invuln = false;
            // fade to ordinary colour
            TweenColor(clr, 1, Ease.Linear);
        }
        
    }
}
