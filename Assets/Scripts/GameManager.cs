using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event EventHandler OnStateChange;
    public event EventHandler OnPause;
    public event EventHandler OnLocalPlayerReadyChanged;

    public static GameManager Instance { get; private set; }

    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        Pause,
        GameOver,
    }

    public State _state { get; private set; }

    [SerializeField] private float countdownToStartTimerMax  = 3f;
    public float countdownToStartTimer { get; private set; } = 3f;
    [SerializeField] public float gamePlayingTimerMax = 10f;
    public float gamePlayingTimer { get; private set; }
    public bool isPoused { get; private set; } = false;
    private bool isLocalPlayerReady = false;
    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();

    private void Awake()
    {
        Instance = this;
        ChangeStateServerRpc(State.WaitingToStart);
        gamePlayingTimer = gamePlayingTimerMax;
    }

    private void Start()
    {
        GameInput.Instanse.OnESC += GameInput_OnESC;
        GameInput.Instanse.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_state == State.WaitingToStart) 
        { 
            isLocalPlayerReady = !isLocalPlayerReady;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc(isLocalPlayerReady);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(bool playerState, ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = playerState;

        bool allPlayersReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allPlayersReady = false;
                break;
            }
        }

        Debug.Log(allPlayersReady);
        if (allPlayersReady)
        {
            ChangeStateServerRpc(State.CountdownToStart);
        }
    }

    private void GameInput_OnESC(object sender, EventArgs e)
    {
        if (_state == State.GamePlaying || _state == State.Pause)
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
                    
                    ChangeStateServerRpc(State.GamePlaying);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    ChangeStateServerRpc(State.GameOver);
                }
                break;
            case State.Pause:
                
                break;
            case State.GameOver:
                
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeStateServerRpc(State state)
    {
        bool wannaPoused = state == State.Pause;
        ChangeStateClientRpc(state, gamePlayingTimer, wannaPoused);
    }

    [ClientRpc]
    private void ChangeStateClientRpc(State state, float timer, bool serverPoused)
    {
        if (isPoused != serverPoused) 
        {
            isPoused = serverPoused;

            OnPause?.Invoke(this, EventArgs.Empty);
            if (isPoused) Time.timeScale = 0f; else Time.timeScale = 1f;
        }

        countdownToStartTimer = countdownToStartTimerMax;
        gamePlayingTimer = timer;

        _state = state;
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
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
        if (!isPoused)
        {
            ChangeStateServerRpc(State.Pause);
        }
        else
        {
            ChangeStateServerRpc(State.CountdownToStart);
        }
        
    }

}
