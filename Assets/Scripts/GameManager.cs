using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int playerScore = 0;
    private int opponentScore = 0;

    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text opponentScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Debug to confirm GameManager is active
        Debug.Log("GameManager Initialized");
    }

    public void AddPlayerPoint()
    {
        playerScore++;
        Debug.Log("Updated Player Score: " + playerScore);
        UpdateScoreUI();
    }

    public void AddOpponentPoint()
    {
        opponentScore++;
        Debug.Log("Updated Opponent Score: " + opponentScore);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
        Debug.Log("Score Updated: Player - " + playerScore + " | Opponent - " + opponentScore);
    }
}
