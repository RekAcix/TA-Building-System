using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManagerScript : MonoBehaviour
{
    [SerializeField]
    private BuildingSystemManager buildingSystem;

    private float[] scale = new float[3];
    private float increment = 0.5f;

    void Awake()
    {
        SetInitialScale();
    }

    public void IncreaseScale(int index)
    {
        scale[index] += increment;
        buildingSystem.UpdateObjectScales(scale);
    }

    public void DecreaseScale(int index)
    {
        scale[index] -= increment;
        buildingSystem.UpdateObjectScales(scale);
    }

    public void RevertScale()
    {
        SetInitialScale();
        buildingSystem.UpdateObjectScales(scale);
    }

    public void SetInitialScale()
    {
        scale[0] = 1f;
        scale[1] = 1f;
        scale[2] = 1f;
    }

}
