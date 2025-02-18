using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Balloon.Data.Buffer
{
    public struct BalloonWaveBuffer : IBufferElementData
    {
        public float ID;
        public float waveTime;
        public BlobAssetReference<BalloonBlobReference> balloonSpawns;
    }
    public struct BalloonMovementBuffer: IBufferElementData
    {
        public float ID;
        public BlobAssetReference<BalloonMovementBlobReference> balloonPath;
    }
    public struct BalloonBlobReference : IComponentData
    {
        public BlobArray<BalloonSpawnEntity> balloonSpawns;
    }
    public struct BalloonMovementBlobReference : IComponentData
    {
        public BlobArray<BalloonLocationPath> balloonPath;
    }
}
