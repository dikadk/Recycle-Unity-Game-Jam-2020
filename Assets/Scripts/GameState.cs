using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameState : MonoBehaviour
{

    public float currentSpeed = 0.05f;

    public int numItemsMissingFromBikes { get; private set; }


    private int score = 0;

    [SerializeField]
    GameObject gameOverScreen = null;

    [SerializeField]
    TextMeshProUGUI scoreLabel = null;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onFirstDifficultySpike;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onSecondDifficultySpike;

    // Start is called before the first frame update
    void Start()
    {
        scoreLabel.text = score.ToString() ;
        numItemsMissingFromBikes = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNewBikeMade()
    {
        score += 100;
        scoreLabel.text = score.ToString();
        currentSpeed += 0.0005f;

        if (score > 1000 && score < 2000)
        {
            if (numItemsMissingFromBikes != 2)
                onFirstDifficultySpike?.Invoke();
            numItemsMissingFromBikes = 2;
        }
        else if (score > 2000)
        {
            if (numItemsMissingFromBikes != 3)
                onSecondDifficultySpike?.Invoke();
            numItemsMissingFromBikes = 3;
        }
            
    }

    public void OnGameOver()
    {
        PlayerInput[] inputs = FindObjectsOfType<PlayerInput>();
        foreach(PlayerInput input in inputs)
        {
            input.gameObject.SetActive(false);
        }

        PlayerInputManager inputManager = FindObjectOfType<PlayerInputManager>();
        inputManager.gameObject.SetActive(false);

        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.gameObject.SetActive(false);

        Camera camera = FindObjectOfType<Camera>();
        camera.gameObject.SetActive(false);

        gameOverScreen.SetActive(true);
    }
}
