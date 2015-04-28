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
    }

    private Dictionary<Language, Dictionary<string, string>> data = new Dictionary<Language, Dictionary<string, string>>();
    public TextAsset localisationData;

    public Language currentLanguage = Language.EN;

    public Language CurrentLanguage
    {
        get { return this.currentLanguage; }
        set
        {
            Debug.Log("setter");
            this.currentLanguage = value;
            this.updateLanguage();
        }
    }

    void Start()
    {
        init();
        string line;
        if (!localisationData)
        {
            Debug.LogError("No localisation Data");
            this.enabled = false;
            return;
        }
        line = Encoding.ASCII.GetString(localisationData.bytes);
        string[] lines = localisationData.text.Split(new char[] { '\n' });
        for (int i = 0; i < lines.Length; i++)
        {
            line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            Debug.Log(line);
            parse(line);
        }
    }

    private void init()
    {
        languages = (Language[])Enum.GetValues(typeof(Language));
    }

    private void parse(string toParse)
    {
        string[] values = toParse.Split(separators);
        string key = values[0];
        Debug.Log("Key : " + key);
        for (int i = 1; i < values.Length; i++)
        {
            Language l = languages[i - 1];
            Debug.Log("Language : " + l);
            Dictionary<string, string> localisations = this.getOrCreate(languages[i - 1]);
            Debug.Log("Value : " + values[i]);
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
        Debug.Log("try to get key : " + key);
        Dictionary<string, string> values;
        if (this.data.TryGetValue(language, out values))
        {
            string value;
            if (values.TryGetValue(key, out value))
            {
                Debug.Log("localisation found : " + value);
                return value;
            }
            else
            {
                Debug.Log("no localisation for this key");
                return key;
            }
        }
        else
        {
            Debug.Log("no values for language : " + language);
            return key;
        }
    }

    public string get(string key)
    {
        return this.get(this.currentLanguage, key);
    }

    public void changeLanguage(Localisation.Language language)
    {
        this.currentLanguage = language;
        this.updateLanguage();
    }

    public void updateLanguage()
    {
        Localizator[] toLocalize = GameObject.FindObjectsOfType<Localizator>();
        foreach (Localizator l in toLocalize)
        {
            l.updateLocalisation();
        }
    }
}
