using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterController : MonoBehaviour
{

    // public float speed = 20f;
	private float upForce = 200f, force = 15f;

	private Rigidbody rb;
	private PlayerInput playerInput;
	private Vector2 input;
	private Animator animator;
	[SerializeField] private BoxCollider interactionCollider;
	private float rotationSpeed = 10f, moveSpeed = 2;
	

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		animator = GetComponent<Animator>();
    }

	void Update ()
	{
		input = playerInput.actions["Move"].ReadValue<Vector2>();


	}

    void FixedUpdate()
	{
		Vector3 moveDir = new (input.x, 0f, input.y);

		bool isMoving = moveDir != Vector3.zero;
		animator.SetBool("isWalking", isMoving);

		// movement with acceleration
		// rb.AddForce(moveDir * force);

		// linear movment
		rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);


		// rotate to face movement direction
		if (moveDir.sqrMagnitude > 0.001f)
		{
			Quaternion targetRotation = Quaternion.LookRotation(moveDir);
			Quaternion smooth = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
			rb.MoveRotation(smooth);
		}
		
	}

    public void Jump(InputAction.CallbackContext callbackContext)
    {
		if (callbackContext.performed)
		{
			// rb.AddForce(Vector3.up * upForce);
			animator.SetTrigger("hello");
		}
		Debug.Log("Jump" + callbackContext.phase);
    }

	private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
    }

	public void Interact(InputAction.CallbackContext callbackContext)
	{
		Debug.Log("Interact wiii");
	}
}
