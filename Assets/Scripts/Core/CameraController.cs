using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSmoothTime = 0.2f;
    [SerializeField] private float aheadDistance = 2f;
    [SerializeField] private float verticalOffset = 0f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (player != null)
        {
            transform.position = GetTargetPosition();
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, GetTargetPosition(), ref velocity, cameraSmoothTime);
    }

    public void MoveToNewRoom(Transform newRoom)
    {
        float newPositionX = newRoom.position.x;
    }

    private Vector3 GetTargetPosition()
    {
        float facingDirection = Mathf.Sign(player.localScale.x);

        float targetX = player.position.x + (facingDirection * aheadDistance);
        float targetY = player.position.y + verticalOffset;

        return new Vector3(targetX, targetY, transform.position.z);
    }
}
