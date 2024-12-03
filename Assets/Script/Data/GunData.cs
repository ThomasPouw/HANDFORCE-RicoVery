using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Data
{
    public struct GunBase: IComponentData {}
    public struct newLaser: IEnableableComponent, IComponentData{}
    public struct LaserSelector: IComponentData
    {
        public bool reversed;
        public float timeNeeded;
    }
    public struct LaserOrigin: IComponentData
    {
        public float size;
    }
}