using UnityEngine;
public class Mapa : MonoBehaviour
{
    public float degreesPerSecond = 2f;
    float angle;
    void Update()
    {
        angle = (angle + degreesPerSecond * Time.deltaTime) % 360f;
        if (RenderSettings.skybox.HasProperty("_Rotation"))
            RenderSettings.skybox.SetFloat("_Rotation", angle);
            DynamicGI.UpdateEnvironment();
    }
}
