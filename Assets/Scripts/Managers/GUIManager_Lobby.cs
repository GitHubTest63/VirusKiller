using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Lobby : MonoBehaviour
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
    }

    public void showInvitationPanel(string playerName, string mapName)
    {
        this.invitationText.text = playerName + " has invited you to join the fight on the map : " + mapName;
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
        NetworkManager.Instance.sendAcceptInvitation();
        this.hideInvitationPanel();
    }

    public void declineInvitation()
    {
        NetworkManager.Instance.sendDeclineInvitation();
        this.hideInvitationPanel();
    }

}
