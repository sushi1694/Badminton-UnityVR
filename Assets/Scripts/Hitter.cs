using UnityEngine;

public enum Hitter { None, Player, Opponent }

public class ShuttleHitterTracker : MonoBehaviour
{
    public Hitter lastHitter = Hitter.None;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerRacket"))
        {
            lastHitter = Hitter.Player;
        }
        else if (collision.gameObject.CompareTag("OpponentRacket"))
        {
            lastHitter = Hitter.Opponent;
        }
    }
}
