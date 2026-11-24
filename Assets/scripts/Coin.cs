using UnityEngine;

public enum CoinType { Normal, Red, Blue }

public class Coin : MonoBehaviour
{
    [SerializeField] CoinType type = CoinType.Normal;
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 90f;
    [SerializeField] GameObject fxPrefab;
    [SerializeField] AudioClip clip;
    bool collected;

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

    public void ResetCoin()
    {
        collected = false;
        gameObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        if (fxPrefab != null)
        {
            Instantiate(fxPrefab, transform.position, Quaternion.identity);
        }

        if (SfxPlayer.Instance != null && clip != null)
        {
            SfxPlayer.Instance.Play(clip);
        }

        if (GameManager.Instance != null)
        {
            switch (type)
            {
                case CoinType.Normal: GameManager.Instance.AddScore(value); break;
                case CoinType.Red: GameManager.Instance.CollectRedCoin(other.transform.position); break;
                case CoinType.Blue: GameManager.Instance.CollectBlueCoin(); break;
            }
        }
        gameObject.SetActive(false);
    }
}
