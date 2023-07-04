using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ShapeEntry", menuName = "ScriptableObjects/ShapeUIEntry", order = 1)]
public class ShapeUIScriptableObject : ScriptableObject
{
    public GameObject referencedObject;
    public Sprite referenceImage;
}
