using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManagerGame : MonoBehaviour
{
    [Header("UI")]
    public Text timerText;
    public Text statusText;

    [Header("Referencias")]
    public Player player;             
    public Camera mainCamera;         
    public Asteroide prefab;         

    [Header("Juego")]
    public float gameDuration = 30f;

    [Header("Spawn (Inspector)")]
    [Tooltip("Cuántos asteroides se crean cada vez")]
    public int asteroidesPorOla = 1;

    [Tooltip("Segundos entre spawns al inicio")]
    public float intervaloInicial = 1.0f;

    [Tooltip("Segundos entre spawns cuando ya está difícil")]
    public float intervaloMinimo = 0.45f;

    [Tooltip("1 = lineal, >1 acelera la dificultad, <1 la suaviza")]
    public float curvaDificultad = 1.0f;

    [Tooltip("Tamaño del pool (máximo de asteroides simultáneos)")]
    public int poolSize = 16;

    [Header("Tipos de asteroide (pesos)")]
    [Tooltip("Pesos: Normal, Rapido, ZigZag")]
    public int[] typeWeights = new int[] { 60, 25, 15 };

    [Header("Límites de spawn dentro de cámara")]
    [Tooltip("Margen interior respecto a los bordes visibles")]
    public float margen = 0.5f;

    [Tooltip("Cuánto por debajo del borde inferior se desactiva")]
    public float extraZ = 6f;

    [Header("Timing")]
    [Tooltip("Si está ON, ignora Time.timeScale en el spawn")]
    public bool usarTiempoNoEscalado = true;

    // ---- Estado interno ----
    Asteroide[] pool;
    int poolIdx;
    float timeRemaining;
    bool playing;

    void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;

        // Pool
        pool = new Asteroide[Mathf.Max(1, poolSize)];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(prefab, Vector3.one * 999f, Quaternion.identity);
            pool[i].gameObject.SetActive(false);
            pool[i].gameObject.tag = "Asteroide";
        }

        NuevoJuego();
    }

    public void NuevoJuego()
    {
        for (int i = 0; i < pool.Length; i++) pool[i].gameObject.SetActive(false);

        timeRemaining = gameDuration;
        playing = true;
        if (statusText) statusText.text = "Esquiva con WASD";
        ActualizarTimer();

        StopAllCoroutines();
        StartCoroutine(BucleTimer());
        StartCoroutine(BucleSpawn());
    }

    IEnumerator BucleTimer()
    {
        while (playing && timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;   // el timer sí respeta timeScale
            ActualizarTimer();
            yield return null;
        }
        if (playing) Ganar();
    }

    IEnumerator BucleSpawn()
    {
        if (usarTiempoNoEscalado) yield return new WaitForSecondsRealtime(0.6f);
        else yield return new WaitForSeconds(0.6f);

        while (playing && timeRemaining > 0f)
        {
            for (int i = 0; i < Mathf.Max(1, asteroidesPorOla); i++)
                SpawnUno();

            float progreso = 1f - (timeRemaining / Mathf.Max(0.0001f, gameDuration)); // 0→1
            float t = Mathf.Clamp01(Mathf.Pow(progreso, Mathf.Max(0.0001f, curvaDificultad)));
            float intervaloActual = Mathf.Lerp(intervaloInicial, intervaloMinimo, t);

            if (usarTiempoNoEscalado) yield return new WaitForSecondsRealtime(intervaloActual);
            else yield return new WaitForSeconds(intervaloActual);
        }
    }

    // ---------- NUEVO: rectángulo visible actual (cámara ORTHO cenital) ----------
    void RectCamaraOrtho(out float minX, out float maxX, out float minZ, out float maxZ)
    {
        var cam = mainCamera ? mainCamera : Camera.main;
        if (!cam || !cam.orthographic)
        {
            // Fallback por si falta la cámara
            minX = -8f; maxX = 8f; minZ = -5f; maxZ = 10f;
            return;
        }

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;

        minX = c.x - halfW;
        maxX = c.x + halfW;
        minZ = c.z - halfH;
        maxZ = c.z + halfH;
    }

    // ---------- NUEVO: spawnea dentro de lo visible por la cámara ----------
    void SpawnUno()
    {
        float minX, maxX, minZ, maxZ;
        RectCamaraOrtho(out minX, out maxX, out minZ, out maxZ);

        float m = Mathf.Max(0f, margen);

        // Posición aleatoria DENTRO de la pantalla
        float x = Random.Range(minX + m, maxX - m);
        float z = Random.Range(minZ + m, maxZ - m);
        Vector3 pos = new Vector3(x, 0.5f, z);

        // Evitar spawnear encima del jugador (simple)
        if (player)
        {
            Vector3 pp = player.transform.position;
            if (Mathf.Abs(pos.x - pp.x) < 1.0f && Mathf.Abs(pos.z - pp.z) < 1.0f)
                pos.z = Mathf.Clamp(pp.z + 2.0f, minZ + m, maxZ - m);
        }

        // Desaparece cuando pase por debajo del borde inferior
        float despawnZ = (minZ - extraZ);

        var ob = pool[poolIdx];
        poolIdx = (poolIdx + 1) % pool.Length;

        ob.transform.position = pos;
        ob.Setup(TipoPonderado(), despawnZ);
        ob.gameObject.SetActive(true);
    }

    public TipoAsteroide TipoPonderado()
    {
        int total = 0; for (int i = 0; i < typeWeights.Length; i++) total += typeWeights[i];
        int r = Random.Range(0, total), acc = 0;
        for (int i = 0; i < typeWeights.Length; i++)
        {
            acc += typeWeights[i];
            if (r < acc) return (TipoAsteroide)i;
        }
        return TipoAsteroide.Normal;
    }

    void ActualizarTimer()
    {
        if (timerText) timerText.text = "Tiempo: " + Mathf.CeilToInt(Mathf.Max(0, timeRemaining));
    }

    void Ganar()
    {
        playing = false;
        if (statusText) statusText.text = "¡Ganaste! (R para otra)";
        StopAllCoroutines();
        for (int i = 0; i < pool.Length; i++) pool[i].gameObject.SetActive(false);
    }

    void Perder(string msg)
    {
        playing = false;
        if (statusText) statusText.text = msg;
        StopAllCoroutines();
        for (int i = 0; i < pool.Length; i++) pool[i].gameObject.SetActive(false);
    }

    public void OnPlayerHit()
    {
        if (!playing) return;
        Perder("¡Choque! (R para reiniciar)");
    }

    void Update()
    {
        if (!playing && Input.GetKeyDown(KeyCode.R))
            NuevoJuego();
    }
}
