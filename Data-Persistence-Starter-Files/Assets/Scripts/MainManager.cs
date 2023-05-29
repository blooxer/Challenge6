using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager mainManager;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public TextMeshProUGUI BestScoreText;

    public GameObject GameOverText;

    private bool m_Started = false;
    public int score;

    private bool m_GameOver = false;

    public string BestScorePlayer ;

    private void Awake() {
    if (mainManager != null) {
        Destroy(gameObject);
        return;
    }
    mainManager = this;
}
    // Start is called before the first frame update
    void Start()
    {
        //* when starting a new game the best score and name are loaded and displayed
        LoadNameAndScore();
        BestScoreText.text = " Best Score: " + DataPersistant.Instance.bestScorePlayer +" :" + DataPersistant.Instance.bestScore + " pts";
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //* extra save just in case
                SaveNameAndScore();
                DataPersistant.Instance.SavePlayerNameAndScore();
            }
        }
    }

    void AddPoint(int point)
    {
        score += point;
        ScoreText.text = $"Score: " + score + " pts";

        //* Saving the highes score
        if (score > DataPersistant.Instance.bestScore)
        {
            DataPersistant.Instance.bestScorePlayer = DataPersistant.Instance.playerName;
            DataPersistant.Instance.bestScore += point;

            BestScoreText.text =
                "Best score: "
                + DataPersistant.Instance.playerName
                + " : "
                + DataPersistant.Instance.bestScore
                + " pts";
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        //* losing and resetting the game is also saved
        DataPersistant.Instance.SavePlayerNameAndScore();
        SaveNameAndScore();
    }
  
    [System.Serializable]
    class SaveData
    {
        public string BestScorePlayer ;
        public int score;
    }

    public void SaveNameAndScore()
    {
        SaveData data = new SaveData();
        data.BestScorePlayer = BestScorePlayer;
        data.score = score;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadNameAndScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            BestScorePlayer = data.BestScorePlayer;
            score = data.score;
        }
    }
}
