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

	private GameObject interactableTouchGO = null;
	private Interactable interactableSave;
	[SerializeField] private SkinnedMeshRenderer model;
	[SerializeField] private GameObject rythmMapPlayerSpawn;
	[SerializeField] private GameObject rythmMap;

	[Header("Accessories GO")]
	[SerializeField] public GameObject headbandRewardGO;
	[SerializeField] public GameObject wingsRewardGO;
	private Vector3 lastPositionPlayerMap;
	private Quaternion lastRotationPlayerMap;
	private bool gamepad = false;
	[SerializeField] private Song defaultSong;


	void Start()
	{
		rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		animator = GetComponent<Animator>();
		animatorWings = wingsRewardGO.GetComponent<Animator>();
		digitalCameraPlayer.gameObject.SetActive(true);
		HideAccessories();
		rythmMap.gameObject.SetActive(false);

		// comment for test
		RythmGameLoad();
	}

	void Update()
	{
		input = playerInput.actions["Move"].ReadValue<Vector2>();
		gamepad = playerInput.currentControlScheme == "Gamepad";
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
		interactableTouchGO = other.gameObject;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Interactable") return;

		interactableClose = false;
		interactableTouchGO = null;
	}

	public void Interact(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed) return;
		if (interactableClose)
		{
			interactableSave = interactableTouchGO.GetComponent<Interactable>();
			StartCoroutine(PlayerToRhythmGameCoroutine());
		}
	}

	private IEnumerator PlayerToRhythmGameCoroutine()
	{
		yield return StartCoroutine(CameraBlendCoroutine());
		RythmGameLoad();
	}

	private void RunInteraction()
	{
		interactableSave.Interact(this);
		Debug.Log("You got " + interactableSave.name);
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
		transition = true;
		digitalCameraPlayer.Priority = 1;
		digitalCameraPlayerRythm.Priority = 0;
		digitalCameraMain.Priority = 0;
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
		yield return new WaitForSeconds(2);
		transition = false;
	}

	IEnumerator CameraBlendWorldToRhythmCoroutine()
	{
		digitalCameraPlayerRythm.Priority = 1;
		digitalCameraMain.Priority = 0;
		digitalCameraPlayer.Priority = 0;
		// wait to start the blending
		yield return null;
		while (cinemachineBrain.IsBlending)
		{
			yield return null;
		}
		// yield return new WaitForSeconds(0.5f);

		rythmMap.GetComponent<RythmGame>().StartSong(interactableSave ? interactableSave.song : defaultSong);
		// rythmMap.GetComponent<RythmGame>().StartSong(defaultSong);

		// yield return new WaitForSeconds(2);
		// transition = false;
	}

	void RythmGameLoad()
	{
		// rythmMap.GetComponent<RythmGame>().SetLastTransformPlayerMap(transform);
		lastPositionPlayerMap = transform.position;
		lastRotationPlayerMap = transform.rotation;

		transform.position = rythmMapPlayerSpawn.transform.position;
		transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
		// digitalCameraPlayerRythm.Priority = 1;
		// digitalCameraMain.Priority = 0;
		// digitalCameraPlayer.Priority = 0;
		playerInput.SwitchCurrentActionMap("RhythmGame");
		rythmMap.SetActive(true);
		StartCoroutine(CameraBlendWorldToRhythmCoroutine());
	}

	public void ReturnPlayer()
	{
		StartCoroutine(ReturnPlayerCoroutine());
	}

	private IEnumerator ReturnPlayerCoroutine()
	{
		yield return StartCoroutine(CameraBlendCoroutine());
		transform.position = lastPositionPlayerMap;
		transform.rotation = lastRotationPlayerMap;
		rythmMap.SetActive(false);
		DefaultCameraView();
		playerInput.SwitchCurrentActionMap("Player");
		RunInteraction();
	}

	public bool IsUsingGamepad()
	{
		return gamepad;
	}

	public void ToggleMenu(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed) return;
		Debug.Log("Toggle Menu");
	}

}