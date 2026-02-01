using UnityEngine;
using System.Collections;
using Unity;
using UnityEditor.Animations;
public class Helper 
{
    public static Heart InstantiateHeart(Object original, Transform parent, int message)
    {
        GameObject obj = Object.Instantiate(original, parent) as GameObject;
        Heart heart = obj.GetComponent<Heart>();
        heart.OnCreated(message);
        return heart;
    }

    // create new gameobject as overlay - use target's sprite renderer as reference for the sprite layer
    // if editing transparency, edit the color of the sprite before calling this
    public static void AddOverlay(GameObject target, Sprite sprite, int spriteLayerChange)
    {
        if(!target.TryGetComponent<SpriteRenderer>(out SpriteRenderer targetRend))
        {
            Debug.Log("OVERLAY TARGET HAS NO SPRITE RENDERER. OVERLAY NOT ADDED!!");
            return;
        }

        // Create overlay gameobject and give it a spriterenderer + rect transform -> for size altering
        GameObject olay = new GameObject("OVERLAY");
        SpriteRenderer rend = olay.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        RectTransform rect = olay.AddComponent(typeof(RectTransform))as RectTransform;

        //set the overlay in the correct position and as a child of target
        olay.transform.SetParent(target.transform);
        olay.transform.position = target.transform.position;

        // Set the rect transform size via target sprite renderer size
        rect.localScale = new Vector3(1,1,1);
        rect.sizeDelta = targetRend.size;


        // Disable renderer and apply changes to it. enable when done
        rend.enabled = false;
        rend.drawMode = SpriteDrawMode.Sliced;
        rend.sprite = sprite;
        
        rend.size = targetRend.size;

        rend.sortingOrder = targetRend.sortingOrder + spriteLayerChange;
        rend.enabled = true;

        // Used for deletion
        olay.tag = "Overlay";
    }
    

    // use this when adding an animated overlay
    // WILL LIKELY NEED TO USE RESOURCES.LOAD (CREATE RESOURCES FOLDER WITH ANIMATOR CONTROLLER FILE)
    public static void AddOverlay(GameObject target, Sprite sprite, int spriteLayerChange, AnimatorController anim)
    {
        if(!target.TryGetComponent<SpriteRenderer>(out SpriteRenderer targetRend))
        {
            Debug.Log("OVERLAY TARGET HAS NO SPRITE RENDERER. OVERLAY NOT ADDED!!");
            return;
        }



        // Create overlay gameobject and give it a spriterenderer + rect transform -> for size altering
        GameObject olay = new GameObject("OVERLAY");
        SpriteRenderer rend = olay.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        RectTransform rect = olay.AddComponent(typeof(RectTransform))as RectTransform;

        //set the overlay in the correct position and as a child of target
        olay.transform.SetParent(target.transform);
        olay.transform.position = target.transform.position;

        // Set the rect transform size via target sprite renderer size
        rect.localScale = new Vector3(1,1,1);
        rect.sizeDelta = targetRend.size;


        // Disable renderer and apply changes to it. enable when done
        rend.enabled = false;
        rend.drawMode = SpriteDrawMode.Sliced;
        rend.sprite = sprite;
        
        rend.size = targetRend.size;

        rend.sortingOrder = targetRend.sortingOrder + spriteLayerChange;
        rend.enabled = true;








        // add animator component to overlay and set it's controller as anim
        Animator anim2 = olay.AddComponent(typeof(Animator))as Animator;
        anim2.runtimeAnimatorController = anim;

        // Used for deletion
        olay.tag = "Overlay";
    }

    // NOTE : this has to be deleted via animation event or manually calling delete and the name
    // TODO : put this in UI MANAGER list
    public static void AddOverlay(Vector3 pos, Vector2 size, Sprite sprite, int spriteLayerChange, int spriteLayer, AnimatorController anim)
    {
        GameObject olay = new GameObject("STANDALONE OVERLAY");
        SpriteRenderer rend = olay.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        RectTransform rect = olay.AddComponent(typeof(RectTransform))as RectTransform;

        //Give the standalone overlay values from the parameters
        rect.sizeDelta = size;
        olay.transform.position = pos;
        olay.tag = "Overlay";
    }

    // Remove all overlays within a set target
    public static void RemoveOverlays(GameObject target)
    {
        SpriteRenderer[] rends = target.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < rends.Length; i++)
        {
            if(rends[i].CompareTag("Overlay"))
            {
                GameObject.Destroy(rends[i].gameObject);
            }
                
        }
    }

    
}



