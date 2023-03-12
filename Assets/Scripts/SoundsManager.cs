using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance { get; private set; }

    [SerializeField]
    private AudioClip menuSound;
    [SerializeField]
    private AudioClip gameSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        audioSource.clip = menuSound;
        audioSource.Play();
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        audioSource.clip = gameSound;
        audioSource.Play();
    }
}
