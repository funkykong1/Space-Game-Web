using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
public class BUTTONTHING : MonoBehaviour
{
    // completely visual base class for ingame buttons
    public Sprite[] sprites;
    SpriteRenderer rend;
    // PRETTY SELF EXPLANATORY
    RectTransform child;
    [SerializeField]
    private UnityEvent buttonEvent;
    //array with both a color and offset, by default one child
    public ChildCustom[] custom = new ChildCustom[1];
    public ChildCustom[] temp = new ChildCustom[3];

    // how sticky the button is :-)
    public float pressDuration;
    public bool pressed, selectable;
    
    void Awake()
    {
        pressed = false;

        // get everything and make the initial sprite ZERO!!! in the array
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = sprites[0];

        // asks if buttonevent is null, if so, constructs an empty new one
        buttonEvent ??= new UnityEvent();

        if(!custom[0].child)
        {
            print("BUTTON " + gameObject + " HAS FUCKY SETTINGS!!!!");
            Destroy(gameObject);
        }

    }
    // TODO: turn this into a mobile thing... To you in a year's time
    IEnumerator OnMouseDown()
    {
        // if pressed and selectable, make the second button push deselect it
        if(pressed)
        {
            if(selectable)
            {
                pressed = false;
            }
            yield break;
        }

        pressed = true;

        // set the sprite
        rend.sprite = sprites[1];


        // THIS IS FUCKING MORONIC!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // every press of a button logs the color and position of a child gameobject

        // and also applies the changes to position and color from custom

        for(int i = 0; i < custom.Length; i++)
        {
            if(custom[i].child.TryGetComponent<TextMeshPro>(out TextMeshPro text))
            {
                temp[i].color = text.color;
                temp[i].offset = custom[i].child.localPosition;

                text.color = custom[i].color;
            }
            else
            {
                // most expensive button press award
                SpriteRenderer sprite = custom[i].child.GetComponent<SpriteRenderer>();
                temp[i].color = custom[i].color;
                temp[i].offset = custom[i].child.localPosition;


                sprite.color = custom[i].color;
            }
            custom[i].child.localPosition += custom[i].offset;
        }

        // do the thing the button is actually supposed to do lol
        buttonEvent.Invoke();

        // if button is a selectable, wait until it is pressed again or otherwise deselected
        // if its just a normal button then wait for press duration
        if(selectable)
        {
            yield return new WaitUntil(() => !pressed);
        }
        else
            // wait for a while
        {
            yield return new WaitForSeconds(pressDuration);
            pressed = false;
        }

        // reset values :p
        rend.sprite = sprites[0];

        //yea I know
        for(int i = 0; i < custom.Length; i++)
        {
            if(custom[i].child.TryGetComponent<TextMeshPro>(out TextMeshPro text))
            {
                text.color = temp[i].color;
                custom[i].child.localPosition = temp[i].offset;
            }
            else
            {
                SpriteRenderer sprite = custom[i].child.GetComponent<SpriteRenderer>();
                sprite.color = temp[i].color;
                custom[i].child.localPosition = temp[i].offset;
            }
        }

    }

}

[System.Serializable]
public class ChildCustom{
    public Transform child;
    public Color color;
    public Vector3 offset;
}
