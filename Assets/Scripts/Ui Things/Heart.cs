using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PrimeTween;
public class Heart : MonoBehaviour
{
    public int health = 2;
    private Animator anim;
    private Image rend;
    // we only ever want one of the color tweens alive at a time so create a reference for it
    Tween tween;
    Color clr;

    void Awake()
    {
        rend = GetComponent<Image>();
        anim = GetComponent<Animator>();
        rend.enabled = false;
    }
    public void OnCreated(int hp)
    {
        //if starting health doesnt match with the default sprite (full heart), disable renderer
        if(hp == 2)
        {
            anim.SetInteger("Health", hp);
            rend.enabled = true;
        }
            
        else
            UpdateHeartDiscreet(hp);
    }

    //heart health updated here
    public void UpdateHeart(int hp)
    {
        health = hp;
        
        anim.SetInteger("Health", health);
    }

    // update heart without playing the animation
    // Used by heart manager adding a heart
    public void UpdateHeartDiscreet(int hp)
    {
        health = hp;

        switch (hp)
        {
            case 0:
                // Immediately jump to the final frame of the animation 
                anim.Play("HeartLossFull", 0, 1);
                break;

            case 1:
                anim.Play("HeartLossHalf", 0, 1);
                break;

            case 2:
                anim.Play("HeartGainEmptyFull", 0, 1);
                break;
        }

        //enable IMAGE if its off
        if (!rend.enabled)
            rend.enabled = true;

        anim.SetInteger("Health", hp);
    }

    //fade heart out to a target opacity
    public void FadeHeart(float opacity)
    {
        clr = new Color(1,1,1,opacity);
        tween = Tween.Color(rend, clr, 0.5f, Ease.Linear);
    }
    //instantly stop fading and set the opacity to max
    public void FadeStop()
    {
        if(clr.a != 1)
        {
            clr = new Color(1,1,1,1);
            tween = Tween.Color(rend, clr, 0.1f, Ease.Linear);
        }
    }




}
