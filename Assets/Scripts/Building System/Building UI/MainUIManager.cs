using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    private bool toggledUI = true;

    [SerializeField]
    private GameObject mainUIObject;

    public void ToggleUI()
    {
        mainUIObject.SetActive(!toggledUI);
        toggledUI = !toggledUI;
    }

}
