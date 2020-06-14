using ABXY.AssetLink;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverCountdown : MonoBehaviour
{

    private float levelMaxPlayTime = R.Game_Time; //in seconds

    [SerializeField]
    private TextMeshProUGUI timerLabel;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onGameOver;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        levelMaxPlayTime -= Time.deltaTime;

        timerLabel.text = ((int)Math.Round(levelMaxPlayTime)).ToString();
        if (levelMaxPlayTime <= 0)
        {
            onGameOver?.Invoke();
            enabled = false;
        }


    }
}
