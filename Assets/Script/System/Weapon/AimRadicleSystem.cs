using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.Controller.Data;
using HANDFORCE.TCCavy.General.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.InputSystem;
namespace HANDFORCE.TCCavy.Aim.LoopSystem
{
    partial struct AimRadicleSystem: ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            state.RequireForUpdate<RawControllerInput>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RawControllerInput rawInput = SystemAPI.GetSingleton<RawControllerInput>();
            SettingsData settingsData = SystemAPI.GetSingleton<SettingsData>();
            Entity backEntity = SystemAPI.GetSingletonEntity<BackTriggerField>();
            WorldRenderBounds backGroundFieldBounds = SystemAPI.GetComponent<WorldRenderBounds>(backEntity);
            Entity timerEntity = SystemAPI.GetSingletonEntity<TimeLeftWave>();
            if(!SystemAPI.IsComponentEnabled<Paused>(timerEntity))
            {
                foreach (var (aimRadicle, localTransform) in SystemAPI.Query<AimRadicle, RefRW<LocalTransform>>())
                {
                    float3 pos = localTransform.ValueRW.Position + (new float3(rawInput.cursorPosition.x, rawInput.cursorPosition.y, 0)* settingsData.GetSentitivity3D());
                    pos = math.clamp(pos, backGroundFieldBounds.Value.Min, backGroundFieldBounds.Value.Max);
                    float3 changedPos = pos - localTransform.ValueRW.Position;
                    localTransform.ValueRW.Position = pos;

                    /*DynamicBuffer<CursorLocationBuffer> CLBuffer = SystemAPI.GetSingletonBuffer<CursorLocationBuffer>();
                    Timer time = SystemAPI.GetSingleton<Timer>();
                    CLBuffer.Add(new CursorLocationBuffer
                    {
                        timeStamp = time.Time,
                        rawMovementAdded = new float3(rawInput.cursorPosition.x, rawInput.cursorPosition.y, 0),
                        movementAdded = changedPos,
                        hasShot = rawInput.shoot
                    });*/
               
                    //Mouse.current.WarpCursorPosition(new float2(localTransform.ValueRO.Position.x, localTransform.ValueRO.Position.z));
                }
            }
        }
    }
}