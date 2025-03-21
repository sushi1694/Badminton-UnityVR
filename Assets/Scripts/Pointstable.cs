using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI opponentScoreText;
    public TextMeshProUGUI gameOverText;
    public GameObject gameOverPanel;

    private int playerScore = 0;
    private int opponentScore = 0;
    private bool isShuttleLanded = false;
    private bool scoreProcessed = false;
    private Coroutine scoringCoroutine;
    private GameObject shuttlecock;
    private bool gameEnded = false;

    private void Start()
    {
        shuttlecock = GameObject.FindWithTag("Shuttlecock");
        gameOverText.text = "";
        gameOverPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded || !other.gameObject.CompareTag("Shuttlecock"))
            return;

        if (gameObject.name == "PlayerAway")
        {
            playerScore++;
            UpdateScoreUI();
            CheckGameOver();
            return;
        }
        else if (gameObject.name == "OpponentAway")
        {
            opponentScore++;
            UpdateScoreUI();
            CheckGameOver();
            return;
        }

        if (!isShuttleLanded)
        {
            isShuttleLanded = true;
            scoreProcessed = false;
            scoringCoroutine = StartCoroutine(CheckIfStaysOnGround());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Shuttlecock"))
        {
            isShuttleLanded = false;
            if (scoringCoroutine != null)
            {
                StopCoroutine(scoringCoroutine);
            }
        }
    }

    IEnumerator CheckIfStaysOnGround()
    {
        yield return new WaitForSeconds(1f);

        if (isShuttleLanded && !scoreProcessed)
        {
            if (gameObject.name == "PlayerGround")
            {
                opponentScore++;
            }
            else if (gameObject.name == "OpponentGround")
            {
                playerScore++;
            }

            ScoreAndReset();
        }
    }

    void ScoreAndReset()
    {
        scoreProcessed = true;
        UpdateScoreUI();
        CheckGameOver();
        isShuttleLanded = false;
    }

    void UpdateScoreUI()
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }

    void CheckGameOver()
    {
        if (playerScore >= 5)
        {
            ShowGameOverScreen("Player Wins!");
        }
        else if (opponentScore >= 5)
        {
            ShowGameOverScreen("Opponent Wins!");
        }
    }

    void ShowGameOverScreen(string message)
    {
        gameEnded = true;
        gameOverText.text = message;
        gameOverPanel.SetActive(true);
        shuttlecock.GetComponent<Rigidbody>().velocity = Vector3.zero;
        shuttlecock.GetComponent<Rigidbody>().useGravity = false;

        Debug.Log("Game Over Screen Shown! Starting Transition...");
        StartCoroutine(TransitionToScene(2)); // Ensure this scene index is correct
    }


    IEnumerator TransitionToScene(int sceneIndex)
    {
        Debug.Log("Game Over! Transitioning to scene " + sceneIndex);
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        SceneManager.LoadScene(sceneIndex);
    }



}
