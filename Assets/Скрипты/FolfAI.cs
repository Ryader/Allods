using UnityEngine;

public class FolfAI : MonoBehaviour
{
    [SerializeField] // �������, ������� ��������� ������ ��������� ���������� � ��������� Unity
    private Transform targ; // ����, �� ������� ����� ��������� AI

    [SerializeField]
    private Animator animator; // �������� ��� ���������� ���������� AI

    [SerializeField] private Vector3 posend; // �������� �������, � ������� AI ����� ���������
    [SerializeField] private Vector3 direction; // ����������� �������� AI
    [SerializeField] private Vector3 firstPoint; // ��������� ������� AI

    [SerializeField] private float reactDistance; // ���������, �� ������� AI �������� ����������� �� ����
    [SerializeField] private float thisDistance; // ������� ��������� �� ����
    [SerializeField] private float maxDistance; // ������������ ���������, �� ������� AI ����� ���� �� ����
    [SerializeField] private float moveSpeed; // �������� �������� AI
    [SerializeField] private float tmr; // ������ ��� ���������� ������� AI
    [SerializeField] private float ft; // ��������� �� AI �� �������� �������

    [SerializeField] private Transform _transform; // ������ �� ��������� Transform ����� �������

    void Start()
    {
        _transform = transform; // ��������� ������ �� Transform
        ft = 0.1f;
        tmr = 0;
        reactDistance = 5.0f;
        _ = GameObject.FindGameObjectWithTag("Player"); // ������� ������ �� ����
        firstPoint = _transform.position; // ��������� ��������� �������
        if (maxDistance == 0)
            maxDistance = 0.3f;
    }

    void FixedUpdate()
    {
        thisDistance = Vector3.Distance(targ.position, _transform.position); // ��������� ������� ��������� �� ����
        ft = Vector3.Distance(_transform.position, posend); // ��������� ��������� �� �������� �������

        // ���� AI ��������� �� ���������� �� ���� ����� maxDistance � reactDistance
        if ((thisDistance >= maxDistance) && (thisDistance <= reactDistance))
        {
            MoveTowardsTarget(); // AI �������� � ����
        }
        else
        {
            animator.SetBool("Run", false); // AI ���������������
        }

        // ���� AI ��������� �� ���������� maxDistance �� ����
        if (Mathf.Approximately(thisDistance, maxDistance))
        {
            PunchTarget(); // AI ������� ����
            Debug.Log("����");
        }

        UpdateDirection(); // ��������� ����������� AI

        // ���� AI ��������� ������ reactDistance �� ���� � ����� � �������� �������, ��� 0.1f
        if ((thisDistance > reactDistance) && (ft > 0.1f))
        {
            ReturnToFirstPoint(); // AI ������������ � ��������� �������
        }

        if (thisDistance <= 0.1f)
        {
            StopMoving(); // AI ���������������
        }

        if (thisDistance > 0)
        {
            moveSpeed = 0.5f; // ������������� �������� �������� AI
        }

        if (ft == 0)
        {
            StopMoving(); // AI ���������������
        }
    }

    private void MoveTowardsTarget()
    {
        posend = targ.position; // ������������� �������� ������� ��� ������� ����

        direction = posend - _transform.position; // ��������� ����������� ��������
        direction.Normalize(); // ����������� �����������
        _transform.Translate(moveSpeed * Time.deltaTime * direction); // ������� AI � ����������� ����
        animator.SetBool("Run", true); // �������� �������� ����
    }

    private void PunchTarget()
    {
        tmr += Time.deltaTime; // ����������� ������
        if (tmr >= 1)
        {
            animator.SetBool("Punch", true); // �������� �������� �����
            tmr = 0; // ���������� ������
            targ.GetComponent<PlayerStats>()._currentHealth -= 10; // ������� ���� ����
            Debug.Log("����");
        }
        else
        {
            animator.SetBool("Punch", false); // ��������� �������� �����
        }
    }

    private void UpdateDirection()
    {
        // ���� AI �������� �����, ������ ������� �� (-1, 1, 1)
        if (direction.x < 0)
        {
            _transform.localScale = new Vector3(-1, 1, 1);
        }
        // ���� AI �������� ������, ������ ������� �� (1, 1, 1)
        else if (direction.x > 0)
        {
            _transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ReturnToFirstPoint()
    {
        Debug.DrawLine(_transform.position, firstPoint, Color.green); // ������ ����� �� AI �� ��������� �������
        posend = firstPoint; // ������������� �������� ������� ��� ��������� �������
        direction = posend - _transform.position; // ��������� ����������� ��������
        direction.Normalize(); // ����������� �����������
        _transform.Translate(moveSpeed * Time.deltaTime * direction); // ������� AI � ����������� ��������� �������
        animator.SetBool("Run", true); // �������� �������� ����
    }

    private void StopMoving()
    {
        moveSpeed = 0; // ������������� AI
        animator.SetBool("Run", false); // ��������� �������� ����
    }
}
