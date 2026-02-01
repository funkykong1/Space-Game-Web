using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Manages scene transitions and perhaps player stats
    public GameObject player;
    private static LoadoutManager lm;
    private static Camera cam;
    public static Animator transition;
    // IF scene change is happening
    public bool loading;
    EnemyMaster em;
    public static GameManager instance;
    private UIManager ui;
    public string currentLevel = "MenuScene";

    //TODO PLAYER SAVE FILE
    public int coins;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        //DontDestroyOnLoad(gameObject);
        transition = GameObject.Find("Transition").GetComponent<Animator>();
        //DontDestroyOnLoad(transition.gameObject);
        cam = FindAnyObjectByType<Camera>();

        lm = FindAnyObjectByType<LoadoutManager>();
        ui = FindFirstObjectByType<UIManager>();
    }

    //load scene :p
    public void StartLevel(string level)
    {

        if (!SceneExists(level))
        {
            print(level);
            print("NIL LEVEL");
            return;
        }

        // Used for level restarting
        currentLevel = level;

        if (level == "MenuScene")
        {
            StartCoroutine(menuSceneLoad());
            return;
        }
        //item indexer
        int l = 0;

        //check every equipped item. if all empty then dont start level
        for (int i = 0; i < lm.equippedWeapons.Length; i++)
        {
            if (lm.equippedWeapons[i])
                l++;
        }
        if (l > 0)
        {
            if (l > lm.equippedWeapons.Length)
            {
                print("WARNING ABOUT INSUFFICIENT WEAPONS. TODO");
            }
            StartCoroutine(levelStart(level));
        }

        //todo make a thing berating the player for trying to start level empty handed
        else
        {
            print("nice try lol. itemcount is " + l);
        }
    }

    // TODO cool loading screen
    IEnumerator levelStart(string level)
    {
        loading = true;
        //animation fade out and wait until its done fading out
        transition.SetTrigger("FadeOut");
        // wait a little bit so the animator processes the trigger and actually goes to the fade out animation
        yield return new WaitForEndOfFrame();


        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        // DISABLE UI Manager child gameobjects
        for (int i = 0; i < ui.transform.childCount; i++)
        {
            ui.transform.GetChild(i).gameObject.SetActive(false);
        }

        // move cam too lol cuz it doesnt destroy on load
        cam.transform.position = new Vector3(0, 0, cam.transform.position.z);

        // this loads the scene and awaits until its 100% done
        // so everything is instantiated here but only later set in motion via script
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // get enemy master for current level
        em = GameObject.FindAnyObjectByType<EnemyMaster>();

        Instantiate(player, new Vector3(0, -5, 0), Quaternion.identity);

        //give player time to initialize (loadoutmanager uses a thing from player)
        yield return new WaitForFixedUpdate();

        //when all this is done we can fade back in
        transition.SetTrigger("FadeIn");

        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        loading = false;

        em.StartLevel();


    }
    // menu scene loader
    IEnumerator menuSceneLoad()
    {
        loading = true;
        // fade out then wait until menu scene propa loaded

        transition.SetTrigger("FadeOut");

        //Wait so animator catches up
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);


        for (int i = 0; i < ui.transform.childCount; i++)
        {
            ui.transform.GetChild(i).gameObject.SetActive(true);
        }

        ui.levelWindow.gameObject.SetActive(false);
        ui.hangarButton.SetActive(true);

        var asyncLoadLevel = SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        cam.transform.position = new Vector3(0, 0, cam.transform.position.z);
        transition.SetTrigger("FadeIn");
        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        loading = false;


    }


    bool SceneExists(string sceneName)
    {   
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
