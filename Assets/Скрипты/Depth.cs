using UnityEngine;

public class Depth : MonoBehaviour
{

    [SerializeField] private PlayerMove _b; //Точка которая находится в другом объекте
    [SerializeField] private SpriteRenderer sr; //Рендер объекта
    [SerializeField] private float transparency = 0.5f; // Прозрачность объекта который перед персонажем
    [SerializeField] private Collider2D playerCollider; // Колизия персонажа
    private bool isPlayerBehind;

    private void Start()
    {
        _b = GameObject.FindWithTag("Player").GetComponent<PlayerMove>(); //Подключение компонентов
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision) // Работа триггеров для прозрачности
    {
        if (collision == playerCollider)
        {
            isPlayerBehind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == playerCollider)
        {
            isPlayerBehind = false;
        }
    }


    public void Update() // Расчет точек
    {
        Vector3 A = new(0f, -0.857f, 0f);
        Vector3 gA = transform.TransformPoint(A);

        Vector3 gB = _b.PointB();

        if (gA.y > gB.y) //Если точка А выше точки Б, то слой персонажа меняется на 1 
        {
            sr.sortingOrder = 2;
        }
        else
        {
            sr.sortingOrder = 4;
        }

        if (isPlayerBehind)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparency);
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }

    }
}
