using UnityEngine;
using System.Collections;
using PrimeTween;


// Handles UI windows and screen scaling everywhere

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private static RectTransform rect;
    public static bool windowOpen, levelWindowOpen, tweening;
    [Header("manual")]
    public Window levelWindow;
    public GameObject gizmo;
    Tween moveTween;
    public LevelStartButton startButton;
    public GameObject hangarButton;
    [Header("Automated")]
    public LevelButton lb;
    public Window activeWindow;


    void Awake()
    {
        //destroy this if static instance already exists, static is unchanging and will always be the first instance it is set to
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        //DontDestroyOnLoad(gameObject);

        // main canvas rect
        rect = GetComponent<RectTransform>();

        // immediately hide level selector- skips the animation
        HideLevelSelector();

    }


    // Function which shows the level info window 
    // button passes level info along with the button itself
    public void ShowLevelWindow(int levelIndex, string prim, string sec, string tert, bool unlocked, LevelButton elbe)
    {

        startButton.levelNumber = levelIndex;

        if (!unlocked)
        {
            startButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(true);
        }

        // get references to current active window and current level button
        lb = elbe;
        activeWindow = levelWindow;

        levelWindow.primary.text = prim;
        levelWindow.secondary.text = sec;

        // Check for nulls because some levels might not have a tertiary text box
        if (levelWindow.tertiary != null)
            levelWindow.tertiary.text = tert;

        levelWindow.gameObject.SetActive(true);
    }   

    //TODO do this better
    
  /*
    public void ShowWindow(Window window, string prim, string sec, string tert)
    {
        if (activeWindow)
            return;

        activeWindow = window;



        window.primary.text = prim;
        window.secondary.text = sec;
        window.tertiary.text = tert;



        window.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        if (activeWindow)
        {
            activeWindow.gameObject.SetActive(false);
            activeWindow = null;
        }
    }
*/

    //public method for toggling the level select window
    public void ToggleLevelSelect()
    {
        if (!tweening)
            StartCoroutine(levelselectview());
    }
    private IEnumerator levelselectview()
    {
        tweening = true;
        // if its already open, close it
        if (levelWindowOpen)
        {
            LoadoutManager.instance.DeselectItem();
            //only tween the Y because no need to touch the other axis
            moveTween = Tween.LocalPositionY(gizmo.transform, gizmo.transform.localPosition.y - rect.rect.height, 0.6f, Ease.OutSine).OnComplete(() => tweening = false, true);
            yield return new WaitUntil(() => !tweening);
            levelWindowOpen = false;
        }
        
        // vice versa
        else
        {
            moveTween = Tween.LocalPositionY(gizmo.transform, 0, 0.6f, Ease.OutSine).OnComplete(() => tweening = false, true);
            yield return new WaitUntil(() => !tweening);
            levelWindowOpen = true;
            LoadoutManager.instance.DeselectItem();
        }
    }

    public void HideLevelSelector()
    {
        gizmo.transform.localPosition = new Vector3(0, -rect.rect.height, 0);
    }
}
