using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boton : MonoBehaviour, IPointerClickHandler
{
    [Header("Identificador")]
    public string valor;

    [Header("Relleno")]
    public Renderer rend;
    public Color colorNormal = Color.white;
    public Color colorActivo = Color.cyan;

    [Header("Contorno")]
    public Color colorContorno = Color.cyan;
    public float grosorContorno = 0.01f;
    public float margen = 0.005f;
    private LineRenderer borde;

    [Header("Audio")]
    public AudioClip clickClip;           // arrástralo en el Inspector
    private AudioSource sfx;              // se crea solo

    void Awake()
    {
        if (!rend) rend = GetComponent<Renderer>();
        CrearContorno();
        // AudioSource simple para este botón:
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;            // 2D (igual de fuerte en toda la escena)
        sfx.volume = 1f;
    }

    void CrearContorno()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (!col) return;

        borde = gameObject.AddComponent<LineRenderer>();
        borde.useWorldSpace = false;
        borde.loop = true;
        borde.positionCount = 4;
        borde.widthMultiplier = grosorContorno;
        borde.material = new Material(Shader.Find("Sprites/Default"));
        borde.startColor = borde.endColor = new Color(0, 0, 0, 0);

        Vector3 s = col.size * 0.5f;
        Vector3 c = col.center;
        float x = s.x + margen;
        float y = s.y + margen;
        float z = c.z;
        Vector3[] pts = new Vector3[4] {
            new Vector3(c.x - x, c.y - y, z),
            new Vector3(c.x - x, c.y + y, z),
            new Vector3(c.x + x, c.y + y, z),
            new Vector3(c.x + x, c.y - y, z)
        };
        borde.SetPositions(pts);
        borde.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.RecibirToque(valor);
        StartCoroutine(Flash());
        PlayClick(); // sonido al toque del jugador
    }

    void PlayClick()
    {
        if (clickClip && sfx) sfx.PlayOneShot(clickClip);
    }

    public IEnumerator Flash()
    {
        if (rend) rend.material.color = colorActivo;
        if (borde) { borde.startColor = borde.endColor = colorContorno; borde.enabled = true; }

        // Opcional: también suena cuando el GameManager reproduce la secuencia
        PlayClick();

        yield return new WaitForSeconds(0.2f);

        if (rend) rend.material.color = colorNormal;
        if (borde) borde.enabled = false;
    }
}
