
using UnityEngine;

public class HeliPawn : Pawn
{
    private Rigidbody rb;


    // Movement
    public float thrustForce = 10f;
    public float pitchSpeed = 5f;
    public float rollSpeed = 5f;
    public float yawSpeed = 5f;


    public override void Start()
    {
        // Get Rigidbody Component
        rb = GetComponent<Rigidbody>();
    }

    public override void Update()
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

    public override void PitchDown()
    {
        transform.Rotate(-pitchSpeed * Time.deltaTime, 0, 0);

    }

    public override void PitchUp()
    {
        transform.Rotate(pitchSpeed * Time.deltaTime, 0, 0);

    }

    public override void RollLeft()
    {
        transform.Rotate(0, 0, rollSpeed * Time.deltaTime);

    }

    public override void RollRight()
    {
        transform.Rotate(0, 0, -rollSpeed * Time.deltaTime);

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

