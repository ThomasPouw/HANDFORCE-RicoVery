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
            DynamicBuffer<BalloonWaveBuffer> BBuffer= default;
            if(!transferDataWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).TryGetSingletonBuffer<BalloonWaveBuffer>(out BBuffer))
            {
                Entity _entity = transferDataWorld.EntityManager.CreateEntity();
                transferDataWorld.EntityManager.SetName(_entity, "balloonBuffer");
                BBuffer = transferDataWorld.EntityManager.AddBuffer<BalloonWaveBuffer>(_entity);
            }

            for (int i = 0; i < balloonWaveSpawner.waveSpawns.Count; i++)
            {
                using(BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref BalloonBlobReference balloonWave = ref blobBuilder.ConstructRoot<BalloonBlobReference>();
                    BlobBuilderArray<BalloonSpawn> balloonWaveBlobArray = blobBuilder.Allocate(ref balloonWave.balloonSpawns, balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count);
                    
                    
                    for (int ii = 0; ii < balloonWaveSpawner.waveSpawns[i].balloonSpawns.Count; ii++)
                    {
                        balloonWaveBlobArray[ii] = balloonWaveSpawner.waveSpawns[i].balloonSpawns[ii];
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
        }
    }
}
