using UnityEngine;

public class CourtTrigger : MonoBehaviour
{
    [SerializeField] private bool isPlayerSide; // Check for PlayerSide, Uncheck for OpponentSide

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shuttlecock"))
        {
            Debug.Log(gameObject.name + " was hit by " + other.name);

            if (isPlayerSide)
            {
                Debug.Log("Point for Opponent!");
                GameManager.Instance.AddOpponentPoint();
            }
            else
            {
                Debug.Log("Point for Player!");
                GameManager.Instance.AddPlayerPoint();
            }
        }
    }
}
