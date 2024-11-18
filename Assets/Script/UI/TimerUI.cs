using HANDFORCE.TCCavy.General.Data;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timer_Text;
    private void OnEnable() {
        TimerSystem.OnTimeTick += ChangeTime;
    }
    private void ChangeTime(float time)
    {
        timer_Text.text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
    }
    private void OnDisable()
    {
        TimerSystem.OnTimeTick -= ChangeTime;
    }
}
