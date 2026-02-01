using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;

public class Notification : MonoBehaviour
{
    // set in editor
    public bool stackable;
    public string preface;
    Tween tween;
    LevelUI lui;

    float timer;
    public bool tweening;
    int onScreenx = 165, offScreenx=-350;

    public string type;
    
    [SerializeField]
    private TextMeshProUGUI targetText;

    void Start()
    {
        //targetText = GetComponent<TextMeshProUGUI>();
        lui = FindAnyObjectByType<LevelUI>();
        if (stackable)
        {
            StartCoroutine(CounterCoroutine());
        }
    }
    public void ChangeText(int input)
    {
        targetText.text = preface + input.ToString();
    }
    IEnumerator CounterCoroutine()
    {
        // bring notification onscreen
        tween = Tween.PositionX(transform, onScreenx, 0.7f, ease:Ease.Linear);

        //get current coins
        int count = lui.recentCoins;



        timer = 4;
        while (timer > 0)
        {
            // if coins change and this is still onscreen and not moving, reset timer
            if (count < lui.recentCoins)
            {
                timer = 4;
                count = lui.recentCoins;
                ChangeText(count);
                
            }
            yield return new WaitForFixedUpdate();
            timer -= Time.deltaTime;
        }

        tweening = true;
        // start tweening back out of the screen
        tween = Tween.PositionX(transform, offScreenx, 0.7f, ease:Ease.InSine).OnComplete(() => tweening = false);

        //reset recentcoins in level ui so the next notification starts at 0
        lui.recentCoins = 0;
        lui.activeNotifs.Remove(this);

        // Destroy gameobject and remove from list when its offscreen
        yield return new WaitUntil(() => !tweening);
        Destroy(gameObject);
    }
}
