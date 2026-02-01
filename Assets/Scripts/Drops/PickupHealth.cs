using UnityEngine;

public class PickupHealth : Pickup
{
    // todo move off heart integer system
    private float healAmount = 1;
    protected override void PickupAction()
    {
        if(player.health.currentHealth == player.health.maxHealth)
            SellPickup();
        else
        {
            player.Heal(healAmount);
        }
    }
}
