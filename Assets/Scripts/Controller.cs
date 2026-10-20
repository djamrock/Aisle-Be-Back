using UnityEngine;


// This script defines what a controller must be able to do, blueprint for things that control Pawns. 


public abstract class Controller : MonoBehaviour
{
    public Pawn pawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void Start();

    // Update is called once per frame
    public abstract void Update();

    public abstract void MakeDecisions();
}
