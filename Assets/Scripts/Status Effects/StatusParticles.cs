using UnityEngine;
using Unity.Mathematics;
using System;



// small class to ensure scalability and functionality of particle effects
public class StatusParticles : MonoBehaviour
{
    // emission of particles scales with size!
    public int baseEmissionSpeed  = 50;

    // set by planebase - corresponds with id of status effect
    public int id = 0;

    [SerializeField]
    private Sprite overlay;
    void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        SpriteRenderer rend = transform.parent.GetComponent<SpriteRenderer>();
        // get reference to shape module (no need to reassign)
        var shape = ps.shape;
        var emitter = ps.emission;

        // apply sprite to module
        shape.sprite = rend.sprite;
        // get averages of both axes of sprite size
        float halk = (rend.bounds.size.x + rend.bounds.size.y) / 2; 

        // Base sprite world size is 2x2 units, so adjust according to that
        emitter.rateOverTime = baseEmissionSpeed * halk/2;


        // if this applies a sprite overlay, apply it and set mask
        if(TryGetComponent(out SpriteMask mask))
        {
            mask.sprite = rend.sprite;
            GetComponent<SpriteRenderer>().sprite = overlay;
        }
        

    }

}
