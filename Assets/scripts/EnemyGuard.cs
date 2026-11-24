using UnityEngine;

// Patrol -> Chase -> Return.
// Chases inside chaseRange, gives up only outside the longer giveUpRange
// (so it can't flicker at one edge), then walks back to its waypoint.
public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    [SerializeField] float chaseSpeed = 3f;
    [SerializeField] float returnSpeed = 2f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float giveUpRange = 8f;
    [SerializeField] float arriveDistance = 0.1f;
    [SerializeField] float turnSpeed = 360f;

    EnemyPatrol patrol;
    Transform player;
    State state = State.Patrol;

    void Awake() => patrol = GetComponent<EnemyPatrol>();

    void Start()
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        if (pc != null) player = pc.transform;
    }

    void Update()
    {
        if (player == null) return;

        switch (state)
        {
            case State.Patrol:
                if (InRange(chaseRange)) SetState(State.Chase);
                break;

            case State.Chase:
                MoveToward(player.position, chaseSpeed);
                if (!InRange(giveUpRange)) SetState(State.Return);
                break;

            case State.Return:
            {
                if (InRange(chaseRange)) { SetState(State.Chase); break; }
                Vector3 home = patrol.CurrentTarget;
                MoveToward(home, returnSpeed);
                if (Vector3.Distance(transform.position, home) < arriveDistance)
                    SetState(State.Patrol);
                break;
            }
        }
    }

    void SetState(State s)
    {
        state = s;
        patrol.enabled = s == State.Patrol;
    }

    bool InRange(float range)
    {
        Vector3 d = player.position - transform.position;
        d.y = 0f;
        return d.magnitude < range;
    }

    void MoveToward(Vector3 target, float speed)
    {
        Vector3 t = new Vector3(target.x, transform.position.y, target.z);
        Vector3 toTarget = t - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, t, speed * Time.deltaTime);
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
    }
}
