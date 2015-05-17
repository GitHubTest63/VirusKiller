using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatManager : Photon.MonoBehaviour
{
    private static ChatManager instance;
    public static ChatManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Text chatText;

    private GameObject chatContent;
    private InputField chatInput;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this.chatContent = this.transform.FindChild("ScrollView/ChatContent").gameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in this.chatContent.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));
        this.chatInput = this.transform.FindChild("ChatInput").GetComponentInChildren<InputField>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [RPC]
    public void addChatMessage(string sender, string text)
    {
        Text txt = GameObject.Instantiate(this.chatText) as Text;
        txt.text = sender + " : " + text;
        txt.transform.SetParent(this.chatContent.transform, false);
    }

    public void sendChatMessage()
    {
        string txt = this.chatInput.text;
        if (!string.IsNullOrEmpty(txt))
        {
            photonView.RPC("addChatMessage", PhotonTargets.All, NetworkManager.Instance.playerName, txt);
            this.chatInput.text = "";
        }
    }
}
