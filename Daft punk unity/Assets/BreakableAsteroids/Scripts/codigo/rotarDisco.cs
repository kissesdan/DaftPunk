using UnityEngine;

public class RotarDisco : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public float velocidadRotacion = 33f; // RPM como un tocadiscos (33, 45, o 78)
    
    [Header("Opciones")]
    public bool rotarAlInicio = true; // Empieza rotando automáticamente
    private bool estaRotando = true;
    
    void Start()
    {
        estaRotando = rotarAlInicio;
    }
    
    void Update()
    {
        if (estaRotando)
        {
            // Rotar el disco HORIZONTAL como tocadiscos
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.Self);
        }
    }
    
    // Método para activar/desactivar la rotación desde otro script
    public void ActivarRotacion()
    {
        estaRotando = true;
    }
    
    public void DesactivarRotacion()
    {
        estaRotando = false;
    }
    
    public void AlternarRotacion()
    {
        estaRotando = !estaRotando;
    }
}