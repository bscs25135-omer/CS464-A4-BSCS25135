using System.Collections;
using UnityEngine;

// One script, several kinds of danger: spikes (restTime 0), a guard, a crusher (restTime > 0).
public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float knockTime = 0.25f;
    [SerializeField] float restTime = 0f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] GameObject warning;

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;
        Collider zone = GetComponent<Collider>();
        Renderer slab = GetComponent<Renderer>();
        while (true)
        {
            zone.enabled = false;
            if (slab != null) slab.enabled = false;
            yield return new WaitForSeconds(restTime);
            if (warning != null) warning.SetActive(true);
            yield return new WaitForSeconds(warnTime);
            if (warning != null) warning.SetActive(false);
            zone.enabled = true;
            if (slab != null) slab.enabled = true;
            yield return new WaitForSeconds(strikeTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // knockback only when a hit actually lands
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;

        Vector3 away = body.position - transform.position;
        away.y = 0f;
        Vector3 push = away.normalized * knockback;

        if (other.TryGetComponent(out PlayerController p)) p.Knockback(push, knockTime);
        else body.AddForce(push, ForceMode.Impulse);
    }
}
