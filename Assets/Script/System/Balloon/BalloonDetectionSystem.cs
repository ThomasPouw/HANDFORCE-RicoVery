using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.Controller.Data;
using HANDFORCE.TCCavy.General.Data;
using HANDFORCE.TCCavy.Stateful.Authoring;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering.VirtualTexturing;


namespace HANDFORCE.TCCavy.Aim.LoopSystem
{
    partial struct BalloonDetectionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //RadicleOnBalloonLookUp = SystemAPI.GetComponentLookup<RadicleOnBalloon>();
            //BalloonColourLookUp = SystemAPI.GetComponentLookup<BalloonColour>();
            state.RequireForUpdate<RawControllerInput>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            RawControllerInput rawInput = SystemAPI.GetSingleton<RawControllerInput>();
            if(rawInput.shoot)
            {
                Entity balloonEntity = Entity.Null;
                bool missed = true;
                foreach (var (stateFul, radicleOnBalloon, transform, entity) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<RadicleOnBalloon>, RefRO<LocalTransform>>().WithEntityAccess())
                {
                    if(stateFul.Length == 0)
                        continue;

                    StatefulTriggerEvent firstStateful = stateFul[0];
                
                    if(!(SystemAPI.HasComponent<RadicleOnBalloon>(firstStateful.EntityA) || SystemAPI.HasComponent<RadicleOnBalloon>(firstStateful.EntityB)))
                        continue;
                    if(!(SystemAPI.HasComponent<BalloonColour>(firstStateful.EntityA) || SystemAPI.HasComponent<BalloonColour>(firstStateful.EntityB)))
                        continue;

                    missed = false;
                    balloonEntity = SystemAPI.HasComponent<BalloonColour>(firstStateful.EntityA) ? firstStateful.EntityA : firstStateful.EntityB;
                    {
                        DynamicBuffer<BalloonCollectionBuffer> BCBuffer = SystemAPI.GetSingletonBuffer<BalloonCollectionBuffer>();
                        DynamicBuffer<ShootBuffer> SBuffer = SystemAPI.GetSingletonBuffer<ShootBuffer>();
                        Timer timer = SystemAPI.GetSingleton<Timer>();
                        BalloonData balloonData = SystemAPI.GetComponent<BalloonData>(balloonEntity);
                        BCBuffer.Add(new BalloonCollectionBuffer
                        {
                            balloonEntityID = balloonData.ID,
                            timeStamp = timer.Time,
                            spawningTimestamp = balloonData.spawnTime,
                            balloonColour = SystemAPI.GetComponent<BalloonColour>(balloonEntity).type,
                            isMovingBalloon = SystemAPI.HasComponent<MovingBalloon>(balloonEntity)
                        });
                        //get LaserDirection from laser
                        SBuffer.Add(new ShootBuffer
                        {
                            timeStamp = timer.Time,
                            locationOfShot = transform.ValueRO.Position,
                            balloonColour = radicleOnBalloon.ValueRO.balloonType,
                            balloonEntityID = balloonEntity.Index,
                        });
                    }

                    ecb.DestroyEntity(balloonEntity);
                }
                if(missed)
                {
                    Entity entity = SystemAPI.GetSingletonEntity<RadicleOnBalloon>();
                    DynamicBuffer<MissedShootBuffer> MSBuffer = SystemAPI.GetSingletonBuffer<MissedShootBuffer>();
                    RadicleOnBalloon radicleOnBalloon = SystemAPI.GetSingleton<RadicleOnBalloon>();
                    LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(entity);
                    Timer timer = SystemAPI.GetSingleton<Timer>();

                    {
                        //Calc the closestByBalloon and closestByColourBalloon
                        //ColliderCastInput
                    }
                    MSBuffer.Add(new MissedShootBuffer
                    {
                        timeStamp = timer.Time,
                        locationOfShot = transform.Position,
                        laserDirection = radicleOnBalloon.balloonType
                        /*
                            public float timeStamp;
                            public float3 locationOfShot;
                            public LaserDirection laserDirection;
                            public ShootBuffer closestByBalloon;
                            public ShootBuffer closestByColourBalloon;
                            public float closestByBalloonDistance;
                            public float closestByColourBalloonDistance;
                        */
                    });
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

        }

    }
}
