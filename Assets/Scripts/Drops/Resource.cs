using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;


// coin class
public class Resource : Pickup 
{
    public string typeOf;

    // Coin action is simply adding resources
    protected override void PickupAction()
    {
        player.lui.AddResources(value,"coin");
    }
}