using System.Collections.Generic;
using HANDFORCE.TCCavy.Collection.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct TemporaryDatabase
{
    public List<ShootBuffer> shots;
    public List<MissedShootBuffer> missedShots;
    public List<CursorLocationBuffer> cursorLocations;
    public List<BalloonCollectionBuffer> balloonCollection;
    /*public TemporaryDatabase()
    {
        shots = new NativeList<ShootBuffer>();
        missedShots = new NativeList<MissedShootBuffer>();
        cursorLocations = new NativeList<CursorLocationBuffer>();
        balloonCollection = new List<BalloonCollectionBuffer>();

    }*/
}
