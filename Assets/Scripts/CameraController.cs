using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Room Camera Settings")]
    private float currentPositionX;

    [Header("Follow Player Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSmoothTime = 0.2f;
    [SerializeField] private float aheadDistance = 2f;
    [SerializeField] private float verticalOffset = 0f;

    private Vector3 velocity = Vector3.zero;
    private float lookAhead;

    private void Start()
    {
        float startTargetX = player.position.x + (Mathf.Sign(player.localScale.x) * aheadDistance);
        float startTargetY = player.position.y + verticalOffset;
        transform.position = new (startTargetX, startTargetY, transform.position.z);
    }

    private void LateUpdate()
    {
        float targetX = player.position.x + (Mathf.Sign(player.localScale.x) * aheadDistance);
        float targetY = player.position.y + verticalOffset;
        Vector3 targetPosition = new (targetX, targetY, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSmoothTime);
    }

    public void MoveToNewRoom(Transform newRoom)
    {
        float newPositionX = newRoom.position.x;
        if (newPositionX != currentPositionX)
            currentPositionX = newPositionX;
    }
}
