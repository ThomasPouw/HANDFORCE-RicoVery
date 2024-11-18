using System.Runtime.InteropServices;
using HANDFORCE.TCCavy.Balloon.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace HANDFORCE.TCCavy.Collection.Data
{
    [System.Serializable]
    public struct BalloonCollectionBuffer: IBufferElementData
    {
        public float timeStamp;
        public float balloonEntityID;
        public LaserDirection balloonColour;
        public bool isMovingBalloon;
        public float spawningTimestamp;
    }
    [System.Serializable]
    public struct CursorLocationBuffer: IBufferElementData
    {
        public float timeStamp;
        public float3 rawMovementAdded;
        public float3 movementAdded;
        public bool hasShot;
    }
    [System.Serializable]
    public struct ShootBuffer: IBufferElementData
    {
        public float timeStamp;
        public float3 locationOfShot;
        public LaserDirection balloonColour;
        public float balloonEntityID;
    }
    [System.Serializable]
    public struct MissedShootBuffer: IBufferElementData
    {
        public float timeStamp;
        public float3 locationOfShot;
        public LaserDirection laserDirection;
        public ShootBuffer closestByBalloon;
        public ShootBuffer closestByColourBalloon;
        public float closestByBalloonDistance;
        public float closestByColourBalloonDistance;
    }
    [System.Serializable]
    public struct TempDatabase
    {
        public NativeList<CursorLocationBuffer> cursorLocation;
        public NativeList<ShootBuffer> shot;
        public NativeList<MissedShootBuffer> missedShot;
        public NativeList<BalloonCollectionBuffer> balloonCollection;
    }
}