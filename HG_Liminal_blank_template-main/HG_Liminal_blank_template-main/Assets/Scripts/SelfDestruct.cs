using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // Public variable to set the time in seconds before destruction
    public float destroyTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Call the Destroy function after 'destroyTime' seconds
        Destroy(gameObject, destroyTime);
    }
}
