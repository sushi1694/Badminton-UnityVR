using UnityEngine;
using System.Collections;

public class OpponentAttack : MonoBehaviour
{
    private static int initialPrecision = 2; // [1,3]
    private static int initialStamina = 1; //[1,3]
    private static int initialSpeed = 2;   //[1,3]
    private static int initialStrength = 1; //[1,3]

    [SerializeField] private GameObject centralPoint;
    [SerializeField] private GameObject opponent;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject shuttle;
    [SerializeField] private GameObject netLine;
    [SerializeField] private GameObject leftLine;
    [SerializeField] private GameObject rightLine;
    [SerializeField] private GameObject shortLine;
    [SerializeField] private GameObject centreLine;
    [SerializeField] private GameObject trackLeft;
    [SerializeField] private GameObject trackRight;
    [SerializeField] private GameObject trackCentre;
    [SerializeField] private GameObject trackFront;
    [SerializeField] private GameObject trackBack;
    [SerializeField] private GameObject CourtBase;
    [SerializeField] private GameObject backLinePlayer;
    [SerializeField] private GameObject backLineOpponent;
    [SerializeField] private GameObject PlayerRacket;
    [SerializeField] private AudioSource shuttleAudio;
    private float intervalLength;
    private float GRAVITY;
    private bool attacking;
    private float storedZ;
    private float animationRepetitions;
    private float targetX;
    private float targetZ;
    private int hits;
    private float precision;
    private float stamina;
    private float speed;
    private float strength;

    void Start()
    {
        GRAVITY = 9.8f;
        target.transform.position = new Vector3(centralPoint.transform.position.x, target.transform.position.y, centralPoint.transform.position.z);
        attacking = false;
        hits = 0;
        animationRepetitions = 5;
        targetX = PlayerRacket.transform.localPosition.x;
        targetZ = PlayerRacket.transform.localPosition.z;

        precision = initialPrecision;
        speed = initialSpeed;
        stamina = initialStamina;
        strength = initialStrength;
    }

    void Update()
    {
        // Move Target
        if (attacking)
        {
            target.transform.position = new Vector3(centralPoint.transform.position.x, target.transform.position.y, centralPoint.transform.position.z);
        }
        else if (shuttle.transform.position.z > netLine.transform.position.z)
        {
            moveTargetActual();
        }
        else if (PlayerRacket.GetComponent<Player>().getIsHit())
        {
            moveTargetPredicted();
        }

        // Move Opponent
        intervalLength = 0.03f * speed;

        if (Mathf.Abs(opponent.transform.position.x - target.transform.position.x) > (2 * intervalLength))
        {
            moveToTarget();
        }
        else if (Mathf.Abs(opponent.transform.position.z - target.transform.position.z) > (5 * intervalLength))
        {
            moveInZ();
        }

        // Attack
        attackPlayer();

        // Stop Player
        restrictPlayer();

        // Moderate Opponent
        initializeHits();
        moderateOpponet();
    }

    private void moveToTarget()
    {
        // Smoothly move towards the target
        opponent.transform.position = Vector3.MoveTowards(opponent.transform.position, target.transform.position, intervalLength);
    }

    private void moveInZ()
    {
        // Smoothly move in the Z direction
        float z = Mathf.MoveTowards(opponent.transform.position.z, target.transform.position.z, intervalLength);
        opponent.transform.position = new Vector3(opponent.transform.position.x, opponent.transform.position.y, z);
    }

    void moveTargetPredicted()
    {
        float velocityYZ = PlayerRacket.GetComponent<Player>().getVelecityYZ();
        Vector3 position = PlayerRacket.GetComponent<Player>().getPosition();
        float range = (2 * velocityYZ * velocityYZ) / GRAVITY;
        float predictedZ = position.z + range;
        float predictedX = PlayerRacket.GetComponent<Player>().getPosX();
        target.transform.position = new Vector3(predictedX, target.transform.position.y, predictedZ);
        checkBoundries();
    }

    void moveTargetActual()
    {
        target.transform.position = new Vector3(shuttle.transform.position.x, target.transform.position.y, shuttle.transform.position.z);
        checkBoundries();
    }

    void checkBoundries()
    {
        if (target.transform.position.z < netLine.transform.position.z)
        {
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, netLine.transform.position.z);
        }
        if (target.transform.position.x < rightLine.transform.position.x)
        {
            target.transform.position = new Vector3(rightLine.transform.position.x, target.transform.position.y, target.transform.position.z);
        }
        if (target.transform.position.x > leftLine.transform.position.x)
        {
            target.transform.position = new Vector3(leftLine.transform.position.x, target.transform.position.y, target.transform.position.z);
        }
        if (target.transform.position.z > backLineOpponent.transform.position.z)
        {
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, backLineOpponent.transform.position.z);
        }
    }

    void attackPlayer()
    {
        if (attacking)
        {
            attack();
            if (playerReceive())
            {
                attacking = false;
            }
        }
        else if (!attacking && opponentAttack())
        {
            attacking = true;
            hits++;
            (float, float) targetPoints = setTargetPoints();
            targetX = targetPoints.Item1;
            targetZ = targetPoints.Item2;
            StartCoroutine(racketAnimationSequence());
            shuttleAudio.Play();
        }
    }

    bool opponentAttack()
    {
        bool cond1 = Mathf.Abs(shuttle.transform.localPosition.x - target.transform.localPosition.x) <= 0;
        bool cond2 = Mathf.Abs(shuttle.transform.localPosition.z - target.transform.localPosition.z) <= 0;
        bool reached = cond1 && cond2;
        bool shuttleHeight = shuttle.transform.position.y <= target.transform.position.y;
        bool final = shuttleHeight && reached && PlayerRacket.GetComponent<Player>().getIsHit();
        storedZ = shuttle.transform.localPosition.z;
        return final;
    }

    bool playerReceive()
    {
        bool final = PlayerRacket.GetComponent<Player>().getIsLanded();
        return final;
    }

    void attack()
    {
        // Apply force to the shuttlecock using physics
        Vector3 direction = new Vector3(targetX - shuttle.transform.position.x, 0, targetZ - shuttle.transform.position.z).normalized;
        shuttle.GetComponent<Rigidbody>().AddForce(direction * strength * 10, ForceMode.Impulse);
    }

    IEnumerator racketAnimationSequence()
    {
        for (int i = 0; i < animationRepetitions; i++)
        {
            racketAnimationStart();
            yield return new WaitForSeconds(0.1f);
            racketAnimationEnd();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void racketAnimationStart()
    {
        opponent.transform.eulerAngles = opponent.transform.eulerAngles - new Vector3(1, 4, 0);
    }

    void racketAnimationEnd()
    {
        opponent.transform.eulerAngles = opponent.transform.eulerAngles + new Vector3(1, 4, 0);
    }

    void restrictPlayer()
    {
        if (PlayerRacket.transform.position.z > netLine.transform.position.z)
        {
            PlayerRacket.transform.position = new Vector3(PlayerRacket.transform.position.x, PlayerRacket.transform.position.y, netLine.transform.position.z);
        }
        if (PlayerRacket.transform.position.x < rightLine.transform.position.x)
        {
            PlayerRacket.transform.position = new Vector3(rightLine.transform.position.x, PlayerRacket.transform.position.y, PlayerRacket.transform.position.z);
        }
        if (PlayerRacket.transform.position.x > leftLine.transform.position.x)
        {
            PlayerRacket.transform.position = new Vector3(leftLine.transform.position.x, PlayerRacket.transform.position.y, PlayerRacket.transform.position.z);
        }
        if (PlayerRacket.transform.position.z < backLinePlayer.transform.position.z)
        {
            PlayerRacket.transform.position = new Vector3(PlayerRacket.transform.position.x, PlayerRacket.transform.position.y, backLinePlayer.transform.position.z);
        }
    }

    (float, float) setTargetPoints()
    {
        System.Random rnd = new System.Random();
        (float, float) targetQuad;
        int leftRight = (PlayerRacket.transform.position.x > centreLine.transform.position.x) ? 1 : 0; //0 == left, 1 == right;
        int frontBack = (PlayerRacket.transform.position.z < shortLine.transform.position.z) ? 1 : 0;//0 == front, 1 == back;
        if (precision == 1)
        {
            targetQuad = (leftRight, frontBack);
        }
        else if (precision == 2)
        {
            int num = rnd.Next(2);
            if (num == 0)
            {
                targetQuad = (((leftRight == 0) ? 1 : 0), frontBack);
            }
            else //num == 1
            {
                targetQuad = (leftRight, ((frontBack == 0) ? 1 : 0));
            }
        }
        else //target level 3
        {
            targetQuad = ((leftRight == 0) ? 1 : 0, (frontBack == 0) ? 1 : 0);
        }
        // Target X generating
        float upperBoundX;
        float lowerBoundX;
        if (targetQuad.Item1 == 0)
        {
            upperBoundX = trackCentre.transform.localPosition.x; //right for player
            lowerBoundX = trackLeft.transform.localPosition.x;
        }
        else // item1 == 1
        {
            upperBoundX = trackRight.transform.localPosition.x;
            lowerBoundX = trackCentre.transform.localPosition.x; //left for player
        }
        // Target Z generating
        float upperBoundZ;
        float lowerBoundZ;
        if (targetQuad.Item2 == 0)
        {
            upperBoundZ = trackFront.transform.localPosition.z;
            lowerBoundZ = trackCentre.transform.localPosition.z;
        }
        else // item2 == 1
        {
            upperBoundZ = trackCentre.transform.localPosition.z;
            lowerBoundZ = trackBack.transform.localPosition.z;
        }

        float x = (float)(lowerBoundX + ((upperBoundX - lowerBoundX) * rnd.NextDouble()));
        float z = (float)(lowerBoundZ + ((upperBoundZ - lowerBoundZ) * rnd.NextDouble()));
        return (x, z);
    }

    void initializeHits()
    {
        if (shuttle.transform.position.y <= CourtBase.transform.position.y + 0.6f)
        {
            hits = 0;
        }
    }

    void moderateOpponet()
    {
        if (stamina < 4 && hits % 10 == 0)
        {
            stamina++;
        }
        if (strength < 4 && hits % 10 == 0)
        {
            strength++;
        }
        if (precision < 4 && hits > 1 && hits % 2 == 0)
        {
            precision++;
        }
        float dampingFactor = 1.0f;
        if (hits > 2 && hits % (2 + stamina) == 0)
        {
            dampingFactor *= 0.8f;
        }
        speed *= dampingFactor;
        if (hits <= 1)
        {
            precision = initialPrecision;
            speed = initialSpeed;
            stamina = initialStamina;
            strength = initialStrength;
        }
    }

    public void setPrecision(float precision)
    {
        this.precision = precision;
    }
    public float getPrecision()
    {
        return precision;
    }
    public void setStamina(float stamina)
    {
        this.stamina = stamina;
    }
    public float getStamina()
    {
        return stamina;
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public float getSpeed()
    {
        return speed;
    }
    public void setStrength(float strength)
    {
        this.strength = strength;
    }
    public float getStrength()
    {
        return strength;
    }
}