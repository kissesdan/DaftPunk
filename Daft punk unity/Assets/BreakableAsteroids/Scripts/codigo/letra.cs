using UnityEngine;
using TMPro;

public class letra : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoVisible = 5f;
    public bool desaparecerAlInicio = true;
    
    private TextMeshProUGUI textoTMP; // CAMBIO: UGUI para Canvas/UI
    
    void Start()
    {
        textoTMP = GetComponent<TextMeshProUGUI>(); // CAMBIO: UGUI
        
        if (desaparecerAlInicio && textoTMP != null)
        {
            Invoke("OcultarTexto", tiempoVisible);
        }
    }
    
    void OcultarTexto()
    {
        if (textoTMP != null)
        {
            textoTMP.enabled = false;
        }
    }
    
    public void MostrarTexto()
    {
        if (textoTMP != null)
        {
            textoTMP.enabled = true;
            Invoke("OcultarTexto", tiempoVisible);
        }
    }
}