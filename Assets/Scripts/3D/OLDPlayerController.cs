//using UnityEngine;


//public class PlayerController : Controller
//{


//    public KeyCode moveForward;

//    public KeyCode moveBackward;

//    public KeyCode yawLeft;

//    public KeyCode yawRight;

//    public KeyCode rollLeft;

//    public KeyCode rollRight;

//    public KeyCode pitchUp;

//    public KeyCode pitchDown;

//    //Shoot
//    public KeyCode shootKey;

//    public override void MakeDecisions()  // This is all of the player movement
//    {
//        if (pawn == null)
//        {
//            return;
//        }

//        // Forward and backward movement
//        if (Input.GetKey(moveForward))
//        {
//            pawn.MoveForward();
//        }

//        if (Input.GetKey(moveBackward))
//        {
//            pawn.MoveBackward();
//        }

//        // Yaw: turn left and right
//        if (Input.GetKey(yawLeft))
//        {
//            pawn.YawLeft();
//        }

//        if (Input.GetKey(yawRight))
//        {
//            pawn.YawRight();
//        }

//        // Roll
//        if (Input.GetKey(rollLeft))
//        {
//            pawn.RollLeft();
//        }

//        if (Input.GetKey(rollRight))
//        {
//            pawn.RollRight();
//        }

//        // Pitch
//        if (Input.GetKey(pitchUp))
//        {
//            pawn.PitchUp();
//        }

//        if (Input.GetKey(pitchDown))
//        {
//            pawn.PitchDown();
//        }

//        // Shoot once when the shoot key is pressed
//        if (Input.GetKeyDown(shootKey))
//        {
//            pawn.Shoot();
//        }

//    }

//    public override void Start()
//    {
//        if (GameManager.instance != null)
//        {
//            GameManager.instance.playerController = this; // playerController is registering itself with the GameManager
//        }
//    }

//    public override void Update()
//    {
//        MakeDecisions();
//    }
//}

