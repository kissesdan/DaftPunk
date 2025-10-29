using UnityEngine;

public class Musica : MonoBehaviour
{
    public AudioClip music;    // arrastra tu pista
    [Range(0f, 1f)] public float volume = 0.3f;

    AudioSource src;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // sigue sonando entre escenas
        src = gameObject.AddComponent<AudioSource>();
        src.clip = music;
        src.loop = true;
        src.volume = volume;
        src.spatialBlend = 0f; // 2D
        src.playOnAwake = false;
        src.Play();
    }
}
