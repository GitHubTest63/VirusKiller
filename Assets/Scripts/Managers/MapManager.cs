using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapManager : Photon.MonoBehaviour
{
    [System.Serializable]
    public struct Map
    {
        public string name;
        public Sprite icon;
        public string description;
    }

    private static MapManager instance;
    public static MapManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Text selectedPlayerText;

    private GameObject selectedPlayersContent;
    private Dictionary<string, List<string>> selectedPlayers = new Dictionary<string, List<string>>();

    public List<Map> availablesMaps;
    private int currentMapIndex = -1;
    private Image mapIcon;
    private Text mapDesc;
    private Text mapName;
    private Text chooseButtonText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this.selectedPlayersContent = this.transform.FindChild("SelectedPlayers/ScrollView/Content").gameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in this.selectedPlayersContent.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));
        this.mapIcon = this.transform.FindChild("MapSelection/MapIcon").GetComponent<Image>();
        this.mapDesc = this.transform.FindChild("MapSelection/Description").GetComponentInChildren<Text>();
        this.mapName = this.transform.FindChild("MapSelection/Name").GetComponent<Text>();
        this.chooseButtonText = this.transform.FindChild("MapSelection/Choose").GetComponentInChildren<Text>();
        if (this.availablesMaps.Count > 0)
        {
            this.currentMapIndex = 0;
            this.updateMapIndex();
        }
    }

    private int updateMapIndex()
    {
        if (currentMapIndex < 0)
        {
            currentMapIndex = this.availablesMaps.Count - 1;
        }
        else if (currentMapIndex >= this.availablesMaps.Count)
        {
            currentMapIndex = currentMapIndex % this.availablesMaps.Count;
        }
        this.updateMap();
        return currentMapIndex;
    }

    private Map getCurrentMap()
    {
        return this.availablesMaps[this.currentMapIndex];
    }

    public void selectCurrentMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        this.selectMap(m.name);
    }

    public void selectMap(string mapName)
    {
        List<string> players = this.getOrCreatePlayerList(mapName);
        if (players.Contains(NetworkManager.Instance.playerName))
        {
            this.photonView.RPC("removePlayer", PhotonTargets.AllBuffered, mapName, NetworkManager.Instance.playerName);
        }
        else
        {
            this.photonView.RPC("addPlayer", PhotonTargets.AllBuffered, mapName, NetworkManager.Instance.playerName);
        }
        this.updateChooseButtonText();
    }

    public void nextMap()
    {
        currentMapIndex++;
        this.updateMapIndex();
    }

    public void previousMap()
    {
        currentMapIndex--;
        this.updateMapIndex();
    }

    private void updateMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        this.mapName.text = m.name;
        this.mapIcon.sprite = m.icon;
        this.mapDesc.text = m.description;
        this.updateChooseButtonText();
        this.updatePlayers();
    }

    private void updateChooseButtonText()
    {
        List<string> players = this.getOrCreatePlayerList(this.mapName.text);
        if (players.Contains(NetworkManager.Instance.playerName))
        {
            this.chooseButtonText.text = Localisation.Instance.get("selectedMapChooseButtonText");
        }
        else
        {
            this.chooseButtonText.text = Localisation.Instance.get("defaultMapChooseButtonText");
        }
    }

    public void playSelectedMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        List<string> players = this.getOrCreatePlayerList(m.name);
        if (players.Contains(NetworkManager.Instance.playerName))
        {
            //want to play in multi
            GUIManager_Lobby.Instance.invite(NetworkManager.Instance.playerName, m.name);
        }
        else
        {
            //want to play solo      
            this.removePlayer(NetworkManager.Instance.playerName);
            NetworkManager.Instance.joinGame(m.name);
        }
    }

    private List<string> getPlayerList(string mapName)
    {
        List<string> players;
        this.selectedPlayers.TryGetValue(mapName, out players);
        return players;
    }

    private List<string> getOrCreatePlayerList(string mapName)
    {
        List<string> players = this.getPlayerList(mapName);
        if (players == null)
        {
            players = new List<string>();
            this.selectedPlayers.Add(mapName, players);
        }
        return players;
    }

    [RPC]
    public void addPlayer(string mapName, string playerName)
    {
        List<string> players = this.getOrCreatePlayerList(mapName);
        players.Add(playerName);
        if (mapName.Equals(this.mapName.text))
        {
            updatePlayers();
        }
    }

    private void updatePlayers()
    {
        clearPlayersText();
        List<string> players = this.getOrCreatePlayerList(this.mapName.text);
        foreach (string p in players)
        {
            Text txt = GameObject.Instantiate(this.selectedPlayerText) as Text;
            txt.text = p;
            txt.transform.SetParent(this.selectedPlayersContent.transform, false);
        }
    }

    private void clearPlayersText()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in this.selectedPlayersContent.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));
    }

    public void removePlayer(string playerName)
    {
        foreach (Map map in this.availablesMaps)
        {
            this.photonView.RPC("removePlayer", PhotonTargets.AllBuffered, map.name, playerName);
        }
    }

    [RPC]
    public void removePlayer(string mapName, string playerName)
    {
        List<string> players = this.getPlayerList(mapName);
        if (players == null)
        {
            return;
        }
        players.Remove(playerName);
        if (mapName.Equals(this.mapName.text))
        {
            updatePlayers();
        }
    }
}
