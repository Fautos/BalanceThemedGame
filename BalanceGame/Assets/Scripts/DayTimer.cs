using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DayTimer : MonoBehaviour
{
    public TMP_Text timer;
    public bool dayOver;
    [SerializeField] int timePerDay, timeLeft;

    // Start is called before the first frame update
    public void StartTimer()
    {
        // Inicialization of the variables
        timePerDay = 20;
        timeLeft = timePerDay;
        dayOver = false;

        StartCoroutine(DayTimerCoroutine());
    }

    // Timer
    IEnumerator DayTimerCoroutine()
    {
        while (dayOver == false)
        {
            yield return new WaitForSeconds(1);
            timeLeft --;
            UpdateTimer();

            if (timeLeft <= 0)
            {
                dayOver = true;
            }
        }
        
    }

    // Funtion to update the timer counter
    void UpdateTimer()
    {
        int minutes = timeLeft/60, seconds = timeLeft%60;
        string minText, secText;

        // With this "ifs" we ensure that the timer text always has 2 digits
        if (minutes < 10)
        {
            minText = "0" + minutes;
        }
        else
        {
            minText = minutes.ToString();
        }

        if (seconds < 10)
        {
            secText = "0" + seconds;
        }
        else
        {
            secText = seconds.ToString();
        }

        // Then we update the timer
        timer.text = minText + ":" + secText;
    }

    // In case we need to stop the timer
    public void FinishDay()
    {
        if (dayOver != true)
        {
            StopCoroutine(DayTimerCoroutine());
            dayOver = true;
        }
        
    }
}
