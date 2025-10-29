using UnityEngine;

public class ControladorPrimeraPersona : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 5f;
    public float velocidadCorrer = 8f;
    
    [Header("Configuración de Cámara")]
    public Transform camaraTransform;
    public float sensibilidadMouse = 2f;
    public float limiteVerticalMin = -90f;
    public float limiteVerticalMax = 90f;
    
    [Header("Configuración de Interacción")]
    public float distanciaInteraccion = 3f;
    public LayerMask capasInteractuables;
    
    private float rotacionX = 0f;
    private CharacterController characterController;
    
    void Start()
    {
        // Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Obtener el CharacterController
        characterController = GetComponent<CharacterController>();
        
        // Si no hay cámara asignada, buscar la Main Camera
        if (camaraTransform == null)
        {
            camaraTransform = Camera.main.transform;
        }
    }
    
    void Update()
    {
        MoverJugador();
        RotarCamara();
        InteractuarConMouse();
        
        // Presionar ESC para desbloquear el cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    void MoverJugador()
    {
        // Obtener input de movimiento
        float movimientoX = Input.GetAxis("Horizontal"); // A/D o flechas
        float movimientoZ = Input.GetAxis("Vertical");   // W/S o flechas
        
        // Determinar velocidad (correr con Shift)
        float velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadMovimiento;
        
        // Calcular dirección de movimiento
        Vector3 direccion = transform.right * movimientoX + transform.forward * movimientoZ;
        
        // Mover el jugador
        if (characterController != null)
        {
            characterController.Move(direccion * velocidadActual * Time.deltaTime);
            
            // Aplicar gravedad simple
            characterController.Move(Vector3.down * 9.8f * Time.deltaTime);
        }
        else
        {
            // Si no hay CharacterController, usar transform
            transform.Translate(direccion * velocidadActual * Time.deltaTime, Space.World);
        }
    }
    
    void RotarCamara()
    {
        // Obtener movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;
        
        // Rotar el jugador horizontalmente (izquierda/derecha)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotar la cámara verticalmente (arriba/abajo)
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);
        camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    }
    
    void InteractuarConMouse()
    {
        // Crear un rayo desde el centro de la cámara
        Ray rayo = new Ray(camaraTransform.position, camaraTransform.forward);
        RaycastHit hit;
        
        // Detectar si el rayo golpea algo
        if (Physics.Raycast(rayo, out hit, distanciaInteraccion, capasInteractuables))
        {
            // Opcional: Mostrar en consola qué estamos mirando
            Debug.Log("Mirando: " + hit.collider.gameObject.name);
            
            // Si hacemos click izquierdo
            if (Input.GetMouseButtonDown(0))
            {
                // Intentar obtener el componente ObjetoInteractuable
                ObjetoInteractuable interactuable = hit.collider.GetComponent<ObjetoInteractuable>();
                
                if (interactuable != null)
                {
                    interactuable.Interactuar();
                }
            }
        }
    }
}