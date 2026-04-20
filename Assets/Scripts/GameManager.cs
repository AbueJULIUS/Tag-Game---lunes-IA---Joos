using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Timer")]
    [SerializeField] private float gameTime = 180f; //3 mins
    private float currentTime;
    private bool gameFinished = false;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Finish Game")]
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private TextMeshProUGUI defeatText;

    [Header("Players")]
    List<ITagable> tagables = new List<ITagable>();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentTime = gameTime;
        UpdateTimerUI();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Tagable");

        foreach (var obj in objs)
        {
            if (obj.TryGetComponent<ITagable>(out var tagable))
            {
                tagables.Add(tagable);
            }
        }
    }

    void Update()
    {
        if (gameFinished) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            FinishGame();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        if (timerText != null)
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void FinishGame()
    {
        gameFinished = true;
        //Time.timeScale = 0;
        //defeatScreen.SetActive(true);
        CheckTagged();
    }
    void CheckTagged()
    {
     
            
                if(currentTime <= 0)
                {
                    defeatText.text = "gano";
                    SceneManager.LoadScene("VictoryScreen");
                }
                else
                {
                    defeatText.text = "no aguanto el tiempo";
                    SceneManager.LoadScene("DefeatScreen");
                }
                
            
        
    }
    public void Replay()
    {
        Scene game = SceneManager.GetActiveScene();
        SceneManager.LoadScene(game.name);
    }
    public void ExitGame()
    {
        Debug.Log("Leaving game...");
        Application.Quit();
    }
}
