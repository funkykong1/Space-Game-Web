using UnityEngine;
using System.Collections;

public class EnemyDetonator : EnemyBase
{

    /*  
        Suicide bomber enemy which steers toward the player and tries ramming them
        Gets destroyed when hitting level enemy activator triggers at Y < 0
        Basically is a one off per wave - disconnected from enemy master
    */ 

    [SerializeField]
    [Range(0.5f, 20)]
    private float turnSpeed = 4;

    [SerializeField]
    [Range(0.02f, 0.1f)]
    // speed and the extra speed added when it speeds uppp
    private float speedIncrease = 0.02f;

    Transform player;
    Vector3 start;
    float targetAngle;
    float currentAngle;
    Vector2 direction;
    float angle;

    protected override void Awake()
    {
        start = transform.position;
        base.Awake();
    }

    protected override void EnemyDeath()
    {
        base.EnemyDeath();
        //make an explosion and rotate it randomly
        Instantiate(shipExplosion, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));

        Destroy(gameObject, 0.15f);
    }


    public override void Activate()
    {
        player = FindAnyObjectByType<Player>().transform;
        StartCoroutine(activatethis());
    }

    private IEnumerator activatethis()
    {
        transform.position = start;
        WaitForFixedUpdate wf = new();


        // Ship comes down the screen and slowly rotates and moves toward player
        while (transform.position.y > 6)
        {
          //git_da_angle
            git_da_angle();

            // Move down 
            transform.Translate(new Vector3(0, speed, 0));

            yield return wf;
        }

        // fasta
        speed += speedIncrease;

        // turn even slower 
        turnSpeed /= 2;

        // gooo son
        while (transform.position.y > -14)
        {
            git_da_angle();
            transform.Translate(new Vector3(0, speed, 0));
            yield return wf;
        }


        // when its down lo enough just uhh destroy it
        Destroy(gameObject);

        yield return null;
    }

    void git_da_angle()
    {
        // direction2 target
        direction = player.position - transform.position;

        // calculate angle 
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        currentAngle = transform.eulerAngles.z;

        // rotate towards the target angle
        angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            // todo float hp system
            player.GetComponent<Player>().Damage(2);
        }
    }
}
