using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLevels : MonoBehaviour
{
    // Manages level button colours in a world 
    public LevelButton[] levels;

    void Awake()
    {
        levels = GetComponentsInChildren<LevelButton>();
    }

}
