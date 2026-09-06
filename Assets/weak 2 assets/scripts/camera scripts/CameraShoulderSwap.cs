using UnityEngine;
using Unity.Cinemachine;

public class CameraShoulderSwap : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private KeyCode swapKey = KeyCode.Q;
    [SerializeField] private float swapSpeed = 5f;

    private CinemachineThirdPersonFollow thirdPersonFollow;
    private Vector3 targetOffset;

    void Start()
    {
        if (virtualCamera != null)
        {
            thirdPersonFollow = virtualCamera.GetComponent<CinemachineThirdPersonFollow>();
            if (thirdPersonFollow != null)
            {
                targetOffset = thirdPersonFollow.ShoulderOffset;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(swapKey) && thirdPersonFollow != null)
        {
            targetOffset.x *= -1f;
        }

        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.ShoulderOffset = Vector3.Lerp(thirdPersonFollow.ShoulderOffset, targetOffset, Time.deltaTime * swapSpeed);
        }
    }
}