using System;
using UnityEngine;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class NewPlayer : MonoBehaviour
{
    [SerializeField] private GameObject controller;
    [SerializeField] private ActionBasedController controllerAB;
    [SerializeField] private GameObject Shuttle;
    [SerializeField] private GameObject CourtBase;
    [SerializeField] private GameObject racketCentreFront; // Front side center
    [SerializeField] private GameObject racketCentreBack;  // Back side center
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
        if (other.gameObject.tag == "shuttle")
        {
            // Determine which side of the racket was hit
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            bool isFrontSide = IsFrontSideHit(collisionPoint);

            // Reset shuttlecock position to the appropriate center point
            Vector3 resetPosition = isFrontSide ? racketCentreFront.transform.position : racketCentreBack.transform.position;
            Shuttle.transform.position = resetPosition;

            // Disable gravity and reset velocity
            Shuttle.GetComponent<Rigidbody>().useGravity = false;
            Shuttle.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Play audio and haptic feedback
            shuttleAudio.Play();
            controllerAB.SendHapticImpulse(1, 0.5f);

            // Trigger hit logic after a short delay
            Invoke("hit", 0.1f);
        }
    }

    bool IsFrontSideHit(Vector3 collisionPoint)
    {
        // Determine if the hit was on the front or back side of the racket
        Vector3 racketForward = transform.forward; // Racket's forward direction
        Vector3 collisionDirection = (collisionPoint - transform.position).normalized;

        // Use dot product to check if the collision is on the front or back side
        float dotProduct = Vector3.Dot(racketForward, collisionDirection);
        return dotProduct > 0; // Front side if dot product is positive
    }

    void stick()
    {
        // Stick the racket to the controller's position and rotation
        gameObject.transform.position = controller.transform.position;
        gameObject.transform.rotation = controller.transform.rotation;
    }

    void hit()
    {
        // Calculate the hit direction based on the controller's rotation
        Vector3 dir = hitVector();
        dir.Normalize();
        storePos = dir;

        // Disable stickiness and apply force to the shuttlecock
        Shuttle.GetComponent<stickToController>().enabled = false;
        Shuttle.GetComponent<Rigidbody>().mass = 0.5f;
        Shuttle.GetComponent<Rigidbody>().useGravity = true;
        Shuttle.GetComponent<Rigidbody>().AddForce(dir * forceMag);

        // Store the hit velocity and set the hit flag
        hitVelocity = Shuttle.GetComponent<Rigidbody>().velocity;
        isHit = true;
    }

    Vector3 hitVector()
    {
        // Calculate the hit direction based on the controller's rotation
        float Ax = controller.transform.eulerAngles.x;
        float Ay = controller.transform.eulerAngles.y;
        float Az = controller.transform.eulerAngles.y;

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
            // Add more force towards -ve z and +ve y and none in x
        }
        else
        {
            hitVector = defaultVector;
        }
        return hitVector;
    }

    private void Update()
    {
        // Check if the shuttlecock has landed on the court
        if (isHit && Shuttle.transform.position.y <= CourtBase.transform.position.y)
        {
            shuttlePos = new Vector3(Shuttle.transform.position.x, CourtBase.transform.position.y + 0.01f, Shuttle.transform.position.z);
            isHit = false;
            isLanded = true;
            hitPosition = Shuttle.transform.position;
        }
        else if (isLanded)
        {
            // Reset the shuttlecock position and velocity when it lands
            Shuttle.transform.position = shuttlePos;
            Shuttle.GetComponent<Rigidbody>().useGravity = false;
            Shuttle.GetComponent<Rigidbody>().velocity = Vector3.zero;
            isLanded = false;
        }

        // Update the racket's position and rotation to match the controller
        gameObject.transform.position = controller.transform.position;
        gameObject.transform.rotation = controller.transform.rotation;
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