using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class Localisation : MonoBehaviour
{
    [Serializable]
    public enum Language { EN, FR }

    private static Language[] languages;
    private static char[] separators = new char[] { ';', '\t' };
    private static Localisation instance;
    public static Localisation Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private Dictionary<Language, Dictionary<string, string>> data = new Dictionary<Language, Dictionary<string, string>>();
    public TextAsset localisationData;

    private Language currentLanguage = Language.EN;

    void Start()
    {
        if (!localisationData)
        {
            Debug.LogError("No localisation Data");
            this.enabled = false;
            return;
        }
    }

    public void init()
    {
        languages = (Language[])Enum.GetValues(typeof(Language));
        string line;
        line = Encoding.ASCII.GetString(localisationData.bytes);
        string[] lines = localisationData.text.Split(new char[] { '\n' });
        for (int i = 0; i < lines.Length; i++)
        {
            line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            parse(line);
        }
    }

    private void parse(string toParse)
    {
        string[] values = toParse.Split(separators);
        string key = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            Language l = languages[i - 1];
            Dictionary<string, string> localisations = this.getOrCreate(l);
            localisations.Add(key, values[i]);
        }
    }

    private Dictionary<string, string> getOrCreate(Language language)
    {
        Dictionary<string, string> values;
        if (!this.data.TryGetValue(language, out values))
        {
            values = new Dictionary<string, string>();
            this.data.Add(language, values);
        }
        return values;
    }

    public string get(Language language, string key)
    {
        Dictionary<string, string> values;
        if (this.data.TryGetValue(language, out values))
        {
            string value;
            if (values.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return key;
            }
        }
        else
        {
            return key;
        }
    }

    public string get(string key)
    {
        return this.get(this.currentLanguage, key);
    }

    public void setLanguage(int languageIndex)
    {
        if (languageIndex >= Enum.GetValues(typeof(Language)).Length)
        {
            this.currentLanguage = Language.EN;
        }
        else
        {
            this.currentLanguage = (Language)languageIndex;
        }
        //this.updateLanguage();
    }

    /*public void updateLanguage()
    {
        Localizator[] toLocalize = GameObject.FindObjectsOfType<Localizator>();
        foreach (Localizator l in toLocalize)
        {
            l.updateLocalisation();
        }
    }*/
}
