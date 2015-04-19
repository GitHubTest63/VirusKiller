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

    public void goToMainMenu()
    {
        this.goToScene("mainMenu");
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
        this.loading = Application.LoadLevelAsync(sceneName);
    }
}
