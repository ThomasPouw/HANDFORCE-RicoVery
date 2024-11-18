using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Controller.Data
{
    public struct SettingsData: IComponentData
    {
        public float joystickSensitivityX;
        public float joystickSensitivityY;
        public bool inverseX;
        public bool inverseY;

        public float2 GetSentitivity2D()
        {
            float X = inverseX ? -joystickSensitivityX : joystickSensitivityX;
            float Y = inverseY ? -joystickSensitivityY : joystickSensitivityY;
            return new float2(X, Y);
        }
        public float3 GetSentitivity3D()
        {
            float X = inverseX ? -joystickSensitivityX : joystickSensitivityX;
            float Y = inverseY ? -joystickSensitivityY : joystickSensitivityY;
            return new float3(X, Y, 0);
        }

        
    }
}
