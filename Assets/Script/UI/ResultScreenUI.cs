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
        Debug.Log(JSONWriterSystem.tempDatabase.cursorLocations.Count());
        DynamicBuffer<BalloonWaveBuffer> balloonWavesBuffer = transferWorld.EntityManager.CreateEntityQuery(typeof(BalloonWaveBuffer)).GetSingletonBuffer<BalloonWaveBuffer>();
        SetTimeText(timer);
        SetAccuracyTexts(JSONWriterSystem.tempDatabase.missedShots, balloonWavesBuffer);
        SetBalloonText(balloonWavesBuffer, JSONWriterSystem.tempDatabase.balloonCollection);

    }
    private void SetTimeText(Timer time)
    {
        timerText.text = string.Format("{0}:{1:00}", (int)time.gameTime / 60, (int)time.gameTime % 60);
    }
    private void SetAccuracyTexts(List<MissedShootBuffer> missedShootBuffers, DynamicBuffer<BalloonWaveBuffer> balloonWaveBuffers)
    {
        int amountBalloons = 0;
        foreach (BalloonWaveBuffer bwBuffer in balloonWaveBuffers)
        {
            amountBalloons += bwBuffer.balloonSpawns.Value.balloonSpawns.Length;
        }

        {
            int totalMissedBalloons = missedShootBuffers.Count();
            Debug.Log($" {amountBalloons} {totalMissedBalloons} {(float)amountBalloons/(float)totalMissedBalloons}");
            int percentage = (int)(((float)amountBalloons/(float)totalMissedBalloons)*100);
            defaultAccuracyText.text = $"{percentage}%";
        }

        {
            colouredAccuracyText.text = "What was the plan again?";
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

        defaultBalloonText.text = string.Format("{00}/{00} Balloons popped", defaultBalloonShotAmount, defaultBalloonAmount);
        rightBalloonText.text = string.Format("{00}/{00} Balloons popped", rightBalloonShotAmount, rightBalloonAmount);
        downBalloonText.text = string.Format("{00}/{00} Balloons popped", downBalloonShotAmount, downBalloonAmount);
        leftBalloonText.text = string.Format("{00}/{00} Balloons popped", leftBalloonShotAmount, leftBalloonAmount);
        upBalloonText.text = string.Format("{00}/{00} Balloons popped", upBalloonShotAmount, upBalloonAmount);
    }
}
