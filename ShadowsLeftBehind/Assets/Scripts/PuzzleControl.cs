using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] pictures;

    [SerializeField]
    private GameObject WinText;

    public static bool youWin;

    [SerializeField] private GameObject LoseText;             
    [SerializeField] private TextMeshProUGUI TimerText;       
    [SerializeField] private float timeLimitSeconds = 45f;
    public static bool timeUp = false;                        
    private float _timeLeft;
    private bool _timerRunning = true;

    // (from your intro)
    [Header("Intro Message")]
    [SerializeField] private TextMeshProUGUI IntroText;       
    [SerializeField] private float introDurationSeconds = 20f;
    private float _introTimeLeft;

    [Header("End Button")]
    [SerializeField] private GameObject NextButton;           // assign your Button GameObject in Inspector

    void Start()
    {
        WinText.SetActive(false);
        youWin = false;

        if (LoseText != null) LoseText.SetActive(false);
        timeUp = false;
        _timeLeft = Mathf.Max(0f, timeLimitSeconds);
        UpdateTimerText();

        _introTimeLeft = Mathf.Max(0f, introDurationSeconds);
        if (IntroText != null) IntroText.gameObject.SetActive(true);

        if (NextButton != null) NextButton.SetActive(false);
    }

    void Update()
    {
        // intro countdown
        if (_introTimeLeft > 0f)
        {
            _introTimeLeft -= Time.deltaTime;
            if (_introTimeLeft <= 0f && IntroText != null)
            {
                IntroText.gameObject.SetActive(false);
            }
        }

        if (_timerRunning && !timeUp && !youWin)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0f)
            {
                //if lose, next scene is in dark
                if (GameManager.instance)
                    GameManager.instance.isNextRoomDark = true;

                _timeLeft = 0f;
                timeUp = true;               
                _timerRunning = false;
                if (LoseText != null) LoseText.SetActive(true);

                if (IntroText != null) IntroText.gameObject.SetActive(false);

                if (NextButton != null) NextButton.SetActive(true);
            }
            UpdateTimerText();
        }

        if (
            pictures[0].rotation.z == 0 &&
            pictures[1].rotation.z == 0 &&
            pictures[2].rotation.z == 0 &&
            pictures[3].rotation.z == 0 &&
            pictures[4].rotation.z == 0 &&
            pictures[5].rotation.z == 0 &&
            pictures[6].rotation.z == 0 &&
            pictures[7].rotation.z == 0 &&
            pictures[8].rotation.z == 0 &&
            pictures[9].rotation.z == 0 &&
            pictures[10].rotation.z == 0 &&
            pictures[11].rotation.z == 0 &&
            pictures[12].rotation.z == 0 &&
            pictures[13].rotation.z == 0 &&
            pictures[14].rotation.z == 0 &&
            pictures[15].rotation.z == 0 &&
            pictures[16].rotation.z == 0 &&
            pictures[17].rotation.z == 0 &&
            pictures[18].rotation.z == 0 &&
            pictures[19].rotation.z == 0 &&
            pictures[20].rotation.z == 0 &&
            pictures[21].rotation.z == 0 &&
            pictures[22].rotation.z == 0 &&
            pictures[23].rotation.z == 0 &&
            pictures[24].rotation.z == 0 &&
            pictures[25].rotation.z == 0 &&
            pictures[26].rotation.z == 0 &&
            pictures[27].rotation.z == 0 &&
            pictures[28].rotation.z == 0 &&
            pictures[29].rotation.z == 0 &&
            pictures[30].rotation.z == 0 &&
            pictures[31].rotation.z == 0 &&
            pictures[32].rotation.z == 0 &&
            pictures[33].rotation.z == 0 &&
            pictures[34].rotation.z == 0
        ) 
        {
            //if win, next scene is in light
            if (GameManager.instance)
                GameManager.instance.isNextRoomDark = false;

            youWin = true;
            WinText.SetActive(true);

            _timerRunning = false;

            if (IntroText != null) IntroText.gameObject.SetActive(false);

            if (NextButton != null) NextButton.SetActive(true);
        }
    }
    
    private void UpdateTimerText()
    {
        if (TimerText == null) return;
        int whole = Mathf.CeilToInt(_timeLeft);
        int m = whole / 60;
        int s = whole % 60;
        TimerText.text = $"{m:00}:{s:00}";
    }

    //I dont think we should use this
    //public void ClickNext()
    //{
    //    int idx = SceneManager.GetActiveScene().buildIndex;
    //    int total = SceneManager.sceneCountInBuildSettings;
    //    if (idx + 1 < total)
    //    {
    //        SceneManager.LoadScene(idx + 1);
    //    }
    //    else
    //    {
    //        // If there is no “next” scene, reload current (or handle however you like)
    //        SceneManager.LoadScene(idx);
    //    }
    //}
}
