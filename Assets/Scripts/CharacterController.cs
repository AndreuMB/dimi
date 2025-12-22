using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterController : MonoBehaviour
{

    // public float speed = 20f;
	// private float upForce = 200f, force = 15f;

	private Rigidbody rb;
	private PlayerInput playerInput;
	private Vector2 input;
	private Animator animator;
	private Animator animatorWings;
	// [SerializeField] private BoxCollider interactionCollider;
	private float rotationSpeed = 10f, moveSpeed = 2;
	private bool interactableClose = false;
	[SerializeField] private CinemachineBrain cinemachineBrain;
	[SerializeField] private CinemachineCamera digitalCameraMain;
	[SerializeField] private CinemachineCamera digitalCameraPlayer;
	private float defaultZoom = 20;
	private float maxZoom = 5;
	private float speedZoom = 0.5f;

	private GameObject interactableGO = null;
	[SerializeField] private SkinnedMeshRenderer model;

	[Header("Accessories GO")]
	[SerializeField] public GameObject headbandRewardGO;
	[SerializeField] public GameObject wingsRewardGO;

	

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		animator = GetComponent<Animator>();
		animatorWings = wingsRewardGO.GetComponent<Animator>();
		digitalCameraPlayer.gameObject.SetActive(true);
		HideAccessories();
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
			StartCoroutine(JumpCoroutine());
		}
		Debug.Log("Jump" + callbackContext.phase);
    }

	private IEnumerator JumpCoroutine()
	{
		animatorWings.SetTrigger("fly");
		yield return new WaitForSeconds(0.3f);
		rb.AddForce(Vector3.up * 500);
	}

	public void Emote1(InputAction.CallbackContext callbackContext)
    {
		if (callbackContext.performed)
		{
			animator.SetTrigger("hello");
		}
    }

	private void HideAccessories()
	{
		headbandRewardGO.gameObject.SetActive(false);
		// wingsRewardGO.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag != "Interactable") return;
		
		interactableClose = true;
		interactableGO = other.gameObject;

        Debug.Log("Trigger enter = " + other.gameObject.name);
    }

	private void OnTriggerExit(Collider other)
    {
		if (other.gameObject.tag != "Interactable") return;

		interactableClose = false;
		interactableGO = null;

        Debug.Log("Trigger exit = " + other.gameObject.name);
    }

	public void Interact(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed) return;
		if (interactableClose)
		{
			// digitalCamera.gameObject.SetActive(true);
			Debug.Log("interactable = " + interactableGO);
			// interactable.SetActive(false);
			StartCoroutine(CameraBlendCoroutine());
			
		}
	}

	// private void RunInteractionWhiteDemon()
	// {
	// 	Interactable interactableScript = interactable.GetComponent<Interactable>();

	// 	switch (interactableScript.rewardNumber)
	// 	{
	// 		case Rewards.WhiteMaterial:
	// 			Material whiteMaterial = interactableScript.GetInteractableWhiteColor();
	// 			model.material = whiteMaterial;
	// 			break;

	// 		case Rewards.Headband:
	// 			headbandRewardGO.SetActive(true);
	// 			break;
			
	// 		default:
	// 			Debug.Log("Reward not exists");
	// 			break;
	// 	}
		
	// }

	private void RunInteraction()
	{
		Interactable interactable = interactableGO.GetComponent<Interactable>();
		interactable.Interact(this);
	}

	public void SetNewMaterial(Material newMaterial)
	{
		model.material = newMaterial;
	}

	public void EnableObjectReward(RewardObjectName rewardObjectName)
	{
		switch (rewardObjectName)
		{
			case RewardObjectName.headband:
				headbandRewardGO.SetActive(true);
			break;
			case RewardObjectName.wings:
				wingsRewardGO.SetActive(true);
			break;
		}
	}

	public void DisableDigitalCamera(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed) return;
		// if (callbackContext.performed)
		DefaultCameraView();
	}

	void DefaultCameraView()
	{
		digitalCameraPlayer.Lens.FieldOfView = defaultZoom;
		digitalCameraMain.Priority = 1;
		digitalCameraPlayer.Priority = 0;
	}

	// private void ZoomIn()
	// {
	// 	digitalCameraPlayer.Lens.FieldOfView = 5;
	// }

	IEnumerator CameraBlendCoroutine()
    {
		digitalCameraMain.Priority = 0;
		digitalCameraPlayer.Priority = 1;
		// wait to start the blending
		yield return null;
        while (cinemachineBrain.IsBlending) { 
			yield return null; 
		}
		yield return new WaitForSeconds(0.5f);

		while (digitalCameraPlayer.Lens.FieldOfView > maxZoom) { 
			digitalCameraPlayer.Lens.FieldOfView -= speedZoom;
			yield return null; 
		}
		// RunInteractionWhiteDemon();
		RunInteraction();
		yield return new WaitForSeconds(2);
		DefaultCameraView();
    }
}