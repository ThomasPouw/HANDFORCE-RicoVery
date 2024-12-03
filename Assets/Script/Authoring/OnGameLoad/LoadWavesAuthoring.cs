using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class LoadWavesAuthoring : MonoBehaviour
{
    public BalloonWaveSpawner balloonWaveSpawner;
    public GameObject defaultBalloonPrefab;
    public GameObject upBalloonPrefab;
    public GameObject rightBalloonPrefab;
    public GameObject downBalloonPrefab;
    public GameObject leftBalloonPrefab;
    class LoadWavesBaker : Baker<LoadWavesAuthoring>
    {
        public override void Bake(LoadWavesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            DynamicBuffer<BalloonWaveBuffer> BBuffer = AddBuffer<BalloonWaveBuffer>(entity);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < authoring.balloonWaveSpawner.waveSpawns.Count; i++)
            {
                using(BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref BalloonBlobReference balloonWave = ref blobBuilder.ConstructRoot<BalloonBlobReference>();
                    BlobBuilderArray<BalloonSpawn> balloonWaveBlobArray = blobBuilder.Allocate(ref balloonWave.balloonSpawns, authoring.balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count);
                    
                    
                    for (int ii = 0; ii < authoring.balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count; ii++)
                    {
                        balloonWaveBlobArray[ii] = authoring.balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii];
                    }
                    var blobAssetReference = blobBuilder.CreateBlobAssetReference<BalloonBlobReference>(Allocator.Persistent);
                    BBuffer.Add(new BalloonWaveBuffer
                    {
                        ID = authoring.balloonWaveSpawner.waveSpawns[i].ID,
                        waveTime = authoring.balloonWaveSpawner.waveSpawns[i].time,
                        balloonSpawns = blobAssetReference
                    });
                }
            }
            AddComponent(entity, new WaveSettings
            {
                defaultBalloonEntityToSpawn = GetEntity(authoring.defaultBalloonPrefab, TransformUsageFlags.Dynamic),
                upBalloonEntityToSpawn = GetEntity(authoring.upBalloonPrefab, TransformUsageFlags.Dynamic),
                rightBalloonEntityToSpawn = GetEntity(authoring.rightBalloonPrefab, TransformUsageFlags.Dynamic),
                downBalloonEntityToSpawn = GetEntity(authoring.downBalloonPrefab, TransformUsageFlags.Dynamic),
                leftBalloonEntityToSpawn = GetEntity(authoring.leftBalloonPrefab, TransformUsageFlags.Dynamic),
                amountOfWaves = (short)authoring.balloonWaveSpawner.waveSpawns.Count,
                currentWaves = 0
            });
            AddComponent<WaveRequested>(entity);
            SetComponentEnabled<WaveRequested>(entity, true);
            //Make this a singleton if you see how.
        }
    }
}
