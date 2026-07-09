using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Room previousRoom;
    [SerializeField] private Room nextRoom;
    [SerializeField] private CameraController cameraController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.position.x < transform.position.x)
            {
                cameraController.MoveToNewRoom(nextRoom.transform);
                nextRoom.ActivateRoom(true);
                previousRoom.ActivateRoom(false);
            }
            else
            {
                cameraController.MoveToNewRoom(previousRoom.transform);
                previousRoom.ActivateRoom(true); 
                nextRoom.ActivateRoom(false);
            }
        }
    }
}
