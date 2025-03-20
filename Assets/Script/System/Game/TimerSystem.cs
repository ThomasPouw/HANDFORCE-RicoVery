using System;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;


namespace HANDFORCE.TCCavy.General.Data
{
    [RequireMatchingQueriesForUpdate]
    public partial class TimerSystem : SystemBase
    {
        [WriteOnly] public static Action<float> OnTimeTick;
        EntityQuery waveControllerQuery;
        EntityQuery pausedControllerQuery;
        Entity _entity;

        [BurstCompile]
        protected override void OnCreate()
        {
            _entity = CheckedStateRef.EntityManager.CreateEntity(typeof(Timer), typeof(TimeLeftWave), typeof(Paused), typeof(Result), typeof(Reset));

            SystemAPI.SetComponent(_entity, new Timer{});
            SystemAPI.SetComponent(_entity,new TimeLeftWave{ start = false});
             SystemAPI.SetComponentEnabled<Reset>(_entity, true);
            SystemAPI.SetComponentEnabled<Paused>(_entity, false);
            SystemAPI.SetComponentEnabled<Result>(_entity, false);
            pausedControllerQuery = new EntityQueryBuilder(Allocator.Temp).WithDisabled<Paused>().Build(ref CheckedStateRef);
            waveControllerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<WaveSettings>().Build(ref CheckedStateRef);
            CheckedStateRef.RequireForUpdate<Timer>();
            CheckedStateRef.RequireForUpdate<TimeLeftWave>();
            //CheckedStateRef.RequireForUpdate<BalloonWaveBuffer>();
        }
        [BurstCompile]
        protected override void OnUpdate()
        {
            if(SystemAPI.IsComponentEnabled<Reset>(_entity))
            {
                SystemAPI.SetComponentEnabled<Reset>(_entity, false);
                SystemAPI.SetComponentEnabled<Paused>(_entity, false);
                SystemAPI.SetComponentEnabled<Result>(_entity, false);
                Timer timer = SystemAPI.GetSingleton<Timer>();
                timer.gameTime = 0;
                timer.Time = 0;
                timer.startTime = 0;
                SystemAPI.SetSingleton<Timer>(timer);
            }
            if(!SystemAPI.IsComponentEnabled<Result>(_entity))
            {
                Timer timer = SystemAPI.GetSingleton<Timer>();
                if(!pausedControllerQuery.IsEmpty)
                {
                    TimeLeftWave timeLeftWave = SystemAPI.GetSingleton<TimeLeftWave>();
                    if(!timeLeftWave.start) return;
                    timer.gameTime += SystemAPI.Time.DeltaTime;
                    timeLeftWave.timeLeft -= SystemAPI.Time.DeltaTime;
                    SystemAPI.SetSingleton<TimeLeftWave>(timeLeftWave);
                    if(!waveControllerQuery.IsEmpty && timeLeftWave.timeLeft <= 0)
                    {
                        Entity entity = waveControllerQuery.GetSingletonEntity();
                        CheckedStateRef.EntityManager.DestroyEntity(new EntityQueryBuilder(Allocator.Temp).WithAll<BalloonData>().Build(ref CheckedStateRef));
                        SystemAPI.SetComponentEnabled<WaveRequested>(entity, true);
                    }
                    OnTimeTick?.Invoke(timer.gameTime); //Put here and not a tier below because it updates the gametime and not the default time.
                }
                timer.Time += SystemAPI.Time.DeltaTime;
                SystemAPI.SetSingleton<Timer>(timer);
            }
        }
    }
}
