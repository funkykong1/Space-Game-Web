using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using PrimeTween;
public class LoadoutButton : MonoBehaviour
{
    // Class for loadout buttons. Trimmed down version of a menu button which also contains an item to be selected
    // Interacts with loadout manager directly
    public Sprite[] sprites;
    Image img;
    LoadoutManager lm;
    [HideInInspector]
    public Transform gun;
    public GameObject item;

    public bool pressed;
    Tween itemTween;
    
    void Awake()
    {
        pressed = false;
        gun = transform.GetChild(0);
        // get everything and make the initial sprite ZERO!!! in the array
        img = GetComponent<Image>();
        img.sprite = sprites[0];

        lm = GameObject.Find("LoadoutManager").GetComponent<LoadoutManager>();

    }

    public void SelectItem()
    {
        StartCoroutine(itembuttoncoroutine());
    }


    // tells loadoutmanager which item is being equipped
    // Doesnt need unity events. Need only set the button's item

    // If this is already selected, deselect it and null current item
    // If something else is already selected, deselect it
    IEnumerator itembuttoncoroutine()
    {
        // Deselect and null current item if pressed twice
        if(pressed)
        {
            pressed = false;
            lm.selectedButton = null;
            lm.currentItem = null;
            lm.itemSelected = false;
            yield break;
        }

        // if already another button selected, tell it it's time in the spotlight is up
        // This part is purely visual
        if(lm.selectedButton)
        {
            lm.selectedButton.GetComponent<LoadoutButton>().pressed = false;
        }

        pressed = true;

        // set the sprite
        img.sprite = sprites[1];

        
        lm.selectedButton = this;
        lm.currentItem = item;

        lm.SelectItem();


        //randomize the start pos of item tween
        int i = Random.Range(0,2);
        int t;
        if(i == 0)
        {
            i = -30;
            t = -60;
        }
        else
        {
            i = -60;
            t = -30;
        }

        if(itemTween.isAlive)
            itemTween.Stop();
        
        itemTween = Tween.LocalRotation(gun, new Vector3(0,0,-45), new Vector3(0,0,i), duration:0.35f, ease:Ease.InOutSine);

        while(itemTween.isAlive)
        {
            if(!pressed)
            {
                itemTween.Stop();
                Tween.LocalRotation(gun, new Vector3(0,0,-45), 0.4f,ease:Ease.OutSine);
                img.sprite = sprites[0];
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        itemTween = Tween.LocalRotation(gun, new Vector3(0,0,i), new Vector3(0,0,t), duration:0.7f, ease:Ease.InOutSine, cycleMode:CycleMode.Yoyo, cycles:-1);

        //wait until this is deselected
        yield return new WaitUntil(() => !pressed);

        itemTween.Stop();
        itemTween = Tween.LocalRotation(gun, new Vector3(0,0,-45), 0.4f,ease:Ease.OutSine);
        // reset values :p
        img.sprite = sprites[0];


    }

}

