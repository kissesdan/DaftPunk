using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 12f;

    [Header("Límites de movimiento (X/Z)")]
    public Vector2 xBounds = new Vector2(-8f, 8f);
    public Vector2 zBounds = new Vector2(-5f, 10f);

    [Header("Autocalcular bounds desde cámara")]
    public bool autoBoundsFromCamera = true;
    public Camera cam;                 // si está vacío usa Camera.main
    [Range(0f, 5f)] public float margin = 0.5f;

    [Header("Refs")]
    public ManagerGame manager;

    Rigidbody rb;
    Vector3 inputDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        if (!manager) manager = FindObjectOfType<ManagerGame>();
        if (!cam) cam = Camera.main;
    }

    void Start()
    {
        if (autoBoundsFromCamera) SetBoundsFromCamera();
    }

    void Update()
    {
        float h = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float v = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        inputDir = new Vector3(h, 0f, v).normalized;
    }

    void FixedUpdate()
    {
        Vector3 p = rb.position + inputDir * speed * Time.fixedDeltaTime;
        p.x = Mathf.Clamp(p.x, xBounds.x, xBounds.y);
        p.z = Mathf.Clamp(p.z, zBounds.x, zBounds.y);
        rb.MovePosition(p);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroide"))
            manager?.OnPlayerHit();
    }

    [ContextMenu("Recalcular bounds desde cámara")]
    public void SetBoundsFromCamera()
    {
        if (!cam) cam = Camera.main;
        if (cam == null || !cam.orthographic)
        {
            Debug.LogWarning("[Player] Necesitas una cámara ORTHOGRAPHIC cenital para autocalcular bounds.");
            return;
        }

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        xBounds = new Vector2(-halfW + margin, halfW - margin);
        zBounds = new Vector2(-halfH + margin, halfH - margin);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = new Vector3(xBounds.x, transform.position.y, zBounds.x);
        Vector3 b = new Vector3(xBounds.y, transform.position.y, zBounds.x);
        Vector3 c = new Vector3(xBounds.y, transform.position.y, zBounds.y);
        Vector3 d = new Vector3(xBounds.x, transform.position.y, zBounds.y);
        Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);
    }
}
