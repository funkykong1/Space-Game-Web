using UnityEngine;


// this holds the record for the dumbest unity system Ive had the displeasure of working with. 17.12.25 16:50


// This is a storage container for reusable sound effects and prefabs such as explosions and gunfire


// Don't place this on objects which only require one reference, such as a ship explosion 
[CreateAssetMenu(fileName = "FXLibrary", menuName = "Scriptable Objects/FXLibrary")]
public class FXLibrary : ScriptableObject
{
    public GameObject explosionCollision;
    public GameObject explosionShip;

    public GameObject explosionMid;
    public AudioClip[] thunderImpacts;

    
    public AudioClip[] thunderFire;

}
