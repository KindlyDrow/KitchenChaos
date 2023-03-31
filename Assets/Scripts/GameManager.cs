using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnStateChange;
    public event EventHandler OnPause;

    public static GameManager Instance { get; private set; }

    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    public State _state { get; private set; }
    [SerializeField] private float countdownToStartTimerMax  = 3f;
    public float countdownToStartTimer { get; private set; } = 3f;

    [SerializeField] public float gamePlayingTimerMax = 10f;
    public float gamePlayingTimer { get; private set; }
    public bool isPoused { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
        ChangeState(State.WaitingToStart);
        gamePlayingTimer = gamePlayingTimerMax;
    }

    private void Start()
    {
        GameInput.Instanse.OnESC += GameInput_OnESC;
        GameInput.Instanse.OnInteractAction += GameInput_OnInteractAction;

        //DEBUG ONLY
        ChangeState(State.CountdownToStart);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_state == State.WaitingToStart)  ChangeState(State.CountdownToStart);
    }

    private void GameInput_OnESC(object sender, EventArgs e)
    {
        if (_state == State.GamePlaying)
        {
            TogglePouse();
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case State.WaitingToStart:

                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    countdownToStartTimer = countdownToStartTimerMax;
                    ChangeState(State.GamePlaying);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    ChangeState(State.GameOver);
                }
                break;
            case State.GameOver:
                
                break;
        }
    }

    private void ChangeState(State state)
    {
        _state = state;
        OnStateChange?.Invoke(this, EventArgs.Empty);
        
    }

    public bool IsGamePlaying()
    {
        return _state == State.GamePlaying;
    }

    public bool IsGameCoundownToStart()
    {
        if (_state == State.CountdownToStart) return true; else return false;
    }

    public void TogglePouse()
    {
        isPoused = !isPoused;
        OnPause?.Invoke(this, EventArgs.Empty);
        if (isPoused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
    }

}
