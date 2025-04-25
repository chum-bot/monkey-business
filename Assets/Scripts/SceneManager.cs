using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MBNamespace.MBVars;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] //time in seconds
    public float maxTime;

    [SerializeField]
    public Image timer;

    [SerializeField]
    public TextMeshProUGUI timerText;

    private float seconds;

    [SerializeField]
    public TextMeshProUGUI pressR;

    public static SceneHandler instance;

    [SerializeField]
    public List<Scene> scenes;

    public GAMESTATE gameState { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        seconds = maxTime;
        gameState = GAMESTATE.game;
        pressR.enabled = false;

        //play.onClick.AddListener(PlayClick);
        //quit.onClick.AddListener(QuitClick);
        //SceneManager.LoadSceneAsync("main-menu", LoadSceneMode.Additive);
        //SceneManager.LoadSceneAsync("game-over", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GAMESTATE.menu)
        {

        }
        if (gameState == GAMESTATE.game)
        {
            timer.fillAmount = seconds / maxTime;
            if (seconds <= 0)
            {
                gameState = GAMESTATE.gameover;
                pressR.enabled = true;
            }
            seconds -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(seconds).ToString();
        }
        if (gameState == GAMESTATE.gameover)
        {
            if(Input.GetKeyUp(KeyCode.R)) 
            {
                seconds = maxTime;
                PointManager.instance.score = 0;
                comboCount = 0;
                PointManager.instance.scoreText.text = $"Score: 0";
                PointManager.instance.comboText.text = "";
                pressR.enabled = false;
                gameState = GAMESTATE.game;
            }
        }
    }

    void PlayClick()
    {
        gameState = GAMESTATE.game;
        SceneManager.LoadScene("monkey-test");
    }

    void QuitClick()
    {
        Application.Quit();
    }
    
}
