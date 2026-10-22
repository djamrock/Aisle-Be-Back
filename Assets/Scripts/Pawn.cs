using UnityEngine;


// This script is essentially a blueprint, explains what a Pawn must be capable of doing. Any different type of pawn would define what each of these mean in their own script.
// Local movement is based on the pawn orientation (up is forward), World movement is based on the game worlds fixed direction (up is positive Y)


public abstract class Pawn : MonoBehaviour
{
    public abstract void CollectiveUp();

    public abstract void CollectiveDown();

    public abstract void PitchForward();

    public abstract void PitchBack();

    public abstract void RollLeft();

    public abstract void RollRight();

    public abstract void YawLeft();

    public abstract void YawRight();


    public abstract void Shoot();


}
