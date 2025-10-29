using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform target;
    public float height = 20f;
    public float smooth = 10f;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 d = new Vector3(target.position.x, height, target.position.z);
        transform.position = Vector3.Lerp(transform.position, d, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
