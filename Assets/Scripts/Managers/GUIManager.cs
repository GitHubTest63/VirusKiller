using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

    private static GUIManager instance;
    public static GUIManager Instance
    {
        get { return instance; }
    }
    private GameObject canvas;
    private GameObject options;
    private GameObject leaderboard;
    private GameObject lobby;
    private GameObject loading;
    private Slider loadingProgressBar;
    private Text loadingProgressText;
    private GameManager gameManager;
    private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();

    void Start()
    {
        this.gameManager = GameManager.Instance;
        this.canvas = this.transform.FindChild("Canvas").gameObject;
        this.options = findAndRegisterMenu("Options");
        this.leaderboard = findAndRegisterMenu("Leaderboard");
        this.lobby = findAndRegisterMenu("Lobby");
        this.loading = findAndRegisterMenu("Loading");
        loadingProgressBar = loading.transform.FindChild("ProgressBar").gameObject.GetComponent<Slider>();
        loadingProgressText = loading.transform.FindChild("ProgressText").gameObject.GetComponent<Text>();
    }

    void Update()
    {
        if (this.loading.activeSelf)
        {
            if (this.gameManager.getProgress() > 0)
            {
                loadingProgressBar.value = this.gameManager.getProgress();
                loadingProgressText.text = (loadingProgressBar.value * 100.0f).ToString() + "%";
            }
            else
            {
                deactivate("Loading");
            }
        }
        else
        {
            if (this.gameManager.getProgress() > 0)
            {
                activate("Loading");
            }
        }
    }

    private GameObject findAndRegisterMenu(string name)
    {
        GameObject go = this.canvas.transform.FindChild(name).gameObject;
        this.menus.Add(name, go);
        return go;
    }

    public void activate(string name)
    {
        this.setActive(name, true);
    }

    public void deactivate(string name)
    {
        this.setActive(name, false);
    }

    private void setActive(string name, bool active)
    {
        GameObject go;
        if (this.menus.TryGetValue(name, out go))
        {
            deactivateAllMenus();
            go.SetActive(active);
        }
    }

    private void deactivateAllMenus()
    {
        foreach (KeyValuePair<string, GameObject> entry in this.menus)
        {
            // do something with entry.Value or entry.Key
            entry.Value.SetActive(false);
        }
    }

    public void quit()
    {
        Application.Quit();
    }
}

