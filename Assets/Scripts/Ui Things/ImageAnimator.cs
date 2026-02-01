using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using System.Collections;
public class ImageAnimator : MonoBehaviour
{
    // Small script which animates an UI image
    // used by the loadout menu plane

    // Set in editor
    [Tooltip("Array of sprites to rifle through")]
    [SerializeField]
    Sprite[] sprites;

    // Set in editor
    [SerializeField]
    Image img;

    // yeah i guess just construct it once to save 0.2% cpu
    int i = 0;
    public float frameRate = 0.85f;
    private Coroutine animationCoroutine;

    void OnEnable()
    {
        animationCoroutine = StartCoroutine(Anim());
    }

    void OnDisable()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }

    IEnumerator Anim()
    {
        // construct it only once 
        WaitForSeconds wait = new WaitForSeconds(frameRate);

        while (true)
        {
            // Seriously? Unity doesnt come with a built in way to animate UI??????
            // Are you kidding me? Are you pulling my leg??? 
            for (i = 0; i < sprites.Length; i++)
            {
                img.sprite = sprites[i];
                // modulo 
                i = (i + 1) % sprites.Length;
                yield return wait;
            }
        }
    }
}
