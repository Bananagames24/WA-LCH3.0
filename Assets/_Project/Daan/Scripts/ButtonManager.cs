using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
}
