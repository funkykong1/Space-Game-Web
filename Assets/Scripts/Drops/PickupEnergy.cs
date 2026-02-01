using UnityEngine;

public class PickupEnergy : Pickup
{

    // Woah ! this is reusable
    [SerializeField]
    private StatusEffect effect;
    private Player plr;
    protected override void Start()
    {
        base.Start();
        plr = FindAnyObjectByType<Player>();
    }

    protected override void PickupAction()
    {
        // if the level is over then just sell the pickup
        if(plr.firing)
            player.AddEffect(effect);
        // otherwise adds a firerate modifier status effect to player :-)
        else
            SellPickup();
    }
}
