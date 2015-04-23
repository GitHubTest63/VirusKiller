using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Game : MonoBehaviour
{
    private static GUIManager_Game instance;
    public static GUIManager_Game Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (GUIManager_Game.instance == null)
        {
            GUIManager_Game.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private Slider healthBar;
    private Text healthText;

    void Start()
    {
        GameObject healthBar = GameObject.Find("HealthBar");
        this.healthBar = healthBar.GetComponent<Slider>();
        this.healthText = healthBar.transform.FindChild("Text").GetComponent<Text>();
    }

    public void setHealth(int currentHealth, int maxHealth)
    {
        float healthProgress = currentHealth / (float)maxHealth;
        this.healthBar.value = healthProgress;
        this.healthText.text = currentHealth + "/" + maxHealth + " " + (int)(healthProgress * 100) + "%";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
