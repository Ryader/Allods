using System.Collections;
using UnityEngine;

internal class Menu : MonoBehaviour
{
    [SerializeField] private GameObject Scroll; // Основной свиток

    [SerializeField] private GameObject Panel;  // Панель с кнопками меню
    [SerializeField] private GameObject Panel2; // Панель настройки
    [SerializeField] private GameObject Panel3; // Панель авторы
    [SerializeField] private GameObject Panel4; // Панель сохранений
    
    public Animator animator;





    public void SavePanel()
    {
        Panel.SetActive(false);
        Panel4.SetActive(true);
       
    }

    public void SaveExit()
    {
        Panel.SetActive(true);
        Panel4.SetActive(false);
       
    }


    public void Option()
    {
        Panel.SetActive(false);
        Panel2.SetActive(true);
       
    }

    public void OptionExit()
    {
        Panel.SetActive(true);
        Panel2.SetActive(false);
        
    }


    public void Developers()
    {
        Panel.SetActive(false);
        Panel3.SetActive(true);

        animator.SetBool("Replay", false);
        animator.SetBool("author", true);
       

    }

    public void DevelopersExit()
    {
        Panel.SetActive(true);
        Panel3.SetActive(false);

        animator.SetBool("author", false);
        animator.SetBool("Replay", true);
       
    }

    // Выход из игры
    public void Quit()
    {
        Application.Quit();
    }
}