using Unity.Entities;
using UnityEngine;
using Unity.Burst;

using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.General.Data;
using Unity.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;

namespace HANDFORCE.TCCavy.Writer.LoopSystem
{
    [UpdateAfter(typeof(TimerSystem))]
    public partial struct JSONWriterSystem : ISystem
    {
        public static string directory = Application.persistentDataPath+"/TempDataBase/";
        public static TemporaryDatabase tempDatabase;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Entity entity = state.EntityManager.CreateEntity(typeof(MissedShootBuffer), typeof(ShootBuffer), typeof(CursorLocationBuffer), typeof(BalloonCollectionBuffer), typeof(Saving));
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            SystemAPI.SetComponentEnabled<Saving>(entity, false);
            ecb.SetBuffer<MissedShootBuffer>(entity);
            ecb.SetBuffer<ShootBuffer>(entity);
            ecb.SetBuffer<CursorLocationBuffer>(entity);
            ecb.SetBuffer<BalloonCollectionBuffer>(entity);
            tempDatabase = new TemporaryDatabase()
            {
                missedShots = new List<MissedShootBuffer>(),
                shots = new List<ShootBuffer>(),
                cursorLocations = new List<CursorLocationBuffer>(),
                balloonCollection = new List<BalloonCollectionBuffer>(),
            };

            state.RequireForUpdate<MissedShootBuffer>();
            state.RequireForUpdate<ShootBuffer>();
            state.RequireForUpdate<CursorLocationBuffer>();
            state.RequireForUpdate<BalloonCollectionBuffer>();
        }
        //Create a folder for SerializeObjects.
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity entity = SystemAPI.GetSingletonEntity<MissedShootBuffer>();
            if(state.EntityManager.IsComponentEnabled<Saving>(entity))
            {
                state.EntityManager.SetComponentEnabled<Saving>(entity, false);
                World transferWorld = World.DefaultGameObjectInjectionWorld;
                foreach(World world in World.All)
                {
                    Debug.Log(world.Name);
                    if(world.Name == "TransferDataWorld")
                    {
                        transferWorld = world;
                    }
                }
                EntityQuery balloonSpawnQuery = transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer), typeof(BalloonMovementBuffer));

                {
                    balloonSpawnQuery.TryGetSingletonBuffer<BalloonWaveBuffer>(out DynamicBuffer<BalloonWaveBuffer> DynaBalloonWave);
                    balloonSpawnQuery.TryGetSingletonBuffer<BalloonMovementBuffer>(out DynamicBuffer<BalloonMovementBuffer> DynaBalloonMove);
                    DynaBalloonWave.Clear();
                    DynaBalloonMove.Clear();
                }
                Save(directory, $"Test {DateTime.Now.Day+" "+DateTime.Now.Month+ " "+DateTime.Now.Year +" "+DateTime.Now.Hour +";"+DateTime.Now.Minute}.json", tempDatabase);
                
                ecb.Playback(transferWorld.EntityManager);
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
                return;
            }
            DynamicBuffer<MissedShootBuffer> missedShootBuffers = SystemAPI.GetSingletonBuffer<MissedShootBuffer>();
            DynamicBuffer<ShootBuffer> shootBuffers = SystemAPI.GetSingletonBuffer<ShootBuffer>();
            DynamicBuffer<CursorLocationBuffer> cursorLocationBuffer = SystemAPI.GetSingletonBuffer<CursorLocationBuffer>();
            DynamicBuffer<BalloonCollectionBuffer> balloonCollectionBuffer = SystemAPI.GetSingletonBuffer<BalloonCollectionBuffer>();
            if(!missedShootBuffers.IsEmpty)
            {
                foreach(MissedShootBuffer missedShot in missedShootBuffers)
                {
                    tempDatabase.missedShots.Add(missedShot);
                }
                missedShootBuffers.Clear();
            }
            if(!shootBuffers.IsEmpty)
            {
                foreach(ShootBuffer shoot in shootBuffers)
                {
                    tempDatabase.shots.Add(shoot);
                }
                shootBuffers.Clear();
            }
            if(!cursorLocationBuffer.IsEmpty)
            {
                foreach(CursorLocationBuffer cursorLocation in cursorLocationBuffer)
                {
                    tempDatabase.cursorLocations.Add(cursorLocation);
                }
                cursorLocationBuffer.Clear();
            }
            if(!balloonCollectionBuffer.IsEmpty)
            {
                foreach(BalloonCollectionBuffer balloonCollection in balloonCollectionBuffer)
                {
                    tempDatabase.balloonCollection.Add(balloonCollection);
                }
                balloonCollectionBuffer.Clear();
            }
        }
        public void Save(string dir, string fileName, TemporaryDatabase temporaryDatabase)
        {
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string json = JsonUtility.ToJson(temporaryDatabase, true);
            File.WriteAllText(dir+ fileName, json);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            Debug.Log("Is destroyed");
            //Save(directory, $"Test {DateTime.Now.Day+" "+DateTime.Now.Month+ " "+DateTime.Now.Year +" "+DateTime.Now.Hour +";"+DateTime.Now.Minute}.json", tempDatabase);
            Debug.Log("Saved");
        }
    }
}
