using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    private void Awake()
    {
        AddButtonSounds();
    }
    void AddButtonSounds()
    {
        Button[] buttonComponent = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttonComponent)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    void OnClick()
    {
        Debug.Log("sea");
        AudioManager.instance.Play("ButtonSound");
    }
}
