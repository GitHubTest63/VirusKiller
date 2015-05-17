using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private AsyncOperation loading;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public float getProgress()
    {
        if (this.loading == null || this.loading.isDone)
            return 0.0f;
        else
            return this.loading.progress;
    }

    public void goToConnectionScene()
    {
        this.goToScene("connectionScene");
    }

    public void goToMainScene()
    {
        this.goToScene("mainScene");
    }

    public void goToLobbyScene()
    {
        this.goToScene("lobbyScene");
    }

    public void goToScene(string sceneName)
    {
        if (sceneName == null || sceneName.Equals(""))
            Debug.Log("Impossible to load scene " + sceneName);
        PhotonNetwork.isMessageQueueRunning = false;
        this.loading = Application.LoadLevelAsync(sceneName);
    }

    void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.isMessageQueueRunning = true;
        Debug.Log("scene loaded");

    }
}
