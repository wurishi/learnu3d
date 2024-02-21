
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
partial struct TurretShootingSystem : ISystem
{
    ComponentLookup<LocalToWorld> m_LocalTransformFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_LocalTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalTransformFromEntity.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var turretShootJob = new TurretShoot {
            LocalTransformFromEntity = m_LocalTransformFromEntity,
            ECB = ecb,
        };
        turretShootJob.Schedule();
    }
    
}

//[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct TurretShoot : IJobEntity
{
    [ReadOnly]public ComponentLookup<LocalToWorld> LocalTransformFromEntity;
    public EntityCommandBuffer ECB;

    void Execute(TurretAspect turret)
    {
        var instance = ECB.Instantiate(turret.CannonBallPrefab);
        var spawnLocalToWorld = LocalTransformFromEntity[turret.CannonBallSpawn];
        ECB.SetComponent(instance, new LocalTransform { Position = spawnLocalToWorld.Position, Scale = 0.2f, });
        ECB.SetComponent(instance, new CannonBall { Speed = spawnLocalToWorld.Value.Forward() * 20.0f });
        ECB.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = turret.Color });
    }
}