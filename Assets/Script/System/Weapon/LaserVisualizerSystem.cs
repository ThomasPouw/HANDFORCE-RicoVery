using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Controller.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim
{
    partial struct LaserVisualizerSystem : ISystem
    {
         EntityQuery laserSelectorQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            laserSelectorQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<newLaser>().Build(ref state);
            state.RequireForUpdate<RawControllerInput>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if(!laserSelectorQuery.IsEmpty)
            {
                RawControllerInput rawInput = SystemAPI.GetSingleton<RawControllerInput>();
                NativeArray<Entity> laserSelectorEntity = laserSelectorQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity entity in laserSelectorEntity )
                {
                    LaserSelector laserSelector = SystemAPI.GetComponent<LaserSelector>(entity);
                    RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    if(laserSelector.reversed)
                        localTransform.ValueRW.Rotation = quaternion.Euler(new float3(math.radians(180-(90 * math.log2((uint)rawInput.laserDirection ))), 0,math.radians(180)));
                    else
                        localTransform.ValueRW.Rotation = quaternion.Euler(new float3(math.radians((90 * math.log2((uint)rawInput.laserDirection ))), 0,math.radians(180)));
                    SystemAPI.SetComponentEnabled<newLaser>(entity, false);

                }
            }
            
        }
    }
}
