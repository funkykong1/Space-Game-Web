using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Shield))]


// adds temporarily invincible shield to player
public class ShieldSuper : MonoBehaviour
{
    Shield shield;
    Player player;
    public int length = 5;
    public int shieldHealthSetter = 5;

    void Awake()
    {
        player = GetComponent<Player>();
        shield = GetComponent<Shield>();
    }

    protected void Super()
    {
        // do it in a coroutine to allow for waiting
        StartCoroutine(supa());
    }
    
    IEnumerator supa()
    {
        player.PlayerInvuln(true);
        shield.ShieldInvuln(true);

        yield return new WaitForSeconds(length);

        player.PlayerInvuln(false);
        shield.ShieldInvuln(false);
    }
}
