using System.Linq;
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
            else
            {
                Entity _entity = transferDataWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer), typeof(BalloonMovementBuffer)).GetSingletonEntity();
                BBuffer = ECB.SetBuffer<BalloonWaveBuffer>(_entity);
                //transferDataWorld.EntityManager.GetBufferTypeHandle<BalloonWaveBuffer>(false);
                BMBuffer = ECB.SetBuffer<BalloonMovementBuffer>(_entity);
            }
            for (int i = 0; i < balloonWaveSpawner.waveSpawns.Count; i++)
            {
                using(BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref BalloonBlobReference balloonWave = ref blobBuilder.ConstructRoot<BalloonBlobReference>();
                    BlobBuilderArray<BalloonSpawnEntity> balloonWaveBlobArray = blobBuilder.Allocate(ref balloonWave.balloonSpawns, balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count);
                    
                    for (int ii = 0; ii < balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count; ii++)
                    {
                        bool moving = false;
                        if(balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count > 1)
                        {
                            using(BlobBuilder movementBlobBuilder = new BlobBuilder(Allocator.Temp))
                            {
                                ref BalloonMovementBlobReference balloonStartMovement = ref movementBlobBuilder.ConstructRoot<BalloonMovementBlobReference>();
                                Debug.Log("Should have length: "+ balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count);
                                BlobBuilderArray<BalloonLocationPath> balloonMovementBlobArray = movementBlobBuilder.Allocate(ref balloonStartMovement.balloonPath, balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count);

                                for (int iii = 0; iii < balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath.Count; iii++)
                                {
                                    balloonMovementBlobArray[iii] = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].startPath[iii];
                                    Debug.Log(ii+" "+balloonMovementBlobArray[iii].endLocation);
                                }
    
                                BlobAssetReference<BalloonMovementBlobReference> movementBlobAssetReference = movementBlobBuilder.CreateBlobAssetReference<BalloonMovementBlobReference>(Allocator.Temp);
                                
                                BMBuffer.Add(new BalloonMovementBuffer
                                {
                                    ID = BMBuffer.Length,
                                    balloonPath = movementBlobAssetReference
                                });
                            }
                            //Debug.Log("BMBuffer.Length: "+BMBuffer.Length);
                        }
                        else
                        {
                            Debug.LogError($"You forgot to enter in the starting movement of Balloon ID: {balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].ID}");
                            break;
                        }

                        if(balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count != 0)
                        {
                            using(BlobBuilder movementBlobBuilder = new BlobBuilder(Allocator.Temp))
                            {
                                ref BalloonMovementBlobReference balloonContinueMovement = ref movementBlobBuilder.ConstructRoot<BalloonMovementBlobReference>();
                                BlobBuilderArray<BalloonLocationPath> balloonContinueBlobArray = movementBlobBuilder.Allocate(ref balloonContinueMovement.balloonPath, balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count);
                                for (int iii = 0; iii < balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath.Count; iii++)
                                {
                                    balloonContinueBlobArray[iii] = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].movingPath[iii];
                                }
                                BlobAssetReference<BalloonMovementBlobReference> movementBlobAssetReference = movementBlobBuilder.CreateBlobAssetReference<BalloonMovementBlobReference>(Allocator.Persistent);
                                BMBuffer.Add(new BalloonMovementBuffer
                                {
                                    ID = BMBuffer.Length,
                                    balloonPath = movementBlobAssetReference
                                });
                                moving = true;
                            }
                        }
                        balloonWaveBlobArray[ii] = new BalloonSpawnEntity
                        {
                            ID = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].ID,
                            balloonType = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].balloonType,
                            location = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii].location,
                            startPathIDNumber = (short)(BMBuffer.Length + (moving ? -2 : -1)),
                            movingPathIDNumber = (short)(moving ? BMBuffer.Length -1 : -1),
                        };
                    }
                    foreach(var balloonBuffer in BMBuffer)
                    {
                        Debug.Log("Length: "+balloonBuffer.balloonPath.Value.balloonPath.Length);
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
