using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct TurretRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //Debug.Log("Create");
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        //Debug.Log("OnDestroy");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rotation = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);

        foreach (var (transform, localToWorld) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<LocalToWorld>>().WithAll<Turret>())
        {
            transform.ValueRW = transform.ValueRO.Rotate(rotation);
        }

    }
}
