using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WithAll(typeof(Turret))]
[BurstCompile]
partial struct SafeZoneJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<Shooting> TurretActiveFromEntity;

    public float Squareradius;

    void Execute(Entity entity, LocalTransform transform)
    {
        //Debug.Log(math.lengthsq(transform.Position));
        //Debug.Log(Squareradius);
        var flag = math.lengthsq(transform.Position) > Squareradius;
        TurretActiveFromEntity.SetComponentEnabled(entity, flag);
    }
}

[BurstCompile]
partial struct SafeZoneSystem : ISystem
{
    ComponentLookup<Shooting> m_TurretActiveFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        m_TurretActiveFromEntity = state.GetComponentLookup<Shooting>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
        const float debugRenderStepInDegress = 20;

        for(float angle = 0;angle<360;angle += debugRenderStepInDegress)
        {
            var a = float3.zero;
            var b = float3.zero;
            math.sincos(math.radians(angle), out a.x, out a.z);
            math.sincos(math.radians(angle + debugRenderStepInDegress), out b.x, out b.z);
            UnityEngine.Debug.DrawLine(a * radius, b * radius);
        }

        m_TurretActiveFromEntity.Update(ref state);

        var safeZoneJob = new SafeZoneJob
        {
            TurretActiveFromEntity = m_TurretActiveFromEntity,
            Squareradius = radius * radius,
        };
        safeZoneJob.ScheduleParallel();
    }
}