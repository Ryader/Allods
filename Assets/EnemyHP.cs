using UnityEngine;
public class EnemyHP : MonoBehaviour
{
    public float HP = 100;
    public void AddDamage(float damage)
    {
        HP += damage;
        if (HP <= 0)
        {
            HP = 0;
            gameObject.SetActive(false);
            Destroy(gameObject, 500.5f); // удаляем объект
        }
    }
}
