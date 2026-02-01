using UnityEngine;
using UnityEngine.UI;


// UI Button which contains info about level number, name&desc&image and status
// This knows which buttons come after it (SET IN EDITOR !!)
public class LevelButton : MonoBehaviour
{
    public string levelName, levelDescription, levelTertiary;
    public int levelNumber;

    [SerializeField] // Private so it is only changed via the complete function
    private bool complete;
    public bool locked;
    public Sprite levelImage;

    public LevelButton[] nextLevels;
    public Color clearColor;

    void Awake()
    {
        if (levelNumber == 0)
        {
            print("LEVEL NUMBER ZERO ON" + gameObject);
            Destroy(gameObject);
        }

        UpdateButtonColors();
    }


    public void LevelButtonPress()
    {
        if (locked)
        {
            UIManager.instance.ShowLevelWindow(-1,"?????", "Unknown...", " ", false, this);
        }
        else
        {
            UIManager.instance.ShowLevelWindow(levelNumber, levelName, levelDescription, levelTertiary, true, this);
        }
    }

    public void LevelButtonCleared()
    {
        complete = true;
        if (nextLevels.Length == 0)
        {
            // Final level of world
            print("level without follow up = " + gameObject);
            return;
        }
        for (int i = 0; i < nextLevels.Length; i++)
            {
                nextLevels[i].locked = false;
                nextLevels[i].UpdateButtonColors();
            }
    }

    public void UpdateButtonColors()
    {
        // update this button's colour according to locked status
        Image img = GetComponent<Image>();
        Color clr = img.color;
        if (locked)
        {
            clr.a = 0.3f;
            img.color = clr;
        }
        else if (complete)
        {
            img.color = clearColor;
        }
        else
        {
            clr.a = 1;
            img.color = clr;
        } 
    }

}
