using UnityEngine;



// This is just a class where we can use animationevents to play sounds instead of relying on audiosource play on awake
[RequireComponent(typeof(SpriteRenderer))]
public class AUDIOTHING : MonoBehaviour
{

    [SerializeField]
    private AudioClip clip;

    public void ANIMATOR_PlayAudio()
    {
        if(TryGetComponent<AudioSource>(out AudioSource audio))
        {
            // play explosion audio clip
            // importing multiple different audio files is bad practice
            // TODO: minor adjustments such as pitch to avoid repeat audio
            
            audio.pitch += Random.Range(-0.02f, 0.02f);
            audio.PlayOneShot(clip);
        }
        else
            print(gameObject.name + "TRIED PLAYING AUDIO VIA ANIMATOR WITHOUT A SOURCE!!");
    }

}
