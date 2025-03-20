using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Balloon.Data.Buffer;
using HANDFORCE.TCCavy.Collection.Data;
using HANDFORCE.TCCavy.General.Data;
using HANDFORCE.TCCavy.Writer.LoopSystem;
using TMPro;
using Unity.Entities;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text defaultBalloonText;
    [SerializeField] private TMP_Text rightBalloonText;
    [SerializeField] private TMP_Text downBalloonText;
    [SerializeField] private TMP_Text leftBalloonText;
    [SerializeField] private TMP_Text upBalloonText;
    [SerializeField] private TMP_Text defaultAccuracyText;
    [SerializeField] private TMP_Text colouredAccuracyText;
    [SerializeField] private TMP_Text timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        World transferWorld = null;
        foreach(World world in World.All)
        {
            if(world.Name == "TransferDataWorld")
            {
                transferWorld = world;
            }
        }
        EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //yield return new WaitForSeconds(0.2f);
        Timer timer =_entityManager.CreateEntityQuery(typeof(Timer), typeof(TimeLeftWave)).GetSingleton<Timer>();
        //Debug.Log(JSONWriterSystem.tempDatabase.cursorLocations.Count());
        DynamicBuffer<BalloonWaveBuffer> balloonWavesBuffer = transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).GetSingletonBuffer<BalloonWaveBuffer>();
        SetTimeText(timer);
        SetAccuracyTexts(JSONWriterSystem.tempDatabase.missedShots, JSONWriterSystem.tempDatabase.shots);
        SetBalloonText(balloonWavesBuffer, JSONWriterSystem.tempDatabase.balloonCollection);

    }
    private void SetTimeText(Timer time)
    {
        timerText.text = string.Format("{0}:{1:00}", (int)time.gameTime / 60, (int)time.gameTime % 60);
    }
    private void SetAccuracyTexts(List<MissedShootBuffer> missedShootBuffers, List<ShootBuffer> shootBuffers)
    {
        /*int amountBalloons = 0;
        foreach (BalloonWaveBuffer bwBuffer in balloonWaveBuffers)
        {
            amountBalloons += bwBuffer.balloonSpawns.Value.balloonSpawns.Length;
        }*/
        //shootBuffers.
        int totalshots = shootBuffers.Count();
        {
            int totalMissedBalloons = missedShootBuffers.Count();
            Debug.Log($" {totalshots} {totalMissedBalloons} {(int)(((totalshots-totalMissedBalloons)/totalshots)*100)}");
            int percentage = shootBuffers.Count() != 0 ? (int)(((float)(totalshots-totalMissedBalloons)/(float)totalshots)*100): 0;
            defaultAccuracyText.text = $"{percentage}%";
        }

        {
            int totalMissedColouredBalloons = missedShootBuffers.Where(shots => shots.closestByColourBalloonDistance > 0.25).Count();
            Debug.Log($" {totalshots} {totalMissedColouredBalloons} {((float)(totalshots-totalMissedColouredBalloons )/(float)totalshots)*100}");
            int percentage = shootBuffers.Count() != 0 ? (int)(((float)(totalshots-totalMissedColouredBalloons )/(float)totalshots)*100) : 0;
            colouredAccuracyText.text = $"{percentage}%";
            //colouredAccuracyText.text = "What was the plan again?";
            //????
        }
    }
    private void SetBalloonText(DynamicBuffer<BalloonWaveBuffer> balloonWaveBuffers, List<BalloonCollectionBuffer> balloonCollectionBuffers)
    {
        int defaultBalloonAmount = 0;
        int rightBalloonAmount = 0;
        int downBalloonAmount = 0;
        int leftBalloonAmount = 0;
        int upBalloonAmount = 0;
        foreach (BalloonWaveBuffer bwBuffer in balloonWaveBuffers)
        {
            defaultBalloonAmount += bwBuffer.balloonSpawns.Value.balloonSpawns.ToArray().Where(X => X.balloonType == LaserDirection.Default).Count();
            rightBalloonAmount += bwBuffer.balloonSpawns.Value.balloonSpawns.ToArray().Where(X => X.balloonType == LaserDirection.Right).Count();
            downBalloonAmount += bwBuffer.balloonSpawns.Value.balloonSpawns.ToArray().Where(X => X.balloonType == LaserDirection.Down).Count();
            leftBalloonAmount += bwBuffer.balloonSpawns.Value.balloonSpawns.ToArray().Where(X => X.balloonType == LaserDirection.Left).Count();
            upBalloonAmount += bwBuffer.balloonSpawns.Value.balloonSpawns.ToArray().Where(X => X.balloonType == LaserDirection.Up).Count();
        }
        int defaultBalloonShotAmount = balloonCollectionBuffers.ToArray().Where(X => X.balloonColour == LaserDirection.Default).Count();
        int rightBalloonShotAmount = balloonCollectionBuffers.ToArray().Where(X => X.balloonColour == LaserDirection.Right).Count();
        int downBalloonShotAmount = balloonCollectionBuffers.ToArray().Where(X => X.balloonColour == LaserDirection.Down).Count();
        int leftBalloonShotAmount = balloonCollectionBuffers.ToArray().Where(X => X.balloonColour == LaserDirection.Left).Count();
        int upBalloonShotAmount = balloonCollectionBuffers.ToArray().Where(X => X.balloonColour == LaserDirection.Up).Count();

        defaultBalloonText.text = string.Format("{00}/{00}", defaultBalloonShotAmount, defaultBalloonAmount);
        rightBalloonText.text = string.Format("{00}/{00}", rightBalloonShotAmount, rightBalloonAmount);
        downBalloonText.text = string.Format("{00}/{00}", downBalloonShotAmount, downBalloonAmount);
        leftBalloonText.text = string.Format("{00}/{00}", leftBalloonShotAmount, leftBalloonAmount);
        upBalloonText.text = string.Format("{00}/{00}", upBalloonShotAmount, upBalloonAmount);
    }
}
