using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerTest : MonoBehaviour
{
    public float timerDuration = 60f; // タイマーの持続時間（秒）
    private float timer;
    private bool isTimerRunning = false;

    public Text timerText; // タイマーの時間を表示するUIテキスト

    void Start()
    {
        timer = timerDuration;
        UpdateTimerText();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                isTimerRunning = false;
                TimerEnded(); // タイマー終了時の処理
            }

            UpdateTimerText();
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateTimerText()
    {
        timerText.text = timer.ToString("F2");
    }

    private void TimerEnded()
    {
        // タイマーが終了したときの処理
        Debug.Log("タイマーが終了しました！");
    }
}
