using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    LoadoutManager manager;
    public GameObject equippedItem;
    [NonSerialized]
    public Image rend, display;
    public SpriteRenderer shipItem;
    public Sprite[] sprites;
    Color opaque = new Color(1,1,1,1), trans = new(1,1,1,0);

    void Awake()
    {
        manager = FindFirstObjectByType<LoadoutManager>();
        rend = GetComponent<Image>();
        display = transform.GetChild(0).GetComponent<Image>();
        
        
        // if(!shipItem)
        // {
        //     print("forgot ship item for " + gameObject);
        //     Destroy(gameObject);
        //     return;
        // }


        // if(equippedItem)
        // {
        //     display.sprite = equippedItem.GetComponent<Image>().sprite;
        //     shipItem.sprite = equippedItem.GetComponent<Image>().sprite;
        //     rend.sprite = sprites[2];
        //     display.color = opaque;
        //     shipItem.color = opaque;
        // }
        // else
        // {
        //     rend.sprite = sprites[0];
        //     display.color = trans;
        //     shipItem.color = trans;
        //     display.sprite = null;
        //     shipItem.sprite = null;
        // }


    }
    // if there's an item currently selected then clicking on this will equip that item on a slot
    void OnMouseDown()
    {
        UpdateItem();
    }



    //if pressed with an item, equip it. If pressed without an item
    // highlighted state handled by loadout manager
    public void UpdateItem()
    {
        if(manager.currentItem)
        {
            display.sprite = manager.selectedButton.gun.GetComponent<Image>().sprite;
            shipItem.sprite = manager.selectedButton.gun.GetComponent<Image>().sprite;

            rend.sprite = sprites[2];

            display.color = opaque;
            shipItem.color = opaque;

            equippedItem = manager.currentItem;
        }
        else
        {
            display.color = trans;
            shipItem.color = trans;

            display.sprite = null;
            shipItem.sprite = null;

            rend.sprite = sprites[0];

            equippedItem = null;
        }
        manager.UpdateItems();
    }
}
