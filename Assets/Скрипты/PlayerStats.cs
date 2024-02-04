using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
  
    [SerializeField] internal float _currentHealth = 100f;
    [SerializeField] internal Image _healthSlider;
    [SerializeField] private float _hp = 100f;

    void Start()
    {
        _currentHealth = _hp;
    }

    void Update()
    {
        _healthSlider.fillAmount = _currentHealth / _hp;
    }

    void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _healthSlider.fillAmount = _currentHealth / _hp;
    }
}