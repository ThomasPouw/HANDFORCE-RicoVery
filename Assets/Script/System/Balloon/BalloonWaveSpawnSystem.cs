using System;
using System.Linq;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using HANDFORCE.TCCavy.General.Data;
using Unity.Burst;
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
        private int count = 0;
        [BurstCompile]
        protected override void OnCreate()
        {
            balloonQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<BalloonData>().Build(ref CheckedStateRef);
            //CheckedStateRef.RequireForUpdate<BalloonWaveBuffer>();
            CheckedStateRef.RequireForUpdate<WaveSettings>();
        }
        [BurstCompile]
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
            if(balloonQuery.IsEmpty || CheckedStateRef.EntityManager.IsComponentEnabled<WaveRequested>(parent))
            {
                count++;
                
                transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).TryGetSingletonBuffer<BalloonWaveBuffer>(out DynamicBuffer<BalloonWaveBuffer> DynaBalloonWave);
                SystemAPI.TryGetSingleton<WaveSettings>(out WaveSettings waveSettings);
                
                Debug.Log(DynaBalloonWave.Length +" "+ waveSettings.currentWaves);
                if(DynaBalloonWave.Length <= waveSettings.currentWaves)
                {
                    Debug.Log("Entered Result here");
                    SystemAPI.SetComponentEnabled<WaveRequested>(parent, false); 
                    GameClear?.Invoke();
                    return;
                }
                BalloonWaveBuffer balloonWaveBuffer = DynaBalloonWave[waveSettings.currentWaves];
                waveSettings.currentWaves++; 
                SystemAPI.SetSingleton<WaveSettings>(waveSettings);
                LocalTransform localTransform = SystemAPI.GetComponent<LocalTransform>(parent);
                Timer timer = SystemAPI.GetSingleton<Timer>();
                Debug.Log(balloonWaveBuffer.balloonSpawns.Value.balloonSpawns.Length);
                for (int i = 0; i < balloonWaveBuffer.balloonSpawns.Value.balloonSpawns.Length; i++)
                {
                    //Debug.Log(balloonWaveBuffer.balloonSpawns.Value.balloonSpawns[i]);
                    BalloonSpawnEntity balloonSpawn = balloonWaveBuffer.balloonSpawns.Value.balloonSpawns[i];
                    Entity entity = Entity.Null;
                    //Entity entity = ecb.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                    switch (balloonSpawn.balloonType)
                    {
                        case LaserDirection.Default:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Up:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.upBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Right:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.rightBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Down:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.downBalloonEntityToSpawn);
                            break;
                        case LaserDirection.Left:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.leftBalloonEntityToSpawn);
                            break;
                        default:
                            entity = CheckedStateRef.EntityManager.Instantiate(waveSettings.defaultBalloonEntityToSpawn);
                            break;
                    }
                    float3 pos = balloonSpawn.location;
                    CheckedStateRef.EntityManager.SetName(entity, count.ToString()+" "+balloonSpawn.ID);
                    CheckedStateRef.EntityManager.SetComponentData(entity, new LocalTransform
                    {
                        Position = new float3(pos.x, pos.y, localTransform.Position.z),
                        Scale = 100
                    });
                    CheckedStateRef.EntityManager.AddComponentData(entity, new BalloonData
                    {
                        ID = entity.Index = balloonSpawn.ID,
                        type = balloonSpawn.balloonType,
                        spawnTime = timer.Time
                    });
                    CheckedStateRef.EntityManager.AddComponent<BalloonTimer>(entity);
                    {
                        transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonMovementBuffer)).TryGetSingletonBuffer<BalloonMovementBuffer>(out DynamicBuffer<BalloonMovementBuffer> DynaBalloonMove);
                        DynamicBuffer<BalloonLocationPath> balloonLocationPaths= CheckedStateRef.EntityManager.AddBuffer<BalloonLocationPath>(entity);
                        for (int ii = 0; ii < DynaBalloonMove.Length; ii++)
                        {
                            if(DynaBalloonMove[ii].ID == balloonWaveBuffer.balloonSpawns.Value.balloonSpawns[i].startPathIDNumber)
                            {
                                Debug.Log($"Number {i} with ID {DynaBalloonMove[ii].ID} found the startID! It has length: {DynaBalloonMove[ii].balloonPath.Value.balloonPath.ToArray().Length}");
                                foreach (BalloonLocationPath path in DynaBalloonMove[ii].balloonPath.Value.balloonPath.ToArray())
                                {
                                    balloonLocationPaths.Add(path);
                                }
                                //balloonLocationPaths.CopyFrom(DynaBalloonMove[ii].balloonPath.Value.balloonPath.ToArray());
                                break;
                            }
                        }
                        //balloonLocationPaths.AddRange(DynaBalloonMove);
                    }
                }

                {
                    TimeLeftWave T = SystemAPI.GetSingleton<TimeLeftWave>();
                    T.timeLeft = balloonWaveBuffer.waveTime;
                    SystemAPI.SetSingleton<TimeLeftWave>(T); //Leaving in the <T> to make it easier to read. It does not actually need it.
                    CheckedStateRef.EntityManager.SetComponentEnabled<WaveRequested>(parent, false);
                }
            }
            ecb.Playback(CheckedStateRef.EntityManager);
            ecb.Dispose();
        }
            
    }
}
