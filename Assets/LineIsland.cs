using UnityEngine;

public class LineIsland : MonoBehaviour
{

    [SerializeField] private Vector2[] redLinePoints; //����� ��� ������� �����
    [SerializeField] private Vector2[] blueLinePoints; //����� ��� ����� �����
    private PlayerMove _point; //����� ���������
    private Rigidbody2D _playerRb;
    private SpriteRenderer _sr;

    private void Start()
    {
        _point = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        _playerRb = _point.GetComponent<Rigidbody2D>();
        _sr = _point.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 point = _point.PointB(); // ����� ���������

        for (int i = 0; i < redLinePoints.Length - 1; i++)
        {
            Vector2 vector = redLinePoints[i + 1] - redLinePoints[i]; // ������ ����� ����� ������� ��������� 
            Vector2 pointToLine = point - redLinePoints[i]; // ������ �� ����� ������� ����� �� ����� ���������

            float t = Vector2.Dot(pointToLine, vector) / vector.sqrMagnitude; // ��������� ��������� t ��� ����� �� �����

            if (t >= 0 && t <= 1) // ���� t ��������� � ��������� �� 0 �� 1 => ����� ����� ���������� �� ������� �����
            {
                Vector2 projection = redLinePoints[i] + t * vector; // ��������� �������� ����� �� ������
                if ((projection - point).sqrMagnitude < 0.01f) // ���� �������� ������ � �������� �����, ��� ��������, ��� ����� ��������� �� �����
                {
                    Debug.Log("�������");
                    _playerRb.gravityScale = 10;
                }
            }

            Debug.DrawLine(redLinePoints[i], redLinePoints[i + 1], Color.red);
        }

        Vector2 pointB = _point.PointA();

        for (int i = 0; i < blueLinePoints.Length - 1; i++)
        {
            Vector2 vector = blueLinePoints[i + 1] - blueLinePoints[i]; // ������ ����� ����� ������� ��������� 
            Vector2 pointToLine = pointB - blueLinePoints[i]; // ������ �� ����� ������� ����� �� ����� ���������

            float t = Vector2.Dot(pointToLine, vector) / vector.sqrMagnitude; // ��������� ��������� t ��� ����� �� �����

            if (t >= 0 && t <= 1) // ���� t ��������� � ��������� �� 0 �� 1 => ����� ����� ���������� �� ������� �����
            {
                Vector2 projection = blueLinePoints[i] + t * vector; // ��������� �������� ����� �� ������
                if ((projection - pointB).sqrMagnitude < 0.01f) // ���� �������� ������ � �������� �����, ��� ��������, ��� ����� ��������� �� �����
                {
                    Debug.Log("�����");
                    _sr.sortingOrder = 0;
                }
            }

            Debug.DrawLine(blueLinePoints[i], blueLinePoints[i + 1], Color.blue);
        }
    }
}