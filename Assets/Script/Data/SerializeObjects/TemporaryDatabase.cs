using System.Collections.Generic;
using HANDFORCE.TCCavy.Collection.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "TemporaryDatabase", menuName = "Scriptable Objects/TemporaryDatabase")]
public class TemporaryDatabase : ScriptableObject
{
    public List<ShootBuffer> shots;
    public List<MissedShootBuffer> missedShots;
    public List<CursorLocationBuffer> cursorLocations;
    public List<BalloonCollectionBuffer> balloonCollection;
    public TemporaryDatabase()
    {
        shots = new List<ShootBuffer>();
        missedShots = new List<MissedShootBuffer>();
        cursorLocations = new List<CursorLocationBuffer>();
        balloonCollection = new List<BalloonCollectionBuffer>();

    }
}
