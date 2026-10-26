using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform Player;

    [Header("Follow")]
    [SerializeField] Vector3 Offset = new Vector3(0f, 2f, -15f); // Z is the fixed distance from the player

    [Header("Options")]
    [SerializeField] bool FollowX = true;
    [SerializeField] bool FollowY = true;
    [SerializeField] float MinY = 0f;                            

    void LateUpdate()
    {
        if (Player == null) return;

        Vector3 target = transform.position;

        if (FollowX)
            target.x = Player.position.x + Offset.x;
        if (FollowY)
            target.y = Mathf.Max(Player.position.y + Offset.y, MinY);

        target.z = Player.position.z + Offset.z; // keeps the 2D side view distance

        transform.position = target;
    }
}