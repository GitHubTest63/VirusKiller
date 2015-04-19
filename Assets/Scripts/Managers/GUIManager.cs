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
    private GameObject loading;
    private Slider loadingProgressBar;
    private Text loadingProgressText;
    private GameManager gameManager;
    private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();

    private GameObject errorDisplayer;
    public float errorDisplayingTime = 1.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        this.gameManager = GameManager.Instance;
        this.canvas = this.transform.FindChild("Canvas").gameObject;
        this.loading = findAndRegisterMenu("Loading");
        loadingProgressBar = loading.transform.FindChild("ProgressBar").gameObject.GetComponent<Slider>();
        loadingProgressText = loading.transform.FindChild("ProgressText").gameObject.GetComponent<Text>();


        this.errorDisplayer = canvas.transform.FindChild("ErrorDisplayer").gameObject;
        this.errorDisplayer.SetActive(false);
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

    public void displayErrorMessage(string text)
    {
        StopCoroutine("showErrorMessage");
        StartCoroutine("showErrorMessage", text);
    }

    private IEnumerator showErrorMessage(string text)
    {
        this.errorDisplayer.SetActive(true);
        Text txt = this.errorDisplayer.GetComponentInChildren<Text>();
        txt.text = text;
        CanvasGroup cg = this.errorDisplayer.GetComponent<CanvasGroup>();
        cg.alpha = 1.0f;
        yield return new WaitForSeconds(this.errorDisplayingTime * 0.6f);
        float remainingTime = this.errorDisplayingTime * 0.4f;
        float currentAlpha = cg.alpha;
        while (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime / remainingTime;
            cg.alpha = currentAlpha;
            yield return null;
        }
        this.errorDisplayer.SetActive(false);
    }
}

