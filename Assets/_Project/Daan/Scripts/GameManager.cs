using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;

    PlayerInput playerInput;
    InputAction escapeAction;

    bool isPaused = false;

    private void Awake()
    {
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

    public void OnEscape(InputAction.CallbackContext ctx)
    {
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

    void ResumeGame()
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
}
