using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 10f;
    [SerializeField] private PlayerInput playerInput;
    
    // Événements d'instance (conservés pour ne pas casser d'autres scripts potentiels)
    public event Action OnMoveFinished;
    [HideInInspector] public bool canGainScore = true;

    // Event Bus statiques pour décourpler avec ScoreManager !
    public static event Action GlobalOnMoveFinished;
    public static bool GlobalCanGainScore = true;
    
    private float movedDistance;
    private bool isMoving;

    private void Start()
    {
        spawner.OnVictory += MovePlayer;
        
        // Reset l'état statique quand le niveau (re)démarre
        GlobalCanGainScore = true; 
    }

    private void MovePlayer()
    {
        spawner.OnVictory -= MovePlayer;

        if (playerInput != null)
            playerInput.enabled = false;
        
        canGainScore = false;
        GlobalCanGainScore = false; // Mise à jour globale
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
            GlobalOnMoveFinished?.Invoke(); // Lancement global !
        }
    }
}
