using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.General.Data
{

    public struct Timer: IComponentData
    {
        public float Time;
        public float gameTime;
        public float startTime;
    }
    public struct TimeLeftWave: IComponentData
    {
        public bool start;
        public float timeLeft;
    }
    public struct Reset: IComponentData, IEnableableComponent{}
    public struct Paused: IComponentData, IEnableableComponent{}
    public struct Result: IComponentData, IEnableableComponent{}
}

