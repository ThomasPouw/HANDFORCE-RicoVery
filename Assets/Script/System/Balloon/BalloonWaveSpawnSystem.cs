using System;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using HANDFORCE.TCCavy.General.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HANDFORCE.TCCavy.Balloon
{
    [RequireMatchingQueriesForUpdate]
    public partial class BalloonWaveSpawnSystem : SystemBase //Make it a systembase as you need to implement an Action into it.
    {
        public static Action GameClear;
        private EntityQuery balloonQuery;
        private Entity parent;
        private World transferWorld;
        protected override void OnCreate()
        {
            balloonQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<BalloonData>().Build(ref CheckedStateRef);
            //CheckedStateRef.RequireForUpdate<BalloonWaveBuffer>();
            CheckedStateRef.RequireForUpdate<WaveSettings>();
        }

        protected override void OnUpdate()
        {
            if(transferWorld == null)
            {
                foreach(World world in World.All)
                {
                    Debug.Log(World.Name);
                    if(world.Name == "TransferDataWorld")
                    {
                        transferWorld = world;
                    }
                }
            }
            
            if(parent == Entity.Null)
                parent = SystemAPI.GetSingletonEntity<WaveSettings>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
           // Debug.Log(transferWorld);
            if(balloonQuery.IsEmpty || SystemAPI.IsComponentEnabled<WaveRequested>(parent))
            {
                //Debug.Log(transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)));
                transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).TryGetSingletonBuffer<BalloonWaveBuffer>(out DynamicBuffer<BalloonWaveBuffer> DynaBalloonWave);
                SystemAPI.TryGetSingleton<WaveSettings>(out WaveSettings waveSettings);
                Debug.Log(waveSettings);
                Debug.Log(DynaBalloonWave.Length +" "+ waveSettings.currentWaves);
                if(DynaBalloonWave.Length <= waveSettings.currentWaves)
                {
                    Debug.Log("Entered Result here");
                    SystemAPI.SetComponentEnabled<WaveRequested>(parent, false); 
                    GameClear?.Invoke();
                    return;
                }
                BalloonWaveBuffer balloonWaveBuffer = DynaBalloonWave[waveSettings.currentWaves];
                LocalTransform localTransform = SystemAPI.GetComponent<LocalTransform>(parent);
                Timer timer = SystemAPI.GetSingleton<Timer>();
                Debug.Log(balloonWaveBuffer.balloonSpawns.Value.balloonSpawns.Length);
                for (int i = 0; i < balloonWaveBuffer.balloonSpawns.Value.balloonSpawns.Length; i++)
                {
                    Debug.Log(balloonWaveBuffer.balloonSpawns.Value.balloonSpawns[i]);
                    BalloonSpawnEntity balloonSpawn = balloonWaveBuffer.balloonSpawns.Value.balloonSpawns[i];
                    Entity entity = Entity.Null;
                    //Entity entity = ecb.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                    switch (balloonSpawn.balloonType)
                    {
                        case LaserDirection.Default:
                            entity = ecb.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Up:
                            entity = ecb.Instantiate(waveSettings.upBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Right:
                            entity = ecb.Instantiate(waveSettings.rightBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Down:
                            entity = ecb.Instantiate(waveSettings.downBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Left:
                            entity = ecb.Instantiate(waveSettings.leftBalloonEntityToSpawn);
                            break;
                        default:
                            entity = ecb.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                            break;
                    }
                    float3 pos = balloonSpawn.location;
                    ecb.SetComponent(entity, new LocalTransform
                    {
                        Position = new float3(pos.x, pos.y, localTransform.Position.z),
                        Scale = 1
                    });
                    ecb.AddComponent(entity, new BalloonData
                    {
                        ID = entity.Index = balloonSpawn.ID,
                        type = balloonSpawn.balloonType,
                        spawnTime = timer.Time
                    });
                    ecb.AddComponent<BalloonTimer>(entity);
                    /*{
                        
                        CollisionFilter collisionFilter = new CollisionFilter();
                        collisionFilter.BelongsTo = (uint)0x0;
                        if(balloonSpawn.balloonType == LaserDirection.Default)
                        {
                            collisionFilter.CollidesWith = (uint)balloonSpawn.balloonType;
                        }
                        else
                        {
                            collisionFilter.CollidesWith = (uint)LaserDirection.Default | (uint)balloonSpawn.balloonType;
                        }
                        PhysicsCollider physicsCollider =SystemAPI.GetComponent<PhysicsCollider>(entity);
                        physicsCollider.Value.Value.SetCollisionFilter(collisionFilter);
                        SystemAPI.SetComponent<PhysicsCollider>(entity, physicsCollider);
                    }*/
                    //ecb.Playback(state.EntityManager);
                }

                {
                    TimeLeftWave T = SystemAPI.GetSingleton<TimeLeftWave>();
                    T.timeLeft = balloonWaveBuffer.waveTime;
                    SystemAPI.SetSingleton<TimeLeftWave>(T); //Leaving in the <T> to make it easier to read. It does not actually need it.
                    waveSettings.currentWaves++;
                    SystemAPI.SetComponentEnabled<WaveRequested>(parent, false); 
                    SystemAPI.SetSingleton<WaveSettings>(waveSettings);
                }
            }
            ecb.Playback(CheckedStateRef.EntityManager);
            ecb.Dispose();
        }
    }
}
