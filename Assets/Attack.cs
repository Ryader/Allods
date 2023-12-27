using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _punch;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("Punch", Input.GetKeyDown(KeyCode.Mouse0));
    }
}