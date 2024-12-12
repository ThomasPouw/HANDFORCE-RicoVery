using System.Collections.Generic;
using HANDFORCE.TCCavy.Aim.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Balloon.Data
{
    public struct MovingBalloon: IComponentData
    {
        //Add Animation Curves
    }

    public enum LaserDirection: uint
    {
        //Do not have names due to needing to remember that the balloons can have different colours.
        //Maybe the = 0 Can be the type of laser that be used...
        Default = 1 << 5,
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,
    }
    [System.Serializable]
    public struct BalloonSpawn: IComponentData
    {
        public int ID;
        public LaserDirection balloonType;
        public float3 location;
        //public List<float3> checkPoint;
        public float travelTime;
        public bool isMoving;
    }
    public struct BalloonData: IComponentData
    {
        public int ID;
        public LaserDirection type;
        public float spawnTime;

    }
    [System.Serializable]
    public struct WaveSpawn
    {
        public float ID;
        public float time;
        public List<BalloonSpawn> balloonSpawns;
    }
    public struct WaveSettings: IComponentData
    {
        public Entity defaultBalloonEntityToSpawn;
        public Entity upBalloonEntityToSpawn;
        public Entity rightBalloonEntityToSpawn;
        public Entity downBalloonEntityToSpawn;
        public Entity leftBalloonEntityToSpawn;
        public short currentWaves; 

    }
    public struct WaveRequested: IComponentData, IEnableableComponent{}
    
}
