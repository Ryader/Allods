using UnityEngine;

public class FolfAI : MonoBehaviour
{
    [SerializeField] // Атрибут, который позволяет видеть приватные переменные в редакторе Unity
    private Transform targ; // Цель, за которой будет следовать AI

    [SerializeField]
    private Animator animator; // Аниматор для управления анимациями AI

    private Vector3 posend; // Конечная позиция, к которой AI будет двигаться
    private Vector3 direction; // Направление движения AI
    private Vector3 firstPoint; // Начальная позиция AI

    private float reactDistance; // Дистанция, на которой AI начинает реагировать на цель
    private float thisDistance; // Текущая дистанция до цели
    private float maxDistance; // Максимальная дистанция, на которой AI может быть от цели
    private float moveSpeed; // Скорость движения AI
    private float tmr; // Таймер для управления атаками AI
    private float ft; // Дистанция от AI до конечной позиции

    private Transform _transform; // Ссылка на компонент Transform этого объекта

    void Start()
    {
        _transform = transform; // Сохраняем ссылку на Transform
        ft = 0.1f;
        tmr = 0;
        reactDistance = 5.0f;
        _ = GameObject.FindGameObjectWithTag("Player"); // Находим игрока по тегу
        firstPoint = _transform.position; // Сохраняем начальную позицию
        if (maxDistance == 0)
            maxDistance = 0.2f;
    }

    void FixedUpdate()
    {
        thisDistance = Vector3.Distance(targ.position, _transform.position); // Вычисляем текущую дистанцию до цели
        ft = Vector3.Distance(_transform.position, posend); // Вычисляем дистанцию до конечной позиции

        // Если AI находится на расстоянии от цели между maxDistance и reactDistance
        if ((thisDistance >= maxDistance) && (thisDistance <= reactDistance))
        {
            MoveTowardsTarget(); // AI движется к цели
        }
        else
        {
            animator.SetBool("Run", false); // AI останавливается
        }

        // Если AI находится на расстоянии maxDistance от цели
        if (Mathf.Approximately(thisDistance, maxDistance))
        {
            PunchTarget(); // AI атакует цель
        }

        UpdateDirection(); // Обновляем направление AI

        // Если AI находится дальше reactDistance от цели и ближе к конечной позиции, чем 0.1f
        if ((thisDistance > reactDistance) && (ft > 0.1f))
        {
            ReturnToFirstPoint(); // AI возвращается к начальной позиции
        }

        if (thisDistance <= 0.1f)
        {
            StopMoving(); // AI останавливается
        }

        if (thisDistance > 0)
        {
            moveSpeed = 0.5f; // Устанавливаем скорость движения AI
        }

        if (ft == 0)
        {
            StopMoving(); // AI останавливается
        }
    }

    private void MoveTowardsTarget()
    {
        posend = targ.position; // Устанавливаем конечную позицию как позицию цели

        direction = posend - _transform.position; // Вычисляем направление движения
        direction.Normalize(); // Нормализуем направление
        _transform.Translate(moveSpeed * Time.deltaTime * direction); // Двигаем AI в направлении цели
        animator.SetBool("Run", true); // Включаем анимацию бега
    }

    private void PunchTarget()
    {
        tmr += Time.deltaTime; // Увеличиваем таймер
        if (tmr >= 1)
        {
            animator.SetBool("Punch", true); // Включаем анимацию удара
            tmr = 0; // Сбрасываем таймер
            targ.GetComponent<PlayerStats>()._currentHealth -= 10; // Наносим урон цели
        }
        else
        {
            animator.SetBool("Punch", false); // Выключаем анимацию удара
        }
    }

    private void UpdateDirection()
    {
        // Если AI движется влево, меняем масштаб на (-1, 1, 1)
        if (direction.x < 0)
        {
            _transform.localScale = new Vector3(-1, 1, 1);
        }
        // Если AI движется вправо, меняем масштаб на (1, 1, 1)
        else if (direction.x > 0)
        {
            _transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ReturnToFirstPoint()
    {
        Debug.DrawLine(_transform.position, firstPoint, Color.green); // Рисуем линию от AI до начальной позиции
        posend = firstPoint; // Устанавливаем конечную позицию как начальную позицию
        direction = posend - _transform.position; // Вычисляем направление движения
        direction.Normalize(); // Нормализуем направление
        _transform.Translate(moveSpeed * Time.deltaTime * direction); // Двигаем AI в направлении начальной позиции
        animator.SetBool("Run", true); // Включаем анимацию бега
    }

    private void StopMoving()
    {
        moveSpeed = 0; // Останавливаем AI
        animator.SetBool("Run", false); // Выключаем анимацию бега
    }
}
