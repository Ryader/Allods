using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _punch;
    [SerializeField] private Menu _menu;
    public float damageAmount = 10f; // ���������� �����, ������� �� ������ �������
    internal float _layerWeight = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _layerWeight = _layerWeight == 1f ? 0f : 1f;
            _animator.SetLayerWeight(1, _layerWeight);
        }

        if (!_menu._menuPanel.activeSelf)
        {
            bool mouse0Down = Input.GetKeyDown(KeyCode.Mouse0);
            _animator.SetBool("Punch", mouse0Down && _layerWeight != 1);
            _animator.SetBool("PunchSword", mouse0Down && _layerWeight == 1);
            _animator.SetBool("PunchSword2", Input.GetKeyDown(KeyCode.Mouse1) && _layerWeight == 1);
        }
    }

    // ���� ����� ����� ����������, ����� ��� �������� ���������� � ���������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������, ����� �� ������������� ������ ��� "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // �������� ��������� EnemyHP �������������� �������
            EnemyHP enemyHP = collision.gameObject.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                // ������� ���� �����
                enemyHP.AddDamage(-damageAmount);
            }
        }
    }
}