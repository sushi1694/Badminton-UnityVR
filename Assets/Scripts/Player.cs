using System;
using UnityEngine;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    //[SerializeField] private GameObject Racket;
    [SerializeField] private GameObject controller;
    [SerializeField] private ActionBasedController controllerAB;
    [SerializeField] private GameObject Shuttle;
    [SerializeField] private GameObject CourtBase;
    [SerializeField] private GameObject racketCentre;
    [SerializeField] private AudioSource shuttleAudio;
    [SerializeField] private float forceMag;
    private bool isHit;
    private bool isLanded;
    private Vector3 hitVelocity;
    private Vector3 hitPosition;
    private Vector3 shuttlePos;
    private Quaternion shutttleRot;
    private Vector3 storePos;

    private void Start()
    {
        isHit = false;
        Invoke("stick", 1.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shuttlecock"))
        {
            // Update last hitter to Player
            ShuttleHitterTracker tracker = Shuttle.GetComponent<ShuttleHitterTracker>();
            if (tracker != null)
            {
                tracker.lastHitter = Hitter.Player;
            }

            // Existing logic for shuttle hit
            Shuttle.transform.position = Vector3.zero;
            Shuttle.GetComponent<Rigidbody>().useGravity = false;
            Shuttle.GetComponent<Rigidbody>().velocity = Vector3.zero;
            shuttleAudio.Play();
            controllerAB.SendHapticImpulse(1, 0.5f);
            Invoke("hit", 0.1f);
        }
    }

    void stick()
    {
        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;
    }

    void hit()
    {
        Vector3 dir = hitVector();
        dir.Normalize();
        storePos = dir;
        Shuttle.GetComponent<stickToController>().enabled = false;
        Shuttle.GetComponent<Rigidbody>().mass = 0.5f;
        Shuttle.GetComponent<Rigidbody>().useGravity = true;
        Shuttle.transform.position = racketCentre.transform.position;
        Shuttle.GetComponent<Rigidbody>().AddForce((dir * forceMag));
        hitVelocity = Shuttle.GetComponent<Rigidbody>().velocity;
        isHit = true;
    }

    Vector3 hitVector()
    {
        float Ax = controller.transform.eulerAngles.x;
        float Ay = controller.transform.eulerAngles.y;
        float Az = controller.transform.eulerAngles.y; // Note: using y for both Ay and Az might be intentional

        Vector3 defaultVector = new Vector3(0.01f, 1, 1);
        Vector3 hitVector = defaultVector;

        if (Ax > 0 && Ax < 90)
        {
            // Add more force towards +ve z
            if (Az > 0 && Az < 180)
            {
                // Add more force towards +ve x
                hitVector = new Vector3(0.2f, 1, 1);
            }
            else if ((Az < 0 && Az > -180) || (Az > 180 && Az < 360))
            {
                // Add more force towards -ve x
                hitVector = new Vector3(-0.2f, 1, 1);
            }
            else
            {
                hitVector = defaultVector;
            }
        }
        else if ((Ax < 0 && Ax > -90) || (Ax > 270 && Ax < 360))
        {
            hitVector = defaultVector;
        }
        else
        {
            hitVector = defaultVector;
        }
        return hitVector;
    }

    private void Update()
    {
        if (isHit && Shuttle.transform.position.y <= CourtBase.transform.position.y)
        {
            shuttlePos = new Vector3(Shuttle.transform.position.x, CourtBase.transform.position.y + 0.01f, Shuttle.transform.position.z);
            isHit = false;
            isLanded = true;
            hitPosition = Shuttle.transform.position;
        }
        else if (isLanded)
        {
            Shuttle.transform.position = shuttlePos;
            Shuttle.GetComponent<Rigidbody>().useGravity = false;
            Shuttle.GetComponent<Rigidbody>().velocity = Vector3.zero;
            isLanded = false;
        }

        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;
    }

    public Vector3 getPosition()
    {
        return hitPosition;
    }

    public float getVelecityYZ()
    {
        return ((forceMag * storePos.y) / Shuttle.GetComponent<Rigidbody>().mass) * Time.fixedDeltaTime;
    }

    public float getPosX()
    {
        return storePos.x;
    }

    public bool getIsHit()
    {
        return isHit;
    }

    public bool getIsLanded()
    {
        return isLanded;
    }
}
