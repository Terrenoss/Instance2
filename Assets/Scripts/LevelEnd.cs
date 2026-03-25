using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 2f;
    public event Action OnMoveFinished;
    [HideInInspector] public bool canGainScore = true;
    [SerializeField] private float distance = 10f;
    [SerializeField] private PlayerInput playerInput;
    
    
    private float movedDistance;
    private float time;
    private bool isMoving;

    private void Start()
    {
        spawner.OnVictory += MovePlayer;
    }

    private void MovePlayer()
    {
        spawner.OnVictory -= MovePlayer;

        if (playerInput != null)
            playerInput.enabled = false;
        
        canGainScore = false;
        movedDistance = 0f;
        isMoving = true;
    }
    
    private void Update()
    {
        if (!isMoving) return;

        float step = speed * Time.deltaTime;
        player.position += player.forward * step;

        movedDistance += step;

        if (movedDistance >= distance)
        {
            isMoving = false;
            OnMoveFinished?.Invoke();
        }
    }
}
