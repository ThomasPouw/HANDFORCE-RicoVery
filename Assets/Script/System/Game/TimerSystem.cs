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
            _entity = CheckedStateRef.EntityManager.CreateEntity(typeof(Timer), typeof(TimeLeftWave), typeof(Paused), typeof(Result));

            SystemAPI.SetComponent(_entity, new Timer{});
            SystemAPI.SetComponent(_entity,new TimeLeftWave{});
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
            if(!SystemAPI.IsComponentEnabled<Result>(_entity))
            {
                Timer timer = SystemAPI.GetSingleton<Timer>();
                timer.Time += SystemAPI.Time.DeltaTime;
                if(!pausedControllerQuery.IsEmpty)
                {
                    TimeLeftWave timeLeftWave = SystemAPI.GetSingleton<TimeLeftWave>();
                    timer.gameTime += SystemAPI.Time.DeltaTime;
                    timeLeftWave.timeLeft -= SystemAPI.Time.DeltaTime;
                    SystemAPI.SetSingleton<TimeLeftWave>(timeLeftWave);
                    if(!waveControllerQuery.IsEmpty && timeLeftWave.timeLeft <= 0)
                    {
                        Entity entity = waveControllerQuery.GetSingletonEntity();
                        SystemAPI.SetComponentEnabled<WaveRequested>(entity, true);
                    }
                    OnTimeTick?.Invoke(timer.gameTime); //Put here and not a tier below because it updates the gametime and not the default time.
                }
                SystemAPI.SetSingleton<Timer>(timer);
            }
        }
    }
}
