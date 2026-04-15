using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;

    PlayerInput playerInput;
    InputAction escapeAction;

    bool isPaused = false;

    [Header("Survival Settings")]
    public float nightDuration = 300f; // 5 minutes to survive
    private float timeRemaining;
    
    [Header("Rescue Bot System")]
    public int maxRescues = 3;
    private int rescuesRemaining;
    public Transform homeBayRespawnPoint; // Where the rescue bot drops you

    private bool isGameOver = false;

    private void Awake()
    {
        timeRemaining = nightDuration;
        rescuesRemaining = maxRescues;


        playerInput = FindAnyObjectByType<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            escapeAction = playerInput.actions["Escape"];
        }

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        if (escapeAction != null)
        {
            escapeAction.Enable();
            escapeAction.performed += OnEscape;
        }
    }

    private void OnDisable()
    {
        if (escapeAction != null)
        {
            escapeAction.performed -= OnEscape;
            escapeAction.Disable();
        }
    }

    private void OnEscape(InputAction.CallbackContext ctx)
    {
        if (isGameOver) return; // Can't pause/unpause during game over

        // toggle pause state
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // disable player controls except escape
        if (playerInput != null && playerInput.actions != null)
        {
            DisableIfExists("Move");
            DisableIfExists("Look");
            DisableIfExists("Jump");
            DisableIfExists("Sprint");
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerInput != null && playerInput.actions != null)
        {
            EnableIfExists("Move");
            EnableIfExists("Look");
            EnableIfExists("Jump");
            EnableIfExists("Sprint");
        }
    }

    void DisableIfExists(string name)
    {
        var a = playerInput.actions.FindAction(name, throwIfNotFound: false);
        if (a != null) a.Disable();
    }

    void EnableIfExists(string name)
    {
        var a = playerInput.actions.FindAction(name, throwIfNotFound: false);
        if (a != null) a.Enable();
    }

    private void Update()
    {
        if (isPaused || isGameOver) return;

        // Night Timer Countdown
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            WinGame();
        }
    }

    public void TriggerRescue(PlayerStats player)
    {
        if (rescuesRemaining > 0)
        {
            rescuesRemaining--;
            Debug.Log($"Rescue Bot activated! Rescues left: {rescuesRemaining}");
            
            // Teleport player to Home Bay
            if (homeBayRespawnPoint != null)
            {
                // Disable CharacterController temporarily to teleport
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                
                player.transform.position = homeBayRespawnPoint.position;
                player.transform.rotation = homeBayRespawnPoint.rotation;
                
                if (cc != null) cc.enabled = true;
            }

            player.Revive();
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! No rescues left or time ran out.");
        // TODO: Show Game Over UI
    }

    void WinGame()
    {
        isGameOver = true;
        Debug.Log("YOU SURVIVED THE NIGHT!");
        // TODO: Show Victory UI
    }
}
