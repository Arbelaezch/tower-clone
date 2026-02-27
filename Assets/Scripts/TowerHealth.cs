using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        Debug.Log($"[Tower] HP: {CurrentHealth} / {maxHealth}");

        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Debug.Log("[Tower] Destroyed!");
        // Pause the game — swap this out for your own game-over screen later
        Time.timeScale = 0f;
    }

    // Draw HP above the tower in the Scene view for easy debugging
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        int display = Application.isPlaying ? CurrentHealth : maxHealth;
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f, $"HP: {display}");
#endif
    }
}
