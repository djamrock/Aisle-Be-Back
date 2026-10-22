using UnityEngine;


// This script defines what a controller must be able to do, blueprint for things that control Pawns. 


public abstract class Controller : MonoBehaviour
{
    public Pawn pawn;

    public abstract void MakeDecisions();
}
