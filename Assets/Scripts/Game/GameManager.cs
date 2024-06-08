using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Range(0f, 1f)] [SerializeField] private float volume = 0.5f;
    [Header("Player Die Settings")]
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject playerDeathParticles = null;
    [SerializeField] private float timeResetPlayer = 1.5f;

    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject playerCamera = null;
    [SerializeField] private GameObject UI_Pause = null;
    [SerializeField] private GameObject resumeButton = null;
    [SerializeField] private PanelManager panelManager = null;
    private bool pause = false;

    static GameManager mInstance;

    private AudioSource source;

    public static GameManager Instance
    {
        get
        {
            return mInstance;
        }
    }

    MiscInputs miscInputs;

    void Awake()
    {
        mInstance = this;
     
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.volume = volume;
        source.Play();
        miscInputs = GetComponent<MiscInputs>();
    }

    private IEnumerator coroutinePlayerReset()
    {
        yield return new WaitForSeconds(timeResetPlayer);

        player.SetActive(true);
        player.GetComponent<Player>().ResetPlayer();
    }


    public void PlayerDie()
    {
        player.SetActive(false);
        Instantiate(playerDeathParticles, player.transform.position, Quaternion.identity).GetComponent<Transform>().SetParent(null);
        StartCoroutine("coroutinePlayerReset");
    }

    private IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(1f);
        UI_Pause.SetActive(false);
        playerCamera.SetActive(true);
        player.SetActive(true);
    }

    public void Pause()
   {
        if (!pause)
        {
            UI_Pause.SetActive(true);
            playerCamera.SetActive(false);
            player.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        else
        {
            StartCoroutine("CloseMenu");
        }

        pause = !pause;
   }

   
   public void LoadMenu()
   {
        SceneManager.LoadScene(0);
   }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != 60)
            Application.targetFrameRate = 60;

       //if (miscInputs.ResetSceneKey)
       //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       //if (miscInputs.ExitGameKey)
       //    Application.Quit();

        if (miscInputs.PauseKey && !UI_Pause.activeSelf)
            Pause();
    }
}
