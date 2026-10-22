using UnityEngine;
using UnityEngine.UI;

public enum ControlMode
{
    Advanced,
    Basic
}

// Turns one control set on and the other off.
// Put this on the helicopter object (the one with the Rigidbody).
public class ControlModeSwitcher : MonoBehaviour
{
    public HelicopterController advanced;   // physics-based controls
    public HeliController basic;            // Pawn/Controller controls
    public Rigidbody body;
    public Text modeLabel;                  // optional on-screen text
    public KeyCode switchKey = KeyCode.Tab;
    public ControlMode startMode = ControlMode.Basic;
    public bool gravityInBasicMode = false; // tick this to give basic mode gravity

    public float advancedDamping = 1f;
    public float basicDamping = 3f;

    public ControlMode CurrentMode { get; private set; }

    void Start()
    {
        ApplyMode(startMode, true);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            if (CurrentMode == ControlMode.Advanced)
                ApplyMode(ControlMode.Basic, false);
            else
                ApplyMode(ControlMode.Advanced, false);
        }
    }

    // Hook these two up to UI buttons (Button > OnClick).
    public void SetAdvanced() { ApplyMode(ControlMode.Advanced, false); }
    public void SetBasic() { ApplyMode(ControlMode.Basic, false); }

    void ApplyMode(ControlMode mode, bool firstTime)
    {
        CurrentMode = mode;
        bool isAdvanced = (mode == ControlMode.Advanced);

        advanced.enabled = isAdvanced;
        basic.enabled = !isAdvanced;

        // Start fresh so the helicopter doesn't carry speed or tilt across modes.
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.rotation = Quaternion.Euler(0f, body.rotation.eulerAngles.y, 0f);

        // Advanced always uses gravity. Basic follows the checkbox.
        body.useGravity = isAdvanced || gravityInBasicMode;
        body.linearDamping = isAdvanced ? advancedDamping : basicDamping;

        if (isAdvanced)
        {
            // Idle power at startup (rotors spin, helicopter stays on the ground).
            // Hover power when switching in mid-flight so it doesn't drop.
            advanced.EngineForce = firstTime ? 5f : 9.81f;
        }
        else
        {
            // Basic mode: only drives the rotor and sound visuals,
            // because the advanced lift code is off while that script is disabled.
            advanced.EngineForce = 10f;
        }

        if (modeLabel != null)
            modeLabel.text = "Controls: " + mode + "  (" + switchKey + " to switch)";
    }
}