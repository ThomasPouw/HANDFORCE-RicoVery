using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class LoadWavesAuthoring : MonoBehaviour
{
    //public BalloonWaveSpawner balloonWaveSpawner;
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
            //DynamicBuffer<BalloonWaveBuffer> BBuffer = AddBuffer<BalloonWaveBuffer>(entity);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            AddComponent(entity, new WaveSettings
            {
                defaultBalloonEntityToSpawn = GetEntity(authoring.defaultBalloonPrefab, TransformUsageFlags.Dynamic),
                upBalloonEntityToSpawn = GetEntity(authoring.upBalloonPrefab, TransformUsageFlags.Dynamic),
                rightBalloonEntityToSpawn = GetEntity(authoring.rightBalloonPrefab, TransformUsageFlags.Dynamic),
                downBalloonEntityToSpawn = GetEntity(authoring.downBalloonPrefab, TransformUsageFlags.Dynamic),
                leftBalloonEntityToSpawn = GetEntity(authoring.leftBalloonPrefab, TransformUsageFlags.Dynamic),
                currentWaves = 0
            });
            AddComponent<WaveRequested>(entity);
            SetComponentEnabled<WaveRequested>(entity, true);
            //Make this a singleton if you see how.
        }
    }
}
