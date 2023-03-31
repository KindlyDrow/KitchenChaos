using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public event EventHandler OnAnyPlayerSpawned;

    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnPickedSomth;
    public static void ResetStaticData()
    {
        OnPickedSomth = null;
    }
    public event EventHandler<OnSelectedCounterChangedEventArg> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArg : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] Transform _kitchenObjectHoldPoint;
    public Transform KitchenObjectHoldPoint { get { return _kitchenObjectHoldPoint; } }

    private KitchenObject _kitchenObject;
    public KitchenObject KitchenObject
    {
        get
        { 
            return _kitchenObject;
        }
        set
        {
            OnPickedSomth?.Invoke(this, EventArgs.Empty);
            _kitchenObject = value;
        }
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;

    private GameInput gameInput;
    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        gameInput = GameInput.Instanse;
        playerRigidbody = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if(!GameManager.Instance.IsGamePlaying())  return; 
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if ( selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) 
        {
            return; 
        }
        HandleInteraction();
        HandleMovement();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter seeableCounter))
            {
                if(seeableCounter != selectedCounter)
                {
                    SetSelectedCounter(seeableCounter);
                }
            } else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius,
            moveDir, out RaycastHit playerMoveRaycastHitOut, moveDistance);

        if (!canMove)
        {
            Vector3 reflectedVector3 = Vector3.Reflect(moveDir, playerMoveRaycastHitOut.normal).normalized;

            moveDir = reflectedVector3 + moveDir;
            moveDir.y = 0f;
            if (moveDir.magnitude > .1f)
            {
                moveDir = moveDir.normalized;
                canMove = true;

            }
        }
        if (canMove)
        {
            playerRigidbody.MovePosition(transform.position + moveDir.normalized * moveDistance);
            //transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArg { selectedCounter = selectedCounter });
    }
}
