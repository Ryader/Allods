using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float speed = 1f;
    public Animator animator;
    public Transform punch1;
    public float punch1Radius;
    void Start()
    {


    }

    static GameObject NearTarget(Vector3 position, Collider2D[] array)
    {
        Collider2D current = null;
        float dist = Mathf.Infinity;

        foreach (Collider2D coll in array)
        {
            float curDist = Vector3.Distance(position, coll.transform.position);

            if (curDist < dist)
            {
                current = coll;
                dist = curDist;
            }
        }

        return current?.gameObject;
    }

    // point - точка контакта
    // radius - радиус поражения
    // layerMask - номер слоя, с которым будет взаимодействие
    // damage - наносимый урон
    // allTargets - должны-ли получить урон все цели, попавшие в зону поражения
    public static void Action(Vector2 point,
                              float radius,
                              int layerMask,
                              float damage,
                              bool allTargets)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, radius, 1 << layerMask);

        if (!allTargets)
        {
            GameObject obj = NearTarget(point, colliders);
            if (obj != null && obj.GetComponent<EnemyHP>())
            {
                obj.GetComponent<EnemyHP>().HP -= 10;
            }
            return;
        }

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<EnemyHP>())
            {
                hit.GetComponent<EnemyHP>().HP -= 10;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);


        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        transform.position = transform.position + movement * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Fight2D.Action(punch1.position, punch1Radius, 9, 15, false);
        }

    }

}
