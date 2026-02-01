using UnityEngine;

public class LevelStartButton : MonoBehaviour
{
    // Call level start within a gamemanager instance
    // primarily used in-level

    // Set level index in editor
    public int levelNumber;

    // NUMBER CHANGED VIA OTHER SCRIPTS then passed onto GameManager
    public void GmInstanceLevelStart()
    {
        if(levelNumber == 0)
            GameManager.instance.StartLevel("MenuScene");
        else
            GameManager.instance.StartLevel("Level"+levelNumber);
    }

    public void GmLevelRestart()
    {
        GameManager.instance.StartLevel(GameManager.instance.currentLevel);
    }

}
