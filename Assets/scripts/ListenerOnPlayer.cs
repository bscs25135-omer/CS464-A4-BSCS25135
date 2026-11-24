using UnityEngine;

public class ListenerOnPlayer : MonoBehaviour
{
    [SerializeField] Transform target;

    void Awake()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
