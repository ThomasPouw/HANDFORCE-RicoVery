using System.Collections.Generic;
using HANDFORCE.TCCavy.Aim.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Balloon.Data
{


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
    public struct BalloonSpawnObject
    {
        public int ID;
        public LaserDirection balloonType;
        public float3 location;
        public List<BalloonLocationPath> startPath;
        public List<BalloonLocationPath> movingPath;
    }
    [System.Serializable]
    public struct BalloonSpawnEntity: IComponentData
    {
        public int ID;
        public LaserDirection balloonType;
        public float3 location;
        //public List<float3> checkPoint;
        public short startPathIDNumber;
        public short movingPathIDNumber;
    }
    public struct BalloonData: IComponentData
    {
        public int ID;
        public LaserDirection type;
        public float spawnTime;

    }
    [System.Serializable]
    public struct WaveSpawnObject
    {
        public float ID;
        public float time;
        public List<BalloonSpawnObject> balloonSpawns;
    }
    [System.Serializable]
    public struct WaveSpawnEntity
    {
        public float ID;
        public float time;
        public List<BalloonSpawnEntity> balloonSpawns;
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
    [System.Serializable]
    public struct BalloonLocationPath:IBufferElementData
    {
        public float3 endLocation;
        public float timeBetweenLocations; 
    }
    public struct BalloonTimer: IComponentData
    {
        public float3 endLocation;
        public float3 startLocation;
        public float currentTime;
        public float timeBetweenLocations; //End timer
    }
    
}
