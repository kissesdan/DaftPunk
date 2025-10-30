using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("Tipo de Interacción")]
    public TipoInteraccion tipoInteraccion = TipoInteraccion.Desaparecer;
    
    [Header("Configuración")]
    public string mensajeInteraccion = "Objeto interactuado";
    public Color colorResaltado = Color.yellow;
    
    private Color colorOriginal;
    private Renderer objetoRenderer;
    private bool yaInteractuado = false;
    
    public enum TipoInteraccion
    {
        Desaparecer,
        CambiarColor,
        MostrarMensaje,
        Rotar,
        Escalar
    }
    
    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        if (objetoRenderer != null)
        {
            colorOriginal = objetoRenderer.material.color;
        }
    }
    
    public void Interactuar()
    {
        if (yaInteractuado) return;
        
        Debug.Log(mensajeInteraccion);
        
        switch (tipoInteraccion)
        {
            case TipoInteraccion.Desaparecer:
                DesaparecerObjeto();
                break;
                
            case TipoInteraccion.CambiarColor:
                CambiarColor();
                break;
                
            case TipoInteraccion.MostrarMensaje:
                MostrarMensaje();
                break;
                
            case TipoInteraccion.Rotar:
                RotarObjeto();
                break;
                
            case TipoInteraccion.Escalar:
                EscalarObjeto();
                break;
        }
        
        yaInteractuado = true;
    }
    
    void DesaparecerObjeto()
    {
        gameObject.SetActive(false);
    }
    
    void CambiarColor()
    {
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorResaltado;
        }
    }
    
    void MostrarMensaje()
    {
        Debug.Log("¡Has interactuado con: " + gameObject.name + "!");
    }
    
    void RotarObjeto()
    {
        transform.Rotate(0, 90, 0);
    }
    
    void EscalarObjeto()
    {
        transform.localScale *= 1.5f;
    }
    
    public void ResetearInteraccion()
    {
        yaInteractuado = false;
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorOriginal;
        }
    }
}