using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
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
	[SerializeField] private CinemachineCamera digitalCameraPlayerRythm;
	private float defaultZoom = 20;
	private float maxZoom = 5;
	private float speedZoom = 0.5f;

	private bool transition = false;

	private GameObject interactableGO = null;
	[SerializeField] private SkinnedMeshRenderer model;
	[SerializeField] private GameObject rythmMapPlayerSpawn;
	[SerializeField] private GameObject rythmMap;

	[Header("Accessories GO")]
	[SerializeField] public GameObject headbandRewardGO;
	[SerializeField] public GameObject wingsRewardGO;
	private Vector3 lastPositionPlayerMap;
	private Quaternion lastRotationPlayerMap;





	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		animator = GetComponent<Animator>();
		animatorWings = wingsRewardGO.GetComponent<Animator>();
		digitalCameraPlayer.gameObject.SetActive(true);
		HideAccessories();

		// comment for test
		rythmMap.gameObject.SetActive(false);
		// rythmMap.GetComponent<RythmGame>().StartSong();
		// playerInput.SwitchCurrentActionMap("RhythmGame");
		RythmGameLoad();
	}

	void Update()
	{
		input = playerInput.actions["Move"].ReadValue<Vector2>();
	}

	void FixedUpdate()
	{
		if (transition) return;

		Vector3 moveDir = new(input.x, 0f, input.y);

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
			if (wingsRewardGO.activeInHierarchy) StartCoroutine(JumpCoroutine());
		}
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
		wingsRewardGO.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Interactable") return;

		interactableClose = true;
		interactableGO = other.gameObject;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Interactable") return;

		interactableClose = false;
		interactableGO = null;
	}

	public void Interact(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed) return;
		if (interactableClose)
		{
			transition = true;
			// digitalCamera.gameObject.SetActive(true);
			// interactable.SetActive(false);
			StartCoroutine(CameraBlendCoroutine());
			transition = false;
		}
	}

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
		digitalCameraPlayerRythm.Priority = 0;
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
		while (cinemachineBrain.IsBlending)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);

		while (digitalCameraPlayer.Lens.FieldOfView > maxZoom)
		{
			digitalCameraPlayer.Lens.FieldOfView -= speedZoom;
			yield return null;
		}
		// RunInteractionWhiteDemon();
		RunInteraction();
		yield return new WaitForSeconds(2);
		RythmGameLoad();
		// DefaultCameraView();
	}

	void RythmGameLoad()
	{
		rythmMap.SetActive(true);
		// rythmMap.GetComponent<RythmGame>().SetLastTransformPlayerMap(transform);
		lastPositionPlayerMap = transform.position;
		lastRotationPlayerMap = transform.rotation;

		transform.position = rythmMapPlayerSpawn.transform.position;
		transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
		digitalCameraPlayerRythm.Priority = 1;
		digitalCameraMain.Priority = 0;
		digitalCameraPlayer.Priority = 0;
		playerInput.SwitchCurrentActionMap("RhythmGame");
		rythmMap.GetComponent<RythmGame>().StartSong();
	}

	public void ReturnPlayer()
	{
		transform.position = lastPositionPlayerMap;
		transform.rotation = lastRotationPlayerMap;
		rythmMap.SetActive(false);
		DefaultCameraView();
	}
}