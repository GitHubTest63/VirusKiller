using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Start : MonoBehaviour
{
    private InputField id;
    private InputField mp;
    private Toggle rememberMe;
    private

    void Start()
    {
        this.id = this.transform.FindChild("Id").GetComponent<InputField>();
        this.mp = this.transform.FindChild("Password").GetComponent<InputField>();
        this.rememberMe = this.transform.FindChild("RememberMe").GetComponent<Toggle>();

        //auto connection
        string username = PlayerPrefs.GetString("id");
        if (!string.IsNullOrEmpty(username))
        {
            this.id.text = username;
            string mp = PlayerPrefs.GetString("mp");
            if (!string.IsNullOrEmpty(mp))
            {
                Debug.Log("Auto connect ...");
                NetworkManager.Instance.authenticate(username, mp);
            }
        }
    }

    public void quit()
    {
        Application.Quit();
    }

    public void connect()
    {
        if (string.IsNullOrEmpty(this.id.text) || string.IsNullOrEmpty(this.mp.text))
        {
            //error
            GUIManager.Instance.displayErrorMessage("Empty informations !");
        }
        else
        {
            if (this.rememberMe.isOn)
            {
                PlayerPrefs.SetString("id", this.id.text);
                PlayerPrefs.SetString("mp", this.mp.text);
                PlayerPrefs.Save();
                Debug.Log("connection data saved");
            }
            Debug.Log("Connection ...");
            NetworkManager.Instance.authenticate(this.id.text, this.mp.text);
        }
    }
}
