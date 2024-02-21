using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraSystem : SystemBase
{
    Entity Target;
    Unity.Mathematics.Random Random;
    EntityQuery TanksQuery;

    protected override void OnCreate()
    {
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        TanksQuery = GetEntityQuery(typeof(Tank));
        RequireForUpdate(TanksQuery);
    }

    protected override void OnUpdate()
    {
        if (Target == Entity.Null || Input.GetKeyDown(KeyCode.Space))
        {
            var tanks = TanksQuery.ToEntityArray(Allocator.Temp);
            Target = tanks[Random.NextInt(tanks.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var tankTransform = GetComponent<LocalTransform>(Target);
        cameraTransform.position = tankTransform.Position - 10.0f * tankTransform.Forward() + new float3(0.0f, 1.5f, 0.0f);
        cameraTransform.LookAt(tankTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}