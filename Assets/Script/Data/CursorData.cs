using HANDFORCE.TCCavy.Balloon.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Data
{
    public struct BackTriggerField: IComponentData {}
    public struct AimRadicle: IComponentData
    {
        public Color NoTargetColour;
        public Color TargetColour;
    }
    [System.Serializable]
    public struct RadicleOnBalloon: IComponentData
    {
        public LaserDirection balloonType;
    }
    public struct GunBase: IComponentData {}

}