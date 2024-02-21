using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class CannonBallAuthoring : MonoBehaviour
{
}

class CannonBallBaker : Baker<CannonBallAuthoring>
{
    public override void Bake(CannonBallAuthoring authoring)
    {
        AddComponent<CannonBall>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}