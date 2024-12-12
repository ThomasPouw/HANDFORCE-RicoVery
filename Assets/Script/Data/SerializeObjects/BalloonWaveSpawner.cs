using System.Collections.Generic;
using HANDFORCE.TCCavy.Balloon.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "BalloonWaveSpawner", menuName = "Scriptable Objects/BalloonWaveSpawner")]
public class BalloonWaveSpawner : ScriptableObject
{
    public List<WaveSpawn> waveSpawns;
    public short SceneNumber = 1;
}
