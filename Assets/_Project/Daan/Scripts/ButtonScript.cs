using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public ButtonManager buttonManager;
    private void Start()
    {
        buttonManager = FindAnyObjectByType<ButtonManager>();
    }

    public void ResumeGame()
    {
        buttonManager.gameManager.ResumeGame();
    }

    public void QuitGame()
    {
        Application.Quit();
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
