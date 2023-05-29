using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class DataPersistant : MonoBehaviour
{
    public static DataPersistant Instance;
    public TMP_InputField inputField;
    public string playerName;
    public string bestScorePlayer;
    public TextMeshProUGUI bestScoreText;
    public int bestScore;

    
    private void Awake()
{
    if (Instance != null)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
    LoadPlayerNameAndScore();
    

}
private void Start() {
    LoadPlayerNameAndScore();
    bestScoreText.text = "Best score: " + bestScorePlayer + " : " + bestScore + " pts"; 
    //* Shows when starting the game the last best score and name that was saved
}
    public void SetPlayerName()
    {
        playerName = inputField.text;
        SceneManager.LoadScene(1);
    }


    public void SavePlayerNameAndScore()
    {
        SaveData data = new SaveData();
        data.bestScorePlayer = bestScorePlayer;
        data.bestScore = bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", json);
    }

    public void LoadPlayerNameAndScore()
    {
        string path = Application.persistentDataPath + "/saveFile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestScorePlayer = data.bestScorePlayer;
            bestScore = data.bestScore;
        }
    }

    [System.Serializable]
    class SaveData
    {
        public int bestScore;
        public string bestScorePlayer;
    }
    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
        SavePlayerNameAndScore(); //* Saves the score and name in JSON format on exit
    }
}


