using UnityEngine;

public class FolfAI : MonoBehaviour
{
    public Transform targ;
 
    public Animator animator;

    public Vector3 posend;
    public Vector3 direction;
    public Vector3 firstPoint;

    public float reactDistance;
    public float thisDistance;
    public float maxDistance;
    public float moveSpeed;
    public float tmr;
    public float ft;

    void Start()
    {
        ft = 0.1f;
        tmr = 0;
        reactDistance = 5.0f;
        _ = GameObject.FindGameObjectWithTag("Player");
        firstPoint = gameObject.transform.position;
        if (maxDistance == 0)
            maxDistance = 0.2f;
    }

    void FixedUpdate()
    {
        thisDistance = Vector3.Distance(targ.transform.position, gameObject.transform.position);
        ft = Vector3.Distance(gameObject.transform.position, posend);

        if ((thisDistance >= maxDistance) && (thisDistance <= reactDistance))
        {
            posend = targ.transform.position;

            direction = posend - gameObject.transform.position;
            direction.Normalize();
            gameObject.transform.transform.Translate(moveSpeed * Time.deltaTime * direction);
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }



        if (thisDistance.ToString("f1") == maxDistance.ToString("f1"))
        {
            tmr += Time.deltaTime;
            if (tmr >= 1)
            {
                animator.SetBool("Punch", true);
                tmr = 0;
                targ.GetComponent<PlayerStats>()._currentHealth -= 10;
            }
            else
            {
                animator.SetBool("Punch", false);
            }
        }


        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }


        if ((thisDistance > reactDistance) && (ft > 0.1f))
        {
            Debug.DrawLine(gameObject.transform.position, firstPoint, Color.green);
            posend = firstPoint;
            direction = posend - gameObject.transform.position;
            direction.Normalize();
            gameObject.transform.Translate(moveSpeed * Time.deltaTime * direction);
            animator.SetBool("Run", true);
        }


        if (thisDistance <= 0.1f)
        {
            moveSpeed = 0;
            animator.SetBool("Run", false);
        }

        if (thisDistance > 0)
        {
            moveSpeed = 0.5f;
        }

        if (ft == 0)
        {
            moveSpeed = 0;
        }
    }
}