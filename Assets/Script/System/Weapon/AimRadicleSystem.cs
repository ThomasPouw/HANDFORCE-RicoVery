
using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.Controller.Data;
using HANDFORCE.TCCavy.General.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
namespace HANDFORCE.TCCavy.Aim
{
    [RequireMatchingQueriesForUpdate]
    public partial class AimRadicleSystem : SystemBase
    //Change thi to SystemBase
    {
        private World transferWorld;
        [BurstCompile]
        protected override void OnCreate()
        {

            CheckedStateRef.RequireForUpdate<RawControllerInput>();
            CheckedStateRef.RequireForUpdate<BackTriggerField>();
        }
        [BurstCompile]
        protected override void OnUpdate()
        {
            if(transferWorld == null)
            {
                foreach(World world in World.All)
                {
                    if(world.Name == "TransferDataWorld")
                    {
                        transferWorld = world;
                    }
                }
            }
            SettingsData settingsData = transferWorld.EntityManager.CreateEntityQuery(typeof(SettingsData)).GetSingleton<SettingsData>();

            RawControllerInput rawInput = SystemAPI.GetSingleton<RawControllerInput>();
            Entity backEntity = SystemAPI.GetSingletonEntity<BackTriggerField>();
            WorldRenderBounds backGroundFieldBounds = SystemAPI.GetComponent<WorldRenderBounds>(backEntity);
            Entity timerEntity = SystemAPI.GetSingletonEntity<TimeLeftWave>();
            if(!SystemAPI.IsComponentEnabled<Paused>(timerEntity))
            {
                foreach (var (aimRadicle, localTransform) in SystemAPI.Query<AimRadicle, RefRW<LocalTransform>>())
                {
                    float3 pos = localTransform.ValueRW.Position + (new float3(rawInput.cursorPosition.x, rawInput.cursorPosition.y, 0)* settingsData.GetSentitivity3D());
                    pos = math.clamp(pos, backGroundFieldBounds.Value.Min, backGroundFieldBounds.Value.Max);
                    pos = (int3)(pos * new float3(1000.0f)) / new float3(1000.0f);
                    localTransform.ValueRW.Position = pos;

                    if((rawInput.oldShoot != rawInput.shoot || !rawInput.oldCursorPosition.Equals(pos.xy)))
                    {
                        DynamicBuffer<CursorLocationBuffer> CLBuffer = SystemAPI.GetSingletonBuffer<CursorLocationBuffer>();
                        Timer time = SystemAPI.GetSingleton<Timer>();
                        CLBuffer.Add(new CursorLocationBuffer
                        {
                            timeStamp = time.Time,
                            angularMotionAdded = pos.xy,
                            hasFired = rawInput.shoot
                        });
                    };
                    rawInput.oldShoot = rawInput.shoot;
                    rawInput.oldCursorPosition = pos.xy;
                    SystemAPI.SetSingleton<RawControllerInput>(rawInput);

               
                    //Mouse.current.WarpCursorPosition(new float2(localTransform.ValueRO.Position.x, localTransform.ValueRO.Position.z));
                }
            }
        }
    }
}