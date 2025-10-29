using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 8f;
    public float suavizadoMovimiento = 10f;

    [Header("Cámara")]
    public float sensibilidadRaton = 2f;
    public Transform camara;
    public float limiteVerticalMin = -80f;
    public float limiteVerticalMax = 80f;

    private float rotacionX = 0f;
    private Vector3 velocidadActual;
    private bool puedeMoverse = true;

    void Start()
    {
        // Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Si no asignaste la cámara manualmente, buscarla
        if (camara == null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        if (puedeMoverse)
        {
            MoverJugador();
            RotarCamara();
        }

        // Presionar ESC para liberar el cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Click para volver a bloquear el cursor
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void MoverJugador()
    {
        // Obtener input del teclado (WASD o flechas)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Dirección del movimiento
        Vector3 direccion = transform.right * horizontal + transform.forward * vertical;
        direccion.Normalize();

        // Velocidad (con o sin correr)
        float velocidad = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadCaminar;

        // Aplicar movimiento suavizado
        Vector3 velocidadObjetivo = direccion * velocidad;
        velocidadActual = Vector3.Lerp(velocidadActual, velocidadObjetivo, suavizadoMovimiento * Time.deltaTime);

        // Mover el jugador
        transform.position += velocidadActual * Time.deltaTime;
    }

    void RotarCamara()
    {
        // Obtener movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        // Rotación horizontal (Y) del jugador
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical (X) de la cámara
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    }

    // Método para desactivar/activar el movimiento
    public void SetPuedeMoverse(bool puede)
    {
        puedeMoverse = puede;
    }
}