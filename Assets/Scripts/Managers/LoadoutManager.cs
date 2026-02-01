using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    //MANAGES PLAYER WEAPONS OUTSIDE LEVELS - ADDS THEM TO PLAYER
public class LoadoutManager : MonoBehaviour
{

    public bool itemSelected = false;
    // item slots list
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    // Loadout gun buttons list
    public List<LoadoutButton> loadoutButtons = new List<LoadoutButton>();

    public GameObject currentItem;
    public LoadoutButton selectedButton;
    public Sprite overlaySprite;
    public Animator overlayAnim;

    //list for equipped player items
    public GameObject[] equippedWeapons;
    private Player player;
    public static LoadoutManager instance;

    void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        equippedWeapons = new GameObject[2];
        //DontDestroyOnLoad(gameObject);

        GameObject loadoutParent = GameObject.Find("Player Loadout Parent");

        //todo use find objects of type and turn this into an array
        foreach (ItemSlot bt in loadoutParent.GetComponentsInChildren<ItemSlot>())
        {
            itemSlots.Add(bt);
        }

        //get every loadout button in Loadout Menu
        foreach(LoadoutButton bt in loadoutParent.transform.GetComponentsInChildren<LoadoutButton>())
        {
            loadoutButtons.Add(bt);
        }
    }

    public void SelectItem()
    {
        if(!itemSelected)
            StartCoroutine(selectanitem());
    }


    IEnumerator selectanitem()
    {
        itemSelected = true;


        // Set the item slot sprite to a highlighted state if it's empty
        foreach(ItemSlot go in itemSlots)
        {
            if(!go.equippedItem)
                go.rend.sprite = go.sprites[1];
        }

        yield return new WaitUntil(() => !itemSelected);

        foreach(ItemSlot it in itemSlots)
        {
            if(!it.equippedItem)
                it.rend.sprite = it.sprites[0];
        }
    }

    public void DeselectItem()
    {
        if(selectedButton != null)
        {        
            selectedButton.pressed = false;
            selectedButton = null;
            currentItem = null;
            itemSelected = false;
        }
    }

    // update size of internal equipped weapons list to match item slots and apply correct weapons
    // Loadout manager carries over the items from scene to scene
    public void UpdateItems()
    {
        // set array size if its too large or small
        if(equippedWeapons.Length != itemSlots.Count)
            equippedWeapons = new GameObject[itemSlots.Count];

        // Get all the weapons. Notably they should be indexed properly in editor so the ship matches the preview in level.
        // Indexing probably just means naming them slot 1, slot 2, and having them be descending in hierarchy
        for(int i = 0; i < itemSlots.Count; i++)
        {
            equippedWeapons[i] = itemSlots[i].equippedItem;
        }
    }
}
