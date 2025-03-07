using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using HANDFORCE.TCCavy.Balloon.Data;
using Unity.Transforms;
using Unity.Mathematics;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using Unity.Collections;


namespace HANDFORCE.TCCavy.Aim
{
    partial struct BalloonMovementSystem : ISystem
    {
        //private World transferWorld;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (balloonTimer, dynaBalloonMove, localTransform, entity) in SystemAPI.Query<RefRW<BalloonTimer>, DynamicBuffer<BalloonLocationPath>, RefRW<LocalTransform>>().WithEntityAccess())
            {
                balloonTimer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
                Debug.Log("Here!");
                if(balloonTimer.ValueRO.currentTime >= balloonTimer.ValueRO.timeBetweenLocations)
                {
                    localTransform.ValueRW.Position = balloonTimer.ValueRW.endLocation;
                    balloonTimer.ValueRW.startLocation = balloonTimer.ValueRW.endLocation;
                    balloonTimer.ValueRW.index++;
                    balloonTimer.ValueRW.currentTime = 0;
                    if(dynaBalloonMove.Length <= balloonTimer.ValueRO.index)
                    {
                        if(balloonTimer.ValueRO.nextBuffer != -1)
                        {
                            if(!balloonTimer.ValueRO.hasSwitchedToMoveBuffer)
                            {
                                World transferWorld = World.DefaultGameObjectInjectionWorld;
                                foreach(World world in World.All)
                                {
                                    if(world.Name == "TransferDataWorld")
                                    {
                                        transferWorld = world;
                                    }
                                }
                                transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonMovementBuffer)).TryGetSingletonBuffer<BalloonMovementBuffer>(out DynamicBuffer<BalloonMovementBuffer> DynaBalloonMove);
                                dynaBalloonMove.Clear();
                                foreach (BalloonLocationPath path in DynaBalloonMove[balloonTimer.ValueRO.nextBuffer].balloonPath.Value.balloonPath.ToArray())
                                {
                                    dynaBalloonMove.Add(path);
                                }
                                balloonTimer.ValueRW.hasSwitchedToMoveBuffer = true;
                                //ecb.SetBuffer(entity, dynaBalloonMove)
                                
                            }
                            Debug.Log(dynaBalloonMove.Length +" "+ balloonTimer.ValueRO.index);
                            balloonTimer.ValueRW.index = 0;
                        }
                        else
                        {
                            ecb.RemoveComponent<BalloonTimer>(entity);
                            dynaBalloonMove.Clear();
                            balloonTimer.ValueRW.index = 0;
                            break;
                            //ecb.Remove
                        }
                    }
                    float2 f2 = dynaBalloonMove[balloonTimer.ValueRW.index].endLocation;
                    balloonTimer.ValueRW.endLocation = new float3(f2.x,f2.y, balloonTimer.ValueRO.endLocation.z);
                }
                float alpha = balloonTimer.ValueRO.currentTime/balloonTimer.ValueRO.timeBetweenLocations; //Needs to be doen as Lerp works from 0 to 1. As it is also used in transparency, that is also why it is called alpha usually.
                localTransform.ValueRW.Position = math.lerp(balloonTimer.ValueRO.startLocation, balloonTimer.ValueRO.endLocation, alpha);
                
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
