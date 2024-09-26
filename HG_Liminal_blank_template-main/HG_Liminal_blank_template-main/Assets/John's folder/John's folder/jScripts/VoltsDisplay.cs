using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoltsDisplay : MonoBehaviour
{
    public TMP_Text voltsText; // Assign this in the inspector
    public float updateInterval = 0.5f; // Time in seconds between updates

    private void Start()
    {
        if (voltsText == null)
        {
            voltsText = GetComponent<TMP_Text>();
        }
        InvokeRepeating("UpdateVolts", 0, updateInterval);
    }

    void UpdateVolts()
    {
        int voltsValue = Random.Range(130, 133); // Random.Range is exclusive of the upper bound
        voltsText.text = voltsValue.ToString();
    }
}
