using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 500;
    [SerializeField] private Animator _animator;
    private Vector3 _input;

    private void Update()
    {
        GatherInput();
        Look();

        // Set the Speed parameter based on movement input magnitude
        float movementSpeed = _input.magnitude * _speed;
        _animator.SetFloat("speed", movementSpeed);
        Debug.Log("Animator Speed: " + movementSpeed); // Check if Speed is updating
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    //private void Look()
    //{
    //    if (_input == Vector3.zero) return;

    //    var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    //}

    private void Look()
    {
        if (_input == Vector3.zero) return;

        // Calculate the target rotation based on input
        var targetRotation = Quaternion.LookRotation(_input.ToIso(), Vector3.up);

        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime / 100);
    }

    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * _speed * Time.deltaTime);
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}