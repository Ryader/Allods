using UnityEngine;
public class Hp_Bar : MonoBehaviour
{
    public float curHP = 100;
    public GameObject healthObj;
    public float maxHealth = 1000;

    [System.Obsolete]
    private void Update()
    {
        healthObj.transform.localScale = new Vector3(curHP / 100, 1, 1);
        if (curHP >= 100)
        {
            curHP = 100;
        }
        if (curHP <= 0)
        {
            curHP = 100;
            Death();
        }
        void Death()
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }



    public void TakeDamage(float damage)
    {
        curHP -= damage;

    }

}
