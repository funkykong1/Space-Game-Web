using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{

    // Basically just exists in the bootstrap scene which initializes everything once and is never entered again
    // Destroys itself after loading is completely done

    // now with a menu screen!

    public void PlayButton()
    {
        //StartCoroutine(play());
    }

    //IEnumerator play()
    IEnumerator Start()
    {
        // Give system scripts a frame of loadtime
        yield return new WaitForFixedUpdate();


        var asyncLoadLevel = SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        GameManager.transition.SetTrigger("FadeIn");

        Destroy(gameObject);
    }

    void Awake()
    {
        // Dont destroy this while a scene is loading !!
        DontDestroyOnLoad(gameObject);
    }
}
