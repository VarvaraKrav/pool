using UnityEngine;
using TMPro; // Include the TextMesh Pro namespace

public class MPHDisplay : MonoBehaviour
{
    public TMP_Text mphText; // Assign this in the inspector
    public float updateInterval = 0.5f; // Time in seconds between updates

    public Vector3 positionFor28;
    public Vector3 positionFor29;
    public Vector3 positionFor30;
    public Vector3 positionFor31;

    public GameObject mphSlider;

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

        // Move the object based on the generated MPH value
        switch (mphValue)
        {
            case 28:
                mphSlider.transform.localPosition = positionFor28;
                break;
            case 29:
                mphSlider.transform.localPosition = positionFor29;
                break;
            case 30:
                mphSlider.transform.localPosition = positionFor30;
                break;
            case 31:
                mphSlider.transform.localPosition = positionFor31;
                break;
        }

    }

}
