using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConnectedPlayersManager : Photon.MonoBehaviour
{
    private static ConnectedPlayersManager instance;
    public static ConnectedPlayersManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Text connectedPlayerText;

    private GameObject connectedPlayersContent;
    private Dictionary<string, Text> connectedPlayers = new Dictionary<string, Text>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this.connectedPlayersContent = this.transform.FindChild("ScrollView/Content").gameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in this.connectedPlayersContent.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));
    }

    [RPC]
    private void addConnectedPlayer(string name)
    {
        Debug.Log("add connected player " + name);
        Text txt = GameObject.Instantiate(this.connectedPlayerText) as Text;
        txt.text = name;
        txt.transform.SetParent(this.connectedPlayersContent.transform, false);
        this.connectedPlayers.Add(name, txt);
    }

    public void addPlayer(string name)
    {
        photonView.RPC("addConnectedPlayer", PhotonTargets.AllBuffered, name);
    }

    public void removePlayer(string name)
    {
        photonView.RPC("removeConnectedPlayer", PhotonTargets.AllBuffered, name);
    }

    [RPC]
    private void removeConnectedPlayer(string name)
    {
        Debug.Log("remove connected player " + name);
        Text txt;
        if (this.connectedPlayers.TryGetValue(name, out txt))
        {
            Destroy(txt.gameObject);
            this.connectedPlayers.Remove(name);
        }
    }
}
