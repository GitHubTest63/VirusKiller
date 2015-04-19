using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
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
    private Dictionary<string, Dictionary<string, Text>> selectedPlayers = new Dictionary<string, Dictionary<string, Text>>();

    public List<Map> availablesMaps;
    private int currentMapIndex = -1;
    private Image mapIcon;
    private Text mapDesc;
    private Text mapName;

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
        if (this.availablesMaps.Count > 0)
        {
            this.currentMapIndex = 0;
            this.updateMapIndex();
        }
    }

    /*public void addMap(Map mapName)
    {

    }*/

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

    public void selectMap(string mapName)
    {
        NetworkManager.Instance.sendSelectedMap(mapName);
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
    }

    public void playSelectedMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        NetworkManager.Instance.sendPlayMap(m.name);
    }

    private Dictionary<string, Text> getPlayerList(string mapName)
    {
        Dictionary<string, Text> players;
        this.selectedPlayers.TryGetValue(mapName, out players);
        return players;
    }

    private Dictionary<string, Text> getOrCreatePlayerList(string mapName)
    {
        Dictionary<string, Text> players = this.getPlayerList(mapName);
        if (players == null)
        {
            players = new Dictionary<string, Text>();
            this.selectedPlayers.Add(mapName, players);
        }
        return players;
    }

    public void addPlayer(string mapName, string playerName)
    {
        Dictionary<string, Text> players = this.getOrCreatePlayerList(mapName);
        if (players.ContainsKey(playerName))
        {
            return;
        }
        Debug.Log("add " + playerName + " to map " + mapName);
        Text txt = GameObject.Instantiate(this.selectedPlayerText) as Text;
        txt.text = playerName;
        txt.transform.SetParent(this.selectedPlayersContent.transform, false);
        players.Add(playerName, txt);
    }

    public void removePlayer(string mapName, string playerName)
    {
        Dictionary<string, Text> players = this.getPlayerList(mapName);
        if (players == null)
        {
            return;
        }
        Text txt;
        if (players.TryGetValue(playerName, out txt))
        {
            Debug.Log("remove " + playerName + " from map " + mapName);
            Destroy(txt.gameObject);
            players.Remove(playerName);
        }
    }
}
