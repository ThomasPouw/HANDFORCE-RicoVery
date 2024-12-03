using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.Controller.Data;
using HANDFORCE.TCCavy.General.Data;
using HANDFORCE.TCCavy.Stateful.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


namespace HANDFORCE.TCCavy.Aim
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
                Timer timer = SystemAPI.GetSingleton<Timer>();
                bool missed = true;
                foreach (var (stateFul, radicleOnBalloon, transform, entity) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<ReticleOnBalloon>, RefRO<LocalTransform>>().WithEntityAccess())
                {
                    if(stateFul.Length == 0)
                        continue;

                    StatefulTriggerEvent firstStateful = stateFul[0];
                
                    if(!(SystemAPI.HasComponent<ReticleOnBalloon>(firstStateful.EntityA) || SystemAPI.HasComponent<ReticleOnBalloon>(firstStateful.EntityB)))
                        continue;
                    if(!(SystemAPI.HasComponent<BalloonData>(firstStateful.EntityA) || SystemAPI.HasComponent<BalloonData>(firstStateful.EntityB)))
                        continue;

                    Entity balloonEntity = SystemAPI.HasComponent<BalloonData>(firstStateful.EntityA) ? firstStateful.EntityA : firstStateful.EntityB;
                    BalloonData balloonData = SystemAPI.GetComponent<BalloonData>(balloonEntity);

                    if(balloonData.type == rawInput.laserDirection || balloonData.type == LaserDirection.Default)
                    {
                        Debug.Log($"firstStateful.ColliderKeyA.Value: {firstStateful.ColliderKeyA.Value} firstStateful.ColliderKeyB.Value: {firstStateful.ColliderKeyB.Value}");
                        missed = false;
                        DynamicBuffer<BalloonCollectionBuffer> BCBuffer = SystemAPI.GetSingletonBuffer<BalloonCollectionBuffer>();
                        DynamicBuffer<ShootBuffer> SBuffer = SystemAPI.GetSingletonBuffer<ShootBuffer>();
                        BCBuffer.Add(new BalloonCollectionBuffer
                        {
                            balloonEntityID = balloonData.ID,
                            timeStamp = timer.Time,
                            spawningTimestamp = balloonData.spawnTime,
                            balloonColour = balloonData.type,
                            isMovingBalloon = SystemAPI.HasComponent<MovingBalloon>(balloonEntity)
                        });
                        //get LaserDirection from laser
                        SBuffer.Add(new ShootBuffer
                        {
                            timeStamp = timer.Time,
                            locationOfShot = transform.ValueRO.Position.xy,
                            balloonColour = rawInput.laserDirection,
                            balloonEntityID = balloonData.ID,
                        });
                    }
                    else
                    {
                        missed = true;
                        continue;
                    }

                    ecb.DestroyEntity(balloonEntity);
                }
                if(missed)
                {
                    Entity entity = SystemAPI.GetSingletonEntity<ReticleOnBalloon>();
                    DynamicBuffer<MissedShootBuffer> MSBuffer = SystemAPI.GetSingletonBuffer<MissedShootBuffer>();
                    LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(entity);
                    ShootBuffer closestHitBalloon = default;
                    float closestHitDistance = 0;
                    ShootBuffer closestHitColouredBalloon = default;
                    float closestColouredDistance = 0;

                    {
                        PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
                        //Calc the closestByBalloon and closestByColourBalloon
                        PointDistanceInput closestBalloon = new PointDistanceInput
                        {
                            Position = transform.Position,
                            MaxDistance = 500,
                            Filter = new CollisionFilter
                            {
                                CollidesWith = 1 << 0 | 1 << 1 | 1 << 2| 1 << 3,
                                BelongsTo = ~0u
                            }
                        };
                        PointDistanceInput closestColouredBalloon = new PointDistanceInput
                        {
                            Position = transform.Position,
                            MaxDistance = 100,
                            Filter = new CollisionFilter
                            {
                                CollidesWith = (uint)rawInput.laserDirection,
                                BelongsTo = ~0u
                            }
                        };
                        if(physicsWorld.CalculateDistance(closestBalloon, out DistanceHit closestHit))
                        {
                            BalloonData data = SystemAPI.GetComponent<BalloonData>(closestHit.Entity);
                            closestHitDistance = closestHit.Distance;
                            closestHitBalloon = new ShootBuffer
                            {
                                timeStamp = timer.Time,
                                locationOfShot = closestHit.Position.xy,
                                balloonColour = data.type,
                                balloonEntityID = data.ID
                            };
                        }
                        if(physicsWorld.CalculateDistance(closestColouredBalloon, out DistanceHit closestColouredHit))
                        {
                            BalloonData data = SystemAPI.GetComponent<BalloonData>(closestColouredHit.Entity);
                            closestColouredDistance= closestColouredHit.Distance;
                            closestHitColouredBalloon = new ShootBuffer
                            {
                                timeStamp = timer.Time,
                                locationOfShot = closestColouredHit.Position.xy,
                                balloonColour = data.type,
                                balloonEntityID = data.ID
                            };
                        }


                        
                    }
                    MSBuffer.Add(new MissedShootBuffer
                    {
                        timeStamp = timer.Time,
                        locationOfShot = transform.Position.xy,
                        laserDirection = rawInput.laserDirection,
                        closestByBalloon = closestHitBalloon,
                        closestByColourBalloon = closestHitColouredBalloon,
                        closestByBalloonDistance = math.abs(closestHitDistance),
                        closestByColourBalloonDistance = math.abs(closestColouredDistance)
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
