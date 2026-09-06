using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float sensitivity = 1.5f;
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;

    private float targetYaw;
    private float targetPitch;

    private void Start()
    {
        if (cameraRoot != null)
        {
            targetYaw = cameraRoot.eulerAngles.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (cameraRoot == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        targetYaw += mouseX * sensitivity;
        targetPitch -= mouseY * sensitivity;

        targetYaw = ClampAngle(targetYaw, float.MinValue, float.MaxValue);
        targetPitch = ClampAngle(targetPitch, bottomClamp, topClamp);

        cameraRoot.rotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}