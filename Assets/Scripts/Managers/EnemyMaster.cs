using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using Dreamteck.Splines;


// Keeps track of all enemies within a stage and manages waves
// Manages enemy drops also (so we dont have to put 5 different references on each enemy)
public class EnemyMaster : MonoBehaviour
{

    public int currentWave = 0;

    // Current enemies - List them via enemy components to avoid unnecessary use of getcomponent
    // not like the gameobject has anything for us to use anyway
    public List<EnemyBase> activeEnemies = new List<EnemyBase>();
    public List<AmbientEnemy> ambientEnemies = new List<AmbientEnemy>();

    public GameObject[] waves;
    //current wave - all ships are children to wave parent
    public GameObject activeWave;
    Player player;

    float shootDelay = 0;
    LevelUI lui;
    public Resource[] coins;
    public Pickup[] pickups;

    public SplineComputer[] aSplines;

    

    void Awake()
    {
        aSplines = GameObject.Find("AMBIENT SPLINES").GetComponentsInChildren<SplineComputer>();
    }

    void Start()
    {
        lui = FindAnyObjectByType<LevelUI>();
    }

    public void StartLevel()
    {
        player = GameObject.FindAnyObjectByType<Player>();
        //make the waves go away during load time
        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].transform.position += new Vector3(30 + i * 4, 0, 0);
        }
        Invoke(nameof(TrySpawnWave), 3);
    }

    //enemy will call this when it dies
    public void TrySpawnWave()
    {
        if (activeEnemies.Count <= 0)
            StartCoroutine(SpawnNewWave());

    }

    IEnumerator SpawnNewWave()
    {
        // DESTROY ambient enemies of current wave to make way for the next wave
        // they only do this offscreen. Allows them to exist while moving on a spline
        foreach (AmbientEnemy go in ambientEnemies)
        {
            go.GetComponent<AmbientEnemy>().DestroyAmbient();
        }

        // WIN CONDITION //
        if (currentWave >= waves.Count())
        {
            WaitForFixedUpdate wf = new();
            float timer = 0;

            // player no longer accepts firerate booster pickups
            player.firing = false;

            yield return new WaitForSeconds(0.5f);


            // wait for either 4 seconds or until all ambient enemies die
            while(ambientEnemies.Count > 0)
            {
                timer += Time.deltaTime;
                if(timer > 3)
                    break;

                yield return wf;
            }

            // coins get collected rapidly upon level end
            foreach (Pickup coin in FindObjectsByType<Pickup>(FindObjectsSortMode.None))
            {
                // dont drag offscreen coins up lol
                if(coin.transform.position.y > -11)
                    coin.StartCoroutine(coin.ForceFollowPlayer());
            }

            player.ToggleFiring(false);
            player.PlayerInvuln(true);


            // player has a lil bit of time to collect stuff b4 stage end
            yield return new WaitForSeconds(2);
            player.moving = false;


            // Level clear window pop up
            lui.LevelClear();

            // Next stage unlocked through ui manager
            UIManager.instance.lb.LevelButtonCleared();


            yield break;
        }

        // simultaneously check for nulls or otherwise empty waves 
        if (!waves[currentWave])
        {
            print("WAVE IS NULL MORON. INDEX IS :" + currentWave);
            currentWave++;
            StartCoroutine(SpawnNewWave());
            yield break;
        }

        activeWave = waves[currentWave];

        //finds all enemies and activates them (starts splinecoroutine and enables them)
        FindNextEnemies(activeWave);

        currentWave++;
    }


    //find all enemy gameobjects within the wave
    //ignores the need for tags and checks for EnemyBase
    private void FindNextEnemies(GameObject wave)
    {

        //base and ambient enemy index for sprite layering
        int i = 1;
        int t = -25;

        // Ambient enemy indexer for queuing on the same spline
        int a = 0;

        //Find all the children of the wave
        foreach (Transform go in wave.GetComponentsInChildren<Transform>())
        {
            // check for enemybase component within go
            // directly iterate for the enemy base component instead of the gameobject
            // target 
            if (go.TryGetComponent(out BasicEnemy enemy))
            {
                activeEnemies.Add(enemy);

                //make the first enemy have priority within the enemy layer
                //prevents awkward clipping of textures when overlapping
                go.GetComponent<SpriteRenderer>().sortingOrder = i;

                //tell the enemy its time to rumble
                enemy.Activate();


                i--;
            }
            else if (go.TryGetComponent(out AmbientEnemy ambient))
            {
                ambientEnemies.Add(ambient);

                ambient.index = a;
                a++;

                go.GetComponent<SpriteRenderer>().sortingOrder = t;
                ambient.Activate();

                t--;
            }

            // Try to find the enemy base as a fallback. Only activate it
            else if (go.TryGetComponent(out EnemyBase eb))
            {
                eb.Activate();
            }

            // make enemy shields pronounced when they enter
            if(go.TryGetComponent(out Shield shield))
                if(shield.health > 0)
                    shield.HitTweener();

        }
        //when wave is fully spawned (tho out of bounds at this point), player will begin firing
        if (!player.firing)
            player.ToggleFiring(true);

        StartCoroutine(WaveShoot());
    }

    //Some enemies within a wave will be queued to fire a bullet
    IEnumerator WaveShoot()
    {
        while (activeEnemies.Count > 0)
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                //f1 = Random.Range(1f,2f+i);
                //f2 = Random.Range(2f+i,3f+i);
                shootDelay = Random.Range(0.7f, 2.8f);
                if (activeEnemies[i])
                {
                    
                    // if selected enemy is frozen or something then reduce delay and move onto next one
                    if(!activeEnemies[i].firing)
                    {
                        shootDelay *= 0.8f;
                    }
                    else
                        activeEnemies[i].EnemyShoot();
                    yield return new WaitForSeconds(shootDelay);
                }
            }
        }
    }

}