using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Botones (puedes dejar vacío)")]
    public List<Boton> botones = new List<Boton>();

    [Header("Teclas válidas (deben coincidir con Boton.valor)")]
    public string[] teclas = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "*", "#" };

    [Header("Ritmo")]
    public float delayInicio = 0.6f;
    public float delayEntreFlashes = 0.45f;

    // ===== Estado interno =====
    private Dictionary<string, Boton> mapa = new Dictionary<string, Boton>(); // valor -> botón
    private List<string> secuencia = new List<string>(); // array/secuencia del juego
    private int indiceJugador = 0;
    private bool esperandoInput = false;
    private System.Random rng = new System.Random();

    void Awake() { Instance = this; }

    void Start()
    {
        // Autodetecta botones si no se asignaron a mano
        if (botones.Count == 0)
            botones = GetBotonesEnEscena();

        // Construye el mapa valor->botón
        mapa.Clear();
        foreach (var b in botones)
            if (b != null && !string.IsNullOrEmpty(b.valor) && !mapa.ContainsKey(b.valor))
                mapa.Add(b.valor, b);

        NuevaPartida();
    }

    // Compatibilidad: obtiene Boton[] según versión de Unity
    List<Boton> GetBotonesEnEscena()
    {
#if UNITY_2023_1_OR_NEWER
        return new List<Boton>(FindObjectsByType<Boton>(FindObjectsSortMode.None));
#else
        return new List<Boton>(FindObjectsOfType<Boton>());
#endif
    }

    // ===== Lógica principal =====
    public void NuevaPartida()
    {
        secuencia.Clear();
        indiceJugador = 0;
        esperandoInput = false;
        AgregarPaso();
        StartCoroutine(ReproducirSecuencia());
    }

    void AgregarPaso()
    {
        // Elige una tecla que exista en el mapa; evita * y # para la secuencia
        while (true)
        {
            string k = teclas[rng.Next(teclas.Length)];
            if (k != "*" && k != "#" && mapa.ContainsKey(k))
            {
                secuencia.Add(k);
                return;
            }
        }
    }

    IEnumerator ReproducirSecuencia()
    {
        esperandoInput = false;
        indiceJugador = 0;

        yield return new WaitForSeconds(delayInicio);

        // FOR: reproduce la secuencia haciendo brillar cada botón
        for (int i = 0; i < secuencia.Count; i++)
        {
            string key = secuencia[i];
            if (mapa.TryGetValue(key, out var b) && b != null)
                yield return b.StartCoroutine(b.Flash());

            yield return new WaitForSeconds(delayEntreFlashes);
        }

        esperandoInput = true;
    }

    // Llamado por Boton.OnPointerClick
    public void RecibirToque(string valor)
    {
        if (!esperandoInput || secuencia.Count == 0) return;

        // SWITCH: comandos especiales
        switch (valor)
        {
            case "*":  // Reinicia el juego
                NuevaPartida();
                return;
            case "#":  // Repite la secuencia actual
                StartCoroutine(ReproducirSecuencia());
                return;
        }

        // IF: validación del paso actual
        if (valor == secuencia[indiceJugador])
        {
            indiceJugador++;

            // ¿Completó la ronda?
            if (indiceJugador >= secuencia.Count)
            {
                // Límite de niveles: 5
                if (secuencia.Count >= 5)
                {
                    StartCoroutine(Ganar());
                }
                else
                {
                    AgregarPaso();
                    StartCoroutine(ReproducirSecuencia());
                }
            }
        }
        else
        {
            StartCoroutine(FallarYReiniciar());
        }
    }

    IEnumerator FallarYReiniciar()
    {
        esperandoInput = false;
        // Pequeño feedback: todos brillan una vez
        foreach (var kv in mapa)
            kv.Value.StartCoroutine(kv.Value.Flash());
        yield return new WaitForSeconds(0.7f);
        NuevaPartida();
    }

    IEnumerator Ganar()
    {
        esperandoInput = false;
        // Efecto simple de victoria: 3 parpadeos globales
        for (int i = 0; i < 3; i++)
        {
            foreach (var kv in mapa)
                kv.Value.StartCoroutine(kv.Value.Flash());
            yield return new WaitForSeconds(0.4f);
        }
        NuevaPartida();
    }
}
