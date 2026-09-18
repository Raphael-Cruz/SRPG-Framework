using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [SerializeField] bool isBlinking = false;
    [SerializeField] float timeDelay;
    [SerializeField] float stayOnTime = 10f;
    Light lights;
    void Start()
    {
        lights = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkingLights());
        }
    }   
    IEnumerator BlinkingLights()
    {
        isBlinking = true;
        lights.enabled = false;
        timeDelay = Random.Range(0.02f, 0.5f);
        yield return new WaitForSeconds(timeDelay);
        lights.enabled = true;
        timeDelay = Random.Range(0.1f, 0.4f);
        yield return new WaitForSeconds(timeDelay);
        lights.enabled = false;
        timeDelay = Random.Range(0.1f, 0.4f);
        yield return new WaitForSeconds(timeDelay);
        lights.enabled = true;
        yield return new WaitForSeconds(stayOnTime);
        isBlinking = false;

    }

    
}
