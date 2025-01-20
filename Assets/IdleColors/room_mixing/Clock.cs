using System;
using UnityEngine;

[ExecuteAlways]
public class Clock : MonoBehaviour
{
    public GameObject seconds;
    public GameObject minutes;
    public GameObject hours;

    private void Awake()
    {
        InvokeRepeating(nameof(SetClock), 1, 1);
    }

    private void SetClock()
    {
        seconds.transform.localRotation = Quaternion.Euler(0, 90, (360 / 60) * DateTime.Now.Second);
        minutes.transform.localRotation = Quaternion.Euler(0, 90, (360 / 60) * DateTime.Now.Minute);
        hours.transform.localRotation = Quaternion.Euler(0, 90, (360 / 12) * (DateTime.Now.Hour % 12));
    }
}