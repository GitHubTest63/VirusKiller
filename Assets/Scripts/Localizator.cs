using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Localizator : MonoBehaviour
{
    public string key = "unknown";
    Text toLocalize;

    void Start()
    {
        this.toLocalize = this.GetComponent<Text>();
        this.updateLocalisation();
    }

    public void updateLocalisation()
    {
        this.toLocalize.text = Localisation.Instance.get(this.key);
    }
}
