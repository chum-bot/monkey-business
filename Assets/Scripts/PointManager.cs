using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MBNamespace.MBVars;

public class PointManager : MonoBehaviour
{

    public static PointManager instance;

    public int score;

    private float colorPulseTime;

    private float scoreCD;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    Text comboText;

    [SerializeField]
    Text addedScoreText;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreCD = 0.3f;
        colorPulseTime = 0.5f;
        addedScoreText.text = "";
        scoreText.text = $"Score: {score}";
        comboText.gameObject.SetActive(false);
    }

    public void Score(int addedScore)
    {
        if(scoreCD <= 0)
        {
            score += addedScore;
            if (score < 0) score = 0; //no negative scoring

            scoreText.text = $"Score: {score}";
            if (addedScore > 50)
            {
                addedScoreText.text = $"+{addedScore}!!";
                StartCoroutine(TextPulse(new Color(250f / 255f, 215f / 255f, 36f / 255f)));
                comboCount++;
            }
            else if (addedScore > 0)
            {
                addedScoreText.text = $"+{addedScore}";
                StartCoroutine(TextPulse(Color.green));
                comboCount++;
            }
            else if (addedScore < 0)
            {
                addedScoreText.text = $"{addedScore}";
                StartCoroutine(TextPulse(Color.red));
                comboCount = 0;

            }
            if (comboCount >= 2)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"{comboCount} COMBO!!";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
        scoreCD = 0.2f;
    }

    private void Update()
    {
        scoreCD -= Time.deltaTime;
    }
    public IEnumerator TextPulse(Color pulseColor)
    {
        float t = colorPulseTime;
        while (t > 0)
        {
            if(pulseColor != Color.red)
            {
                addedScoreText.CrossFadeAlpha(1, 0f, true);
                addedScoreText.CrossFadeAlpha(0, 0.3f, false);
            }
            addedScoreText.color = Color.Lerp(Color.black, pulseColor, t / colorPulseTime);
            scoreText.color = Color.Lerp(Color.black, pulseColor, t / colorPulseTime);

            t -= Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
