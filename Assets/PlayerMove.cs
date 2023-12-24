using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector2 _moveVector;
    [SerializeField] private float _speed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    private void FixedUpdate()
    {
        _moveVector.x = Input.GetAxis("Horizontal");
        _moveVector.y = Input.GetAxis("Vertical");
        _rigidbody.MovePosition(_rigidbody.position + _speed * Time.deltaTime * _moveVector);

        _animator.SetBool("Run", _moveVector != Vector2.zero);

        _spriteRenderer.flipX = _moveVector.x < 0 || _moveVector.x <= 0 && _spriteRenderer.flipX;
    }
}