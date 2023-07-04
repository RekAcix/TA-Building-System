using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeUITextureEntry : MonoBehaviour
{
    [SerializeField]
    public Texture texture;

    [SerializeField]
    private BuildingSystemManager buildingSystem;

    public void SetTexture()
    {
        buildingSystem.ChangeTexture(texture);
    }
}
