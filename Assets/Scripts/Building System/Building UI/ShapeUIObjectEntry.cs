using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeUIObjectEntry : MonoBehaviour
{
    public GameObject referencedObject;
    public Image referencedObjectImage;
    private ShapeSelector shapeSelector;

    public void Initialize(ShapeUIScriptableObject entryData, ShapeSelector shapeSelector)
    {
        referencedObjectImage.sprite = entryData.referenceImage;
        referencedObject = entryData.referencedObject;
        this.shapeSelector = shapeSelector;
    }

    public void SelectObject()
    {
        shapeSelector.SelectObject(referencedObject);
    }

}
