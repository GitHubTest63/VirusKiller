using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Start : MonoBehaviour
{
    private InputField id;
    private

    void Start()
    {
        Localisation.Instance.init();
        this.id = this.transform.FindChild("Id").GetComponent<InputField>();

        //auto connection
        string username = PlayerPrefs.GetString("id");
        if (!string.IsNullOrEmpty(username))
        {
            this.id.text = username;
            //Debug.Log("Auto connect ...");
            //NetworkManager.Instance.authenticate(username);
        }
    }

    public void quit()
    {
        Application.Quit();
    }

    public void connect()
    {
        if (string.IsNullOrEmpty(this.id.text))
        {
            //error
            GUIManager.Instance.displayErrorMessage("Empty username");
        }
        else
        {
            PlayerPrefs.SetString("id", this.id.text);
            PlayerPrefs.Save();
            Debug.Log("Connection ...");
            NetworkManager.Instance.authenticate(this.id.text);
        }
    }
}
