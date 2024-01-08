using UnityEngine;

public class Depth : MonoBehaviour
{

    [SerializeField] private PlayerMove _b; //Точка которая находится в другом скрипте который висит на другом объекте
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float transparency = 0.5f;
    [SerializeField] private Collider2D playerCollider;
    private bool isPlayerBehind;

    private void Start()
    {
        _b = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
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


    public void Update()
    {
        Vector3 A = new(0f, -0.857f, 0f);
        Vector3 gA = transform.TransformPoint(A);

        Vector3 gB = _b.PointB();

        if (gA.y > gB.y)
        {
            Debug.Log("Точка A выше точки B");

            sr.sortingOrder = 1;
        }
        else
        {
            sr.sortingOrder = 3;
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
