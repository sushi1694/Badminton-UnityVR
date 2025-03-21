using UnityEngine;
using TMPro;

public class ShuttlecockCollision : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI opponentScoreText;

    private int playerScore = 0;
    private int opponentScore = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSide"))
        {
            // Opponent gets a point when shuttlecock lands on player's side
            opponentScore++;
            UpdateScore();
        }
        else if (other.CompareTag("OpponentSide"))
        {
            // Player gets a point when shuttlecock lands on opponent's side
            playerScore++;
            UpdateScore();
        }
    }

    void UpdateScore()
    {
        playerScoreText.text = "Player: " + playerScore;
        opponentScoreText.text = "Opponent: " + opponentScore;
    }
}
