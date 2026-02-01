using UnityEngine;
using System.Collections;
using PrimeTween;

public class HealthManager : MonoBehaviour
{
    Player player;
    public GameObject heartPrefab;
    // hearts in scene // single heart variable // handy heart index number
    public Heart[] hearts;Heart ob; int j=-1;

    //PLAYER HEALTH VALUES
    public float maxHealth=6, currentHealth=6;
    //ABSOLUTE MAX AMOUNT OF HEARTS
    public int maxHearts = 6;
    bool fading;
    Transform heartParent;

    void Awake()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        //check for maxhealth errors
        if(maxHealth % 2 != 0)
            maxHealth += 1;

        currentHealth = maxHealth;
    }
    void Start()
    {
        heartParent = GameObject.Find("Heart Parent").transform;
        // find player using this method for once
        player = FindFirstObjectByType<Player>();

        // construct the array and give player max hp of 24
        // as in the ABSOLUTE MAX amount of health ever
        hearts = new Heart[maxHearts];

        // Instantiate hearts and assign them to array elements
        for(int i = 0; i < maxHealth; i+=2)
        {
            j++;
            ob = Helper.InstantiateHeart(heartPrefab, heartParent, 2);
            hearts[j] = ob;
        }
        StartCoroutine(FadeHearts());
    }

    public void UpdateHearts(float value, bool healing)
    {

        
        if(fading)
        {
            StopCoroutine(FadeHearts());
            fading = false;
        }

        //if value is healing
        if(healing)
        {
            //if hp is already max then frig off
            if(currentHealth == maxHealth)
                return;

            for(int i = 0; i <= j; i++)
            {
                //make all of them snap back to opacity 1
                hearts[i].FadeStop();

                switch(hearts[i].health)
                {
                    // heart is empty
                    case 0:
                    
                    // check the healing amount and add it to the heart (always either 1 or 2)
                    // The value isnt added, it is SET
                    if(value > 1)
                    {
                        hearts[i].UpdateHeart(2);
                        value -= 2;
                    }
                    else
                    {
                        hearts[i].UpdateHeart(1);
                        value--;
                    }

                    break;

                    // heart is half full
                    case 1:
                        hearts[i].UpdateHeart(2);
                        value--;
                    break;

                    default:
                    break;
                }

                // if heart is full it will be skipped
                // end the for loop if all healing has been applied
                if(value == 0)
                    break;
            }
        }

        // Damage here 
        // if bullet damage is 2, reduce heart hp to 0
        // remainder is applied to next heart in array
        else
        {
            // iterate through the hearts in reverse
            // value starts at a negative. Add the amount of damage to it until it reaches 0
            currentHealth -= value;
            for(int i = j; i >= 0; i--)
            {
                hearts[i].FadeStop();
                switch(hearts[i].health)
                {
                    //heart is full
                    case 2:
                    
                    // The amount of damage dealt is checked here anyway. The amount will either be 0 or 1
                    if(value > 1)
                    {
                        hearts[i].UpdateHeart(0);
                        value -= 2;
                    }
                    else
                    {
                        hearts[i].UpdateHeart(1);
                        value--;
                    }

                    break;

                    // heart is half full
                    case 1:
                        hearts[i].UpdateHeart(0);
                        value--;
                    break;

                    // if heart is empty skip it
                    default:
                    break;
                }

                // end the for loop if all damage has been applied (value is 0)
                if(value == 0)
                    return;
            }
        }
        //regardless of healing or damage, start fading hearts out
        if(currentHealth > 0)
            StartCoroutine(FadeHearts());
    }
    //Hearts will fade out after a short delay. Coroutine is cancelled by health addition or loss
    IEnumerator FadeHearts()
    {
        fading = true;
        yield return new WaitForSeconds(3);

        int p = 80;
        for(int i = j; i > 0; i--)
        {
            //if heart has health, make it more opaque
            //first heart is the strongest
            if(hearts[i].health > 0)
            {
                hearts[i].FadeHeart(p);
                if(p > 40)
                    p-= 20;
            }
            else
            {
                hearts[i].FadeHeart(40);
            }
        }
        fading = false;
    }


    // Function to add heart container and ensure the empty hearts are rightmost
    // Adds player health accordingly
    void AddHeart()
    {
        // give player more hp here
        maxHealth += 2;

        // max health12 always
        if(maxHealth > 12)
            maxHealth = 12;

        //if j is equal to heart array capacity, convert added heart to healing
        if(j >= hearts.Length-1)
        {
            UpdateHearts(2, false);
            return;
        }

        //if player is at full health, just instantiate a new heart
        if(hearts[j].health == 2)
        {
            ob = Helper.InstantiateHeart(heartPrefab, heartParent, 2);
            j++;
            hearts[j] = ob;
        }
        else
        {
        
            //  updatehearts already calls this so put it here to avoid a redundant call
            if(fading)
            {
                StopCoroutine(FadeHearts());
                fading = false;
            }


            Heart badHeart=null;
            int p = 0;
            //get one non full heart
            for(int i = 0; i > j; i++)
            {
                hearts[i].FadeStop();
                //update it to be full
                if(hearts[i].health != 2)
                    {
                        //find first non full heart
                        badHeart = hearts[i];
                        //note the value
                        p = badHeart.health;
                        //it is now full
                        badHeart.UpdateHeartDiscreet(2);

                        ob = Helper.InstantiateHeart(heartPrefab, heartParent, 2);
                        ob.health = p;
                    }
            }
            j++;
            StartCoroutine(FadeHearts());
            
        }

    }


}
