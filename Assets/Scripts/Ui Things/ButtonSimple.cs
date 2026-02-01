using UnityEngine;
using UnityEngine.Events;

//simple version of button thing which doesnt account for animations
public class ButtonSimple : MonoBehaviour
{
    public bool pressable;

    [SerializeField]
    private UnityEvent buttonEvent;

    void Awake()
    {
        pressable = true;
        if(buttonEvent == null)
        {
            print("no button event set for " + gameObject);
            Destroy(gameObject);
        }
    }


    void OnMouseDown()
    {
        if(!pressable)
            return;

        buttonEvent.Invoke();
        
    }
}
