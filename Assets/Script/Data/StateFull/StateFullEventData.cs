using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Stateful.Data
{
    public enum StatefulEventState : byte
    {
        Undefined,
        Enter,
        Stay,
        Exit
    }

    // Extends ISimulationEvent with extra StatefulEventState.
    public interface IStatefulSimulationEvent<T> : IBufferElementData, ISimulationEvent<T>
    {
        public StatefulEventState State { get; set; }
    }
}
