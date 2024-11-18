using HANDFORCE.TCCavy.Stateful.Authoring;
using HANDFORCE.TCCavy.Stateful.Buffer;
using HANDFORCE.TCCavy.Stateful.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Stateful.Job
{
    public static class StatefulEventCollectionJobs
    {
        [BurstCompile]
        public struct CollectTriggerEvents : ITriggerEventsJob
        {
            public NativeList<StatefulTriggerEvent> TriggerEvents;
            public void Execute(TriggerEvent triggerEvent) => TriggerEvents.Add(new StatefulTriggerEvent(triggerEvent));
        }


        [BurstCompile]
        public struct ConvertEventStreamToDynamicBufferJob<T, C> : IJob
            where T : unmanaged, IBufferElementData, IStatefulSimulationEvent<T>
            where C : unmanaged, IComponentData
        {
            public NativeList<T> PreviousEvents;
            public NativeList<T> CurrentEvents;
            public BufferLookup<T> EventLookup;

            public bool UseExcludeComponent;
            [ReadOnly] public ComponentLookup<C> EventExcludeLookup;

            public void Execute()
            {
                var statefulEvents = new NativeList<T>(CurrentEvents.Length, Allocator.Temp);

                StatefulSimulationEventBuffers<T>.GetStatefulEvents(PreviousEvents, CurrentEvents, statefulEvents);

                for (int i = 0; i < statefulEvents.Length; i++)
                {
                    var statefulEvent = statefulEvents[i];

                    var addToEntityA = EventLookup.HasBuffer(statefulEvent.EntityA) &&
                        (!UseExcludeComponent || !EventExcludeLookup.HasComponent(statefulEvent.EntityA));
                    var addToEntityB = EventLookup.HasBuffer(statefulEvent.EntityB) &&
                        (!UseExcludeComponent || !EventExcludeLookup.HasComponent(statefulEvent.EntityA));

                    if (addToEntityA)
                    {
                        EventLookup[statefulEvent.EntityA].Add(statefulEvent);
                    }

                    if (addToEntityB)
                    {
                        EventLookup[statefulEvent.EntityB].Add(statefulEvent);
                    }
                }
            }
        }
    }
}
