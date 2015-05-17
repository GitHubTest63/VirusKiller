using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Lobby : Photon.MonoBehaviour
{
    private static GUIManager_Lobby instance;
    public static GUIManager_Lobby Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private GameObject invitationPanel;
    private Text invitationText;

    void Start()
    {
        this.invitationPanel = this.transform.FindChild("Invitation").gameObject;
        this.invitationPanel.SetActive(false);
        this.invitationText = this.invitationPanel.transform.FindChild("Panel/Text").GetComponent<Text>();

        Text playerName = this.transform.FindChild("Player/Text").GetComponent<Text>();
        playerName.text = NetworkManager.Instance.playerName;
    }

    public void invite(string playerName, string mapName)
    {
        this.photonView.RPC("showInvitationPanel", PhotonTargets.AllBuffered, playerName, mapName);
    }

    [RPC]
    public void showInvitationPanel(string playerName, string mapName)
    {
        if (playerName == NetworkManager.Instance.playerName)
        {
            this.invitationText.text = "Ready to join the fight on the map : " + mapName + "?";
        }
        else
        {
            this.invitationText.text = playerName + " has invited you to join the fight on the map : " + mapName;
        }
        this.setInvitationPanelVisible(true);
    }

    private void setInvitationPanelVisible(bool visible)
    {
        this.invitationPanel.SetActive(visible);
    }

    public void hideInvitationPanel()
    {
        this.invitationPanel.SetActive(false);
    }

    public void acceptInvitation()
    {
        NetworkManager.Instance.acceptInvitation();
        this.hideInvitationPanel();
        MapManager.Instance.removePlayer(NetworkManager.Instance.playerName);
    }

    public void declineInvitation()
    {
        MapManager.Instance.removePlayer(NetworkManager.Instance.playerName);
        this.hideInvitationPanel();
    }

}
