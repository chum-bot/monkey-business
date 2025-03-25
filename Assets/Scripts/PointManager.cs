using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MBNamespace.MBVars;

public class PointManager : MonoBehaviour
{

    public static PointManager instance;

    public int score;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    Text comboText;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = $"Score: {score}";
        comboText.gameObject.SetActive(false);
    }

    public void Score(int addedScore)
    {
        if(addedScore > 0) comboCount++;
        else comboCount = 0;
        if (comboCount >= 2)
        {
            comboText.gameObject.SetActive (true);
            comboText.text = $"{comboCount} COMBO!!";
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
        score += addedScore;
        if (score < 0) score = 0; //no negative scoring
        scoreText.text = $"Score: {score}";
    }
}
