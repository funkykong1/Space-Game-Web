using UnityEngine;

public class PickupShield : Pickup
{
    public int shieldAmount;
    protected override void PickupAction()
    {
        if(player.shield != null)
            if(player.shield.health < player.shield.maxHealth)
                player.shield.ShieldAdd(shieldAmount);
            else
                SellPickup();
    }
}

