using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Vector2 _moveVector;
    private float _lastDirection = 1;

    [SerializeField] private Attack _layerAnim;
    [SerializeField] private float _speed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        _moveVector.x = Input.GetAxis("Horizontal");
        _moveVector.y = Input.GetAxis("Vertical");
        _rigidbody.MovePosition(_rigidbody.position + _speed * Time.deltaTime * _moveVector);

        _animator.SetBool(_layerAnim._layerWeight == 0 ? "Run" : "SwordRun", _moveVector != Vector2.zero);

        _lastDirection = _moveVector.x != 0 ? (_moveVector.x < 0 ? -1 : 1) : _lastDirection;
        transform.localScale = new Vector3(_lastDirection, 1, 1);
    }
}