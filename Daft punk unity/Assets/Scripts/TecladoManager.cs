using UnityEngine;
using TMPro;

public class TecladoManager : MonoBehaviour
{
    public static TecladoManager Instance { get; private set; }

    [Header("Pantalla opcional (TextMeshPro)")]
    public TMP_Text pantalla;

    [Header("Lógica del teclado")]
    public string entradaActual = "";
    public string codigoCorrecto = "1234";

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PresionarBoton(string valor)
    {
        if (valor == "*")
        {
            entradaActual = "";
        }
        else if (valor == "#")
        {
            ValidarCodigo();
        }
        else
        {
            entradaActual += valor;
        }

        if (pantalla)
            pantalla.text = entradaActual;

        Debug.Log($"Botón presionado: {valor} → {entradaActual}");
    }

    void ValidarCodigo()
    {
        if (entradaActual == codigoCorrecto)
        {
            Debug.Log("Código correcto");
        }
        else
        {
            Debug.Log("Código incorrecto");
        }

        // Puedes reiniciar o dejar el texto
        // entradaActual = "";
    }
}
