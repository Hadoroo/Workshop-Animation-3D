using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] private float hitFlashDuration = 0.1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        StopAllCoroutines(); 
        StartCoroutine(DamagedEffect());
    }

    private IEnumerator DamagedEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }
}
