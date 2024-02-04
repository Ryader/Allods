using UnityEngine;

public class LineIsland : MonoBehaviour
{

    [SerializeField] private Vector2[] redLinePoints; //Точки для красной линии
    [SerializeField] private Vector2[] blueLinePoints; //Точки для синей линии
    private PlayerMove _point; //Точка персонажа
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
        Vector2 point = _point.PointB(); // Точка персонажа

        for (int i = 0; i < redLinePoints.Length - 1; i++)
        {
            Vector2 vector = redLinePoints[i + 1] - redLinePoints[i]; // Вектор между двумя точками координат 
            Vector2 pointToLine = point - redLinePoints[i]; // Вектор от точки красной линии до точки персонажа

            float t = Vector2.Dot(pointToLine, vector) / vector.sqrMagnitude; // Получение параметра t для точки на линии

            if (t >= 0 && t <= 1) // Если t находится в диапазоне от 0 до 1 => точка может находиться на отрезке линии
            {
                Vector2 projection = redLinePoints[i] + t * vector; // Вычисляем проекцию точки на линиюы
                if ((projection - point).sqrMagnitude < 0.01f) // Если проекция близка к исходной точке, это означает, что точка находится на линии
                {
                    Debug.Log("Красный");
                    _playerRb.gravityScale = 10;
                }
            }

            Debug.DrawLine(redLinePoints[i], redLinePoints[i + 1], Color.red);
        }

        Vector2 pointB = _point.PointA();

        for (int i = 0; i < blueLinePoints.Length - 1; i++)
        {
            Vector2 vector = blueLinePoints[i + 1] - blueLinePoints[i]; // Вектор между двумя точками координат 
            Vector2 pointToLine = pointB - blueLinePoints[i]; // Вектор от точки красной линии до точки персонажа

            float t = Vector2.Dot(pointToLine, vector) / vector.sqrMagnitude; // Получение параметра t для точки на линии

            if (t >= 0 && t <= 1) // Если t находится в диапазоне от 0 до 1 => точка может находиться на отрезке линии
            {
                Vector2 projection = blueLinePoints[i] + t * vector; // Вычисляем проекцию точки на линиюы
                if ((projection - pointB).sqrMagnitude < 0.01f) // Если проекция близка к исходной точке, это означает, что точка находится на линии
                {
                    Debug.Log("Синий");
                    _sr.sortingOrder = 0;
                }
            }

            Debug.DrawLine(blueLinePoints[i], blueLinePoints[i + 1], Color.blue);
        }
    }
}