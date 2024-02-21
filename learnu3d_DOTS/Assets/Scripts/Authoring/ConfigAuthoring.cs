using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject TankPrefab;
    public int TankCount;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            TankPrefab = GetEntity(authoring.TankPrefab),
            TankCount = authoring.TankCount,
            SafeZoneRadius = authoring.SafeZoneRadius,
        });
    }
}
