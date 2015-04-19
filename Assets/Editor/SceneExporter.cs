using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneExporter : EditorWindow
{
    [MenuItem("Scene/Export to server data")]
    private static void Init()
    {
        SceneExporter exporter = EditorWindow.GetWindow<SceneExporter>();
        exporter.position = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 300, 50);
        exporter.Show();
    }
    private static string path = "Serverside Code/Game Code/Data/";
    private string fileName;

    private void export()
    {
        StreamWriter sw = File.CreateText(path + this.fileName + ".txt");

        this.exportStartPoint(sw);
        this.exportSpawners(sw);
        sw.Close();
    }

    private void exportStartPoint(StreamWriter sw)
    {
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        sw.Write("Start = ");
        if (start == null)
        {
            sw.WriteLine("position : " + 0.0 + "," + 0.0 + "," + 0.0);
        }
        else
        {
            sw.WriteLine("position : " + start.transform.position.x + "," + start.transform.position.y + "," + start.transform.position.z);
        }
    }

    private void exportSpawners(StreamWriter sw)
    {
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();
        foreach (Spawner s in spawners)
        {
            sw.Write("Spawner = ");
            if (s.GetType() == typeof(TimedSpawner))
            {
                sw.Write("type : timed");
            }
            else if (s.GetType() == typeof(TriggeredSpawner))
            {
                sw.Write("type : triggered");
            }
            sw.WriteLine(" ; position : " + s.transform.position.x + "," + s.transform.position.y + "," + s.transform.position.z);
        }
    }

    void OnGUI()
    {
        this.fileName = EditorGUILayout.TextField("File name", this.getCurrentSceneName());
        if (GUILayout.Button("Export"))
        {
            Debug.Log("Exporting ...");
            this.export();
            Debug.Log("Successfull export");
            this.Close();
        }
    }

    private string getCurrentSceneName()
    {
        string[] splits = EditorApplication.currentScene.Split(new char[] { '/' });
        string currentSceneName = splits[splits.Length - 1];
        currentSceneName = currentSceneName.Replace(".unity", "");
        return currentSceneName;
    }
}
