using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MPGDisplay : MonoBehaviour
{
    public TMP_Text mpgText; // Assign this in the inspector
    public float updateInterval = 0.5f; // Time in seconds between updates

    private void Start()
    {
        if (mpgText == null)
        {
            mpgText = GetComponent<TMP_Text>();
        }
        InvokeRepeating("UpdateMPG", 0, updateInterval);
    }

    void UpdateMPG()
    {
        int mpgValue = Random.Range(116, 128); // Random.Range is exclusive of the upper bound
        mpgText.text = mpgValue.ToString();
    }
}
