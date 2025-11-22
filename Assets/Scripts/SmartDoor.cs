using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// XR door script that automatically creates and manages a hinge pivot.
/// Works for multiple doors without any manual hinge setup.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SmartDoorXR : XRBaseInteractable
{
    [Header("Door Settings")]
    [Tooltip("How far the door opens in degrees (positive = one way, negative = opposite).")]
    public float openAngle = 90f;

    [Tooltip("How fast the door rotates open/closed.")]
    public float rotationSpeed = 3f;

    private bool isOpen = false;
    private Transform hingePivot;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    protected override void Awake()
    {
        base.Awake();

        // Automatically set up the hinge pivot
        SetupHingePivot();
    }

    private void SetupHingePivot()
    {
        // Create a pivot if none exists
        hingePivot = new GameObject($"{gameObject.name}_Pivot").transform;
        hingePivot.position = GetComponent<Collider>().bounds.center; // Start near the door center
        hingePivot.rotation = transform.rotation;
        hingePivot.SetParent(transform.parent);

        // Adjust pivot to the side of the door depending on size (so it swings naturally)
        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 doorSize = bounds.size;

        // Choose hinge side automatically (left or right)
        Vector3 localRight = transform.right;
        hingePivot.position = bounds.center - localRight * (doorSize.x * 0.5f);

        // Re-parent door under pivot
        transform.SetParent(hingePivot);

        // Record default and open rotations
        closedRotation = hingePivot.localRotation;
        openRotation = Quaternion.Euler(hingePivot.localEulerAngles + new Vector3(0, openAngle, 0));
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(hingePivot.localRotation, targetRotation) > 0.1f)
        {
            hingePivot.localRotation = Quaternion.Slerp(
                hingePivot.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }

        hingePivot.localRotation = targetRotation;
    }
}

