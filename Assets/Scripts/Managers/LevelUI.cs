using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Canvas))]


// Handler for in-level notifications and UI
// Also restarting a specific level
public class LevelUI : MonoBehaviour
{

    // ARRAY OF *prefab* NOTIFICATION GAMEOBJECTS --SET IN EDITOR--
    // 0-COIN
    public GameObject[] notifications;

    // Array of active notifications 
    // inexpensive way of adding resources to existing notifications
    // follows same indexing as the prefab array
    public List<Notification> activeNotifs = new List<Notification>(new Notification[5]);
    public int coinsTotal, recentCoins;
    float nextY;
    public GameObject levelClearWindow, gameOverWindow;

    public TextMeshProUGUI lootText;



    private IEnumerator Start()
    {
        GetComponent<Canvas>().targetDisplay = 0;
        yield return new WaitUntil(() => FindAnyObjectByType<Player>());
    
    }

    public void LevelClear()
    {
        StartCoroutine(moneytracker());
        levelClearWindow.SetActive(true);
    }

    IEnumerator moneytracker()
    {
        WaitForFixedUpdate wf = new();

        // silly way of updating the end screen player money
        while(true)
        {
            lootText.text = "Spoils: $ " + coinsTotal;
            yield return wf;
        }
            
    }

    public void AddResources(int amount, string type)
    {
        switch (type)
        {
            // Check for preexisting notification of matching type, create a new one if none exist
            // seems hideously expensive but itll do for now
            case "coin":
            if(activeNotifs.Count > 0)
                foreach (Notification notif in activeNotifs)
                {
                    if(notif.tweening)
                    {
                        ResourceNotification(amount, type);
                    }
                    else
                    {
                        notif.ChangeText(amount);
                    }
                }
            else
                ResourceNotification(amount, type);

                //Recent coins are used and reset by notification
                recentCoins += amount;
                coinsTotal += amount;
                break;
        }

    }


    // Non-resource notification TBA
    public void ShowNotification(string text, Color color)
    {
        //StartCoroutine(notifCR(text, color));
    }

    // Based on a preexisting gameobject - no need to pass text or color
    // THIS kind of sucks honestly and I will probably just make a permanent counter along with a +5 or something appear
    // todo make a counter and have notifications appear under it and do it more efficiently !!!
    private void ResourceNotification(int amount, string type)
    {
        Transform target;


        // get the position for the next pop up based on the previous one 
        if (activeNotifs.Count > 0)
        {
            // if a notification exists but its high then just go down
            if(activeNotifs[0].transform.position.y > 355)
                nextY = 350;
            else
                nextY = activeNotifs[^1].transform.position.y + 80;
        }
        else
        {
            nextY = 350;
        }

        // manually add new resources here lol
        // 2 switch statements isnt ideal but the notification is only called once
        switch (type)
        {
            case "coin":
                target = Instantiate(notifications[0], new Vector3(-350, nextY, 0), Quaternion.identity, transform).transform;
                //assign coin notification to active coin notif slot - new coins will be added to this one
                Notification notif = target.GetComponent<Notification>();
                activeNotifs.Add(notif);

                notif.ChangeText(amount);
                break;

            default:
                print("ADDED A RESOURCE WITHOUT A SET TYPE");
                return;
        }
    }

}
