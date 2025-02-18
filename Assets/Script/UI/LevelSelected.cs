using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelected : MonoBehaviour
{
    public BalloonWaveSpawner balloonWaveSpawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadLevel()
    {
        World transferDataWorld = null;
        foreach(World world in World.All)
        {
            if(world.Name == "TransferDataWorld")
            {
                transferDataWorld = world;
            }
        }
        if(transferDataWorld != null)
        {
            DynamicBuffer<BalloonWaveBuffer> BBuffer= new DynamicBuffer<BalloonWaveBuffer>();
            DynamicBuffer<BalloonMovementBuffer> BMBuffer= new DynamicBuffer<BalloonMovementBuffer>();
            EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
            if(!transferDataWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).TryGetSingletonBuffer<BalloonWaveBuffer>(out BBuffer) && !transferDataWorld.EntityManager.CreateEntityQuery(typeof(BalloonMovementBuffer)).TryGetSingletonBuffer<BalloonMovementBuffer>(out BMBuffer))
            {
                Entity _entity = transferDataWorld.EntityManager.CreateEntity();
                transferDataWorld.EntityManager.SetName(_entity, "balloonBuffer");
                BBuffer = ECB.AddBuffer<BalloonWaveBuffer>(_entity);
                //transferDataWorld.EntityManager.GetBufferTypeHandle<BalloonWaveBuffer>(false);
                BMBuffer = ECB.AddBuffer<BalloonMovementBuffer>(_entity);
            }
            for (int i = 0; i < balloonWaveSpawner.waveSpawns.Count; i++)
            {
                using(BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref BalloonBlobReference balloonWave = ref blobBuilder.ConstructRoot<BalloonBlobReference>();
                    BlobBuilderArray<BalloonSpawnEntity> balloonWaveBlobArray = blobBuilder.Allocate(ref balloonWave.balloonSpawns, balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count);
                    
                    for (int ii = 0; ii < balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count; ii++)
                    {
                            if(balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count != 0)
                            {
                                ref BalloonMovementBlobReference balloonStartMovement = ref blobBuilder.ConstructRoot<BalloonMovementBlobReference>();
                                BlobBuilderArray<BalloonLocationPath> balloonMovementBlobArray = blobBuilder.Allocate(ref balloonStartMovement.balloonPath, balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count);

                                for (int start = 0; start < balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count; start++)
                                {
                                    balloonMovementBlobArray[start] = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath[start];
                                }
                                BlobAssetReference<BalloonMovementBlobReference> movementBlobAssetReference = blobBuilder.CreateBlobAssetReference<BalloonMovementBlobReference>(Allocator.Persistent);
                                BMBuffer.Add(new BalloonMovementBuffer
                                {
                                    ID = 0,
                                    balloonPath = movementBlobAssetReference
                                });
                            }

                            if(balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count != 0)
                            {
                                ref BalloonMovementBlobReference balloonContinueMovement = ref blobBuilder.ConstructRoot<BalloonMovementBlobReference>();
                                BlobBuilderArray<BalloonLocationPath> balloonContinueBlobArray = blobBuilder.Allocate(ref balloonContinueMovement.balloonPath, balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count);
                                for (int con = 0; con < balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count; con++)
                                {
                                    balloonContinueBlobArray[con] = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath[con];
                                }
                                BlobAssetReference<BalloonMovementBlobReference> movementBlobAssetReference = blobBuilder.CreateBlobAssetReference<BalloonMovementBlobReference>(Allocator.Persistent);
                                BMBuffer.Add(new BalloonMovementBuffer
                                {
                                    ID = 0,
                                    balloonPath = movementBlobAssetReference
                                });
                            }
                        balloonWaveBlobArray[ii] = new BalloonSpawnEntity
                        {
                            ID = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].ID,
                            balloonType = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].balloonType,
                            location = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].location,
                            startPathIDNumber = 0,
                            movingPathIDNumber = 0,
                        };
                    }
                    BlobAssetReference<BalloonBlobReference> blobAssetReference = blobBuilder.CreateBlobAssetReference<BalloonBlobReference>(Allocator.Persistent);
                    BBuffer.Add(new BalloonWaveBuffer
                    {
                        ID = balloonWaveSpawner.waveSpawns[i].ID,
                        waveTime = balloonWaveSpawner.waveSpawns[i].time,
                        balloonSpawns = blobAssetReference
                    });
                }
            }
            SceneManager.LoadScene(balloonWaveSpawner.SceneNumber);
            ECB.Playback(transferDataWorld.EntityManager);
            ECB.Dispose();
        }
    }
}
