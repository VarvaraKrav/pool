using UnityEngine;
using TMPro; // Include the TextMesh Pro namespace

public class MPHDisplay : MonoBehaviour
{
    public TMP_Text mphText; // Assign this in the inspector
    public float updateInterval = 0.5f; // Time in seconds between updates

    private void Start()
    {
        if (mphText == null)
        {
            mphText = GetComponent<TMP_Text>();
        }
        InvokeRepeating("UpdateMPH", 0, updateInterval);
    }

    void UpdateMPH()
    {
        int mphValue = Random.Range(28, 32); // Random.Range is exclusive of the upper bound
        mphText.text = "0" + mphValue.ToString();
    }
}
