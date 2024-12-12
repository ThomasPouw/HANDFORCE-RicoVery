using HANDFORCE.TCCavy.Balloon.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Controller.Data
{
    public struct RawControllerInput: IComponentData
    {
        public float2 oldCursorPosition;
        public float2 cursorPosition;
        public bool oldShoot;
        public bool shoot;
        public bool paused;
        public LaserDirection laserDirection;
    }
}

