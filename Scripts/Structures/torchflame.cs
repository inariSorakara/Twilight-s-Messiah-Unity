using UnityEngine;

public class torchflameflicker : MonoBehaviour
{
    private Light torchLight;
    public float minIntensity = 0.8f;

    public float maxIntensity = 1.2f;

    public float flickerSpeed = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, Random.Range(minIntensity, maxIntensity), flickerSpeed);
    }
}