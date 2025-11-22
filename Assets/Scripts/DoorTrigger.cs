using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorOpenClose : XRBaseInteractable
{
    [Header("Door Settings")]
    [Tooltip("The transform of the door that rotates open/closed.")]
    public Transform doorHinge;

    [Tooltip("The angle to open the door (in degrees).")]
    public float openAngle = 90f;

    [Tooltip("How fast the door rotates.")]
    public float rotationSpeed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    protected override void Awake()
    {
        base.Awake();
        if (doorHinge == null)
            doorHinge = transform;

        closedRotation = doorHinge.localRotation;
        openRotation = Quaternion.Euler(doorHinge.localEulerAngles + new Vector3(0, openAngle, 0));
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

    private System.Collections.IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(doorHinge.localRotation, targetRotation) > 0.1f)
        {
            doorHinge.localRotation = Quaternion.Slerp(
                doorHinge.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }

        doorHinge.localRotation = targetRotation;
    }
}
