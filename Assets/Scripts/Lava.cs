using UnityEngine;
using UnityEngine.UIElements;

public class Lava : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           gameManager.Victory();
        }
    }
}
