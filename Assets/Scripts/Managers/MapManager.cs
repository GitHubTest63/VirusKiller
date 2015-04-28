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
    private Dictionary<string, List<string>> selectedPlayers = new Dictionary<string, List<string>>();

    public List<Map> availablesMaps;
    private int currentMapIndex = -1;
    private Image mapIcon;
    private Text mapDesc;
    private Text mapName;
    private Text chooseButtonText;

    private string selectedMapChooseButtonText = "Remove";
    private string defaultMapChooseButtonText = "Choose";

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

    public void selectCurrentMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        this.selectMap(m.name);
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
        this.updateChooseButtonText();
        this.updatePlayers();
    }

    private void updateChooseButtonText()
    {
        List<string> players = this.getOrCreatePlayerList(this.mapName.text);
        if (players.Contains(NetworkManager.Instance.playerName))
        {
            this.chooseButtonText.text = this.selectedMapChooseButtonText;
        }
        else
        {
            this.chooseButtonText.text = this.defaultMapChooseButtonText;
        }
    }

    public void playSelectedMap()
    {
        Map m = this.availablesMaps[this.currentMapIndex];
        NetworkManager.Instance.sendPlayMap(m.name);
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

    public void addPlayer(string mapName, string playerName)
    {
        List<string> players = this.getOrCreatePlayerList(mapName);
        if (players.Contains(playerName))
        {
            this.removePlayer(mapName, playerName);
        }
        else
        {
            players.Add(playerName);
        }
        if (mapName.Equals(this.mapName.text))
        {
            updatePlayers();
        }
        this.updateChooseButtonText();
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

    public void removePlayer(string mapName, string playerName)
    {
        List<string> players = this.getPlayerList(mapName);
        if (players == null)
        {
            return;
        }
        this.updatePlayers();
    }
}
