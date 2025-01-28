using System;
using IdleColors.Globals;
using TMPro;
using UnityEngine;

public class PufferCount : MonoBehaviour
{
    public int colorIndex;
    private TextMeshProUGUI count;

    private void Awake()
    {
        count = GetComponentInParent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        count.text = "" + GameManager.Instance.FinalColorCounts[colorIndex];
    }
}