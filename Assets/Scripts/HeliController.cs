using UnityEngine;

public class HeliController : Controller

{
    // Collective
    public KeyCode collectiveUp;
    public KeyCode collectiveDown;
    // Pitch
    public KeyCode pitchUp;
    public KeyCode pitchDown;
    // Roll
    public KeyCode rollLeft;
    public KeyCode rollRight;
    // Yaw
    public KeyCode yawLeft;
    public KeyCode yawRight;
  
    //Shoot
    public KeyCode shootKey;


    // This is all of the player movement
    public override void MakeDecisions()  
    {
        if (pawn == null)
        {
            return;
        }

        // Forward and backward movement
        if (Input.GetKey(collectiveUp))
        {
            pawn.CollectiveUp();
        }

        if (Input.GetKey(collectiveDown))
        {
            pawn.CollectiveDown();
        }

        // Pitch
        if (Input.GetKey(pitchUp))
        {
            pawn.PitchUp();
        }

        if (Input.GetKey(pitchDown))
        {
            pawn.PitchDown();
        }

        // Roll
        if (Input.GetKey(rollLeft))
        {
            pawn.RollLeft();
        }

        if (Input.GetKey(rollRight))
        {
            pawn.RollRight();
        }

        // Yaw: turn left and right
        if (Input.GetKey(yawLeft))
        {
            pawn.YawLeft();
        }

        if (Input.GetKey(yawRight))
        {
            pawn.YawRight();
        }

        // Shoot once when the shoot key is pressed
        if (Input.GetKeyDown(shootKey))
        {
            pawn.Shoot();
        }
    }

    public override void Start()
    {
        // HeliController is registering itself with the GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.heliController = this; 
        }
    }

    public override void Update()
    {
        MakeDecisions();
    }
}
