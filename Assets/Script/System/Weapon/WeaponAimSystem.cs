using HANDFORCE.TCCavy.Aim.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim
{
    partial struct WeaponAimSystem: ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity radicleEntity = SystemAPI.GetSingletonEntity<AimRadicle>();
            LocalTransform radicleTransform = SystemAPI.GetComponent<LocalTransform>(radicleEntity);
            foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithPresent<GunBase>())
            {
                float3 relativePos = radicleTransform.Position- transform.ValueRW.Position;
                quaternion start = transform.ValueRO.Rotation;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(relativePos, math.up());


            }
        }
    }
}
