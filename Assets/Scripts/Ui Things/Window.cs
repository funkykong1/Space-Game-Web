using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Window : MonoBehaviour
{
    public Sprite sprite;
    public TMP_Text primary, secondary, tertiary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }
}
