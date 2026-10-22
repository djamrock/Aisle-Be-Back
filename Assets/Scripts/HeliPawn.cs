
using UnityEngine;

public class HeliPawn : Pawn
{
    private Rigidbody rb;


    // Force Movement
    public float thrustForce = 10f;
    public float moveForce = 1.5f;

    // Speeds
    public float pitchSpeed = 5f;
    public float rollSpeed = 5f;
    public float yawSpeed = 5f;


    public void Start()
    {
        // Get Rigidbody Component
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {

    }

    // Receive Inputs (coll, pitch, roll, yaw)
    public override void CollectiveUp()
    {
        rb.AddRelativeForce(Vector3.up * thrustForce);
    }

    public override void CollectiveDown()
    {
        rb.AddRelativeForce(Vector3.down * thrustForce);
    }

    public override void PitchBack()
    {
        transform.Rotate(-pitchSpeed * Time.deltaTime, 0, 0);
        rb.AddRelativeForce(Vector3.back * moveForce);
    }

    public override void PitchForward()
    {
        transform.Rotate(pitchSpeed * Time.deltaTime, 0, 0);
        rb.AddRelativeForce(Vector3.forward * moveForce);
    }

    public override void RollLeft()
    {
        transform.Rotate(0, 0, rollSpeed * Time.deltaTime);
        rb.AddRelativeForce(Vector3.left * moveForce);
    }

    public override void RollRight()
    {
        transform.Rotate(0, 0, -rollSpeed * Time.deltaTime);
        rb.AddRelativeForce(Vector3.right * moveForce);
    }

    public override void YawLeft()
    {
        transform.Rotate(0, -yawSpeed * Time.deltaTime, 0);
    }

    public override void YawRight()
    {
        transform.Rotate(0, yawSpeed * Time.deltaTime, 0);
    }

    public override void Shoot()
    {

    }
}

