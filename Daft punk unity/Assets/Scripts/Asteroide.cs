using UnityEngine;

public enum TipoAsteroide { Normal, Rapido, ZigZag }

public class Asteroide : MonoBehaviour
{
    [Header("Velocidades")]
    public float speedNormal = 12f;
    public float speedRapido = 18f;
    public float zigzagAmplitude = 1.2f;
    public float zigzagFreq = 3.5f;

    float despawnZ;
    float baseX;
    TipoAsteroide tipo;

    // Soporta renderers en hijos (MeshRenderer / SkinnedMeshRenderer / ParticleSystemRenderer)
    Renderer[] renderers;
    Collider anyCollider;

    void Awake()
    {
        // busca TODOS los renderers en este objeto y descendientes
        renderers = GetComponentsInChildren<Renderer>(true);

        // busca un collider en raíz o hijos (para OnTrigger con el Player)
        anyCollider = GetComponent<Collider>();
        if (!anyCollider) anyCollider = GetComponentInChildren<Collider>(true);

        // si existe, vuelve trigger (no crashea si no hay)
        if (anyCollider) anyCollider.isTrigger = true;

        // etiqueta por si el prefab no la trae
        gameObject.tag = "Asteroide";
    }

    public void Setup(TipoAsteroide t, float despawnZWorld)
    {
        tipo = t;
        despawnZ = despawnZWorld;
        baseX = transform.position.x;

        // feedback visual (si hay materials)
        if (renderers != null && renderers.Length > 0)
        {
            Color c = Color.gray;
            switch (tipo)
            {
                case TipoAsteroide.Normal: c = Color.gray; break;
                case TipoAsteroide.Rapido: c = Color.red; break;
                case TipoAsteroide.ZigZag: c = Color.yellow; break;
            }
            // tiñe el primer material de cada renderer (sin crear instancias si no quieres)
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] && renderers[i].material) renderers[i].material.color = c;
            }
        }
    }

    void Update()
    {
        float spd = (tipo == TipoAsteroide.Rapido) ? speedRapido : speedNormal;

        Vector3 p = transform.position;
        p.z -= spd * Time.deltaTime;
        if (tipo == TipoAsteroide.ZigZag)
            p.x = baseX + Mathf.Sin(Time.time * zigzagFreq) * zigzagAmplitude;

        transform.position = p;

        if (p.z < despawnZ) gameObject.SetActive(false);
    }
}
