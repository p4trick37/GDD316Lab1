using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    readonly static private int speedAni = Animator.StringToHash("Speed");
    readonly static private int drunkAni = Animator.StringToHash("Drunk");
    readonly static private int pickupAni = Animator.StringToHash("Pickup");

    [Header("References")]
    [SerializeField] private CharacterController cc;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform pizzaPlacement;
    [SerializeField] private Pizza currentPizza;
    [SerializeField] private GameObject canPrefab;
    [SerializeField] private GameObject character;
    [SerializeField] private GameManager gm;
    [SerializeField] private TMP_InputField textField;
    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera frontCamera;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float mouseSens;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    private bool shouldMove;

    private float ySpeed;

    [Header("Drunk Stats")]
    [SerializeField] private float maxSwayRotation;
    [SerializeField] private float maxSwaySpeed;
    [SerializeField] private float maxCameraMovement;
    [SerializeField] private float maxDrift;
    private bool hasThrown = false;

    [Header("Raycasts")]
    [SerializeField] private float castDistance;
    [SerializeField] private Shop shop;


    [Header("Animation")]
    [SerializeField] private TwoBoneIKConstraint rightHandRig;
    [SerializeField] private TwoBoneIKConstraint leftHandRig;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform hint;
    [SerializeField] private Transform can;
    [SerializeField] private Transform hand;
    [SerializeField] private float weightSpeed;
    [SerializeField] private Animator ani;
    [SerializeField] private float speedAniAccel;
    private bool playingDrinkingAnimation;
    

    [SerializeField] private House targetHouse;

    [SerializeField] private bool holdingPizza;
    private bool foundHouse;
    private InputSystem_Actions inputSystem;

    private float xRotation;
    private float yRotation;
    private float zRotation;
    private float time;

    private bool isDead;

    
    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        Cursor.lockState = CursorLockMode.Locked;
        gm = FindAnyObjectByType<GameManager>();

        xRotation = cameraPivot.eulerAngles.x;
        yRotation = transform.eulerAngles.y;
        zRotation = 0;
        shouldMove = true;
        leftHandRig.weight = 0;
        rightHandRig.weight = 0;
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Player.Disable();
    }
    private void Update()
    {
        if (shouldMove == true && isDead == false)
        {
            time += Time.deltaTime;
            PlayerMovement();
        }
        else
        {
            time = 0;
        }


        if (playingDrinkingAnimation == true)
        {
            HandlePickupRig();
            ThrowCan();
        }

        FindDoor();

        if(holdingPizza == true)
        {
            HoldingPizza();
        }

        if(Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            EnterTextField();
        }
    }

    private void EnterTextField()
    {
        textField.Select();
        textField.ActivateInputField();
    }

    public void ExitTextField()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void PlayerMovement()
    {
        float xMoveInput = inputSystem.Player.Move.ReadValue<Vector2>().x;
        float yMoveInput = inputSystem.Player.Move.ReadValue<Vector2>().y;
        float xLookInput = inputSystem.Player.Look.ReadValue<Vector2>().x;
        float yLookInput = inputSystem.Player.Look.ReadValue<Vector2>().y;

        yRotation += xLookInput * Time.deltaTime * mouseSens;
        xRotation += yLookInput * Time.deltaTime * mouseSens;


        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        float swayMultiplier = Mathf.Lerp(0, maxSwayRotation, ani.GetFloat(drunkAni));
        float swaySpeed = Mathf.Lerp(0, maxSwaySpeed, ani.GetFloat(drunkAni));
        
        cameraPivot.transform.localRotation = Quaternion.Euler(-xRotation, swayMultiplier * Mathf.Sin(time * swaySpeed), swayMultiplier * Mathf.Sin(time * swaySpeed));
        float cameraMovement = Mathf.Lerp(0, maxCameraMovement, ani.GetFloat(drunkAni));
        cameraPivot.transform.localPosition = new Vector3(cameraMovement * Mathf.Sin(time * swaySpeed), 0, cameraMovement * -Mathf.Cos(time * swaySpeed));
        Vector3 move = Vector3.zero;
        move += yMoveInput * DrunkFowardDirection() * movementSpeed;

        move += xMoveInput * DrunkRightDirection() * movementSpeed;


        if(cc.isGrounded == false)
        {
            ySpeed -= gravity * Time.deltaTime;
        }
        else
        {
            ySpeed = 0;
            //if(inputSystem.Player.Jump.IsPressed())
            //{
            //    ySpeed = jumpHeight;
            //}
        }

        move = new Vector3(move.x, ySpeed, move.z);


        SetAni(move);

        cc.Move(move * Time.deltaTime);
    }

    private void FindDoor()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, castDistance);
        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.GetComponent<Pizza>() != null)
            {
                continue;
            }
            else if (hit.transform.GetComponent<House>() != null && targetHouse != null && hit.transform.gameObject == targetHouse.gameObject && foundHouse == false)
            {
                LosePizza();
                shouldMove = false;
                mainCamera.gameObject.SetActive(false);
                frontCamera.gameObject.SetActive(true);
                transform.position = transform.position + -transform.forward * 1.2f;
                ani.SetFloat(speedAni, 0);
                rightHandRig.weight = 0;
                leftHandRig.weight = 0;
                rightTarget.localRotation = Quaternion.Euler(0, -90, -90);
                GameObject canSpawned = Instantiate(canPrefab, transform.position + 0.5f * transform.forward, Quaternion.identity);
                can = canSpawned.transform;
                can.GetComponent<Rigidbody>().isKinematic = true;
                ani.SetBool(pickupAni, true);
                playingDrinkingAnimation = true;
                foundHouse = true;
                break;
            }
            else if (hit.transform.GetComponent<Shop>() != null && hit.transform.gameObject == shop.gameObject && holdingPizza == false)
            {
                holdingPizza = true;
                targetHouse = gm.PickRandomHouse();
                break;
            }
        }
    }

    private void SetAni(Vector3 movement)
    {
        if (movement.x == 0 && movement.z == 0)
        {
            ani.SetFloat(speedAni, Mathf.MoveTowards(ani.GetFloat(speedAni), 0, Time.deltaTime * speedAniAccel));
        }
        else
        {
            ani.SetFloat(speedAni, Mathf.MoveTowards(ani.GetFloat(speedAni), 1, Time.deltaTime * speedAniAccel));
        }
    }

    private Vector3 DrunkFowardDirection()
    {
        float drunkness = ani.GetFloat(drunkAni);
        float movementDrift = Mathf.Lerp(0, maxDrift, drunkness);
        float drift = Mathf.Sin(Time.time * maxSwaySpeed) * movementDrift;
        Vector3 direction = Quaternion.Euler(0, drift, 0) * transform.forward;
        character.transform.localRotation = Quaternion.Euler(0, drift, 0);
        return direction.normalized;
    }

    private Vector3 DrunkRightDirection()
    {
        float drunkness = ani.GetFloat(drunkAni);
        float movementDrift = Mathf.Lerp(0, maxDrift, drunkness);
        float drift = Mathf.Sin(Time.time * maxSwaySpeed) * movementDrift;
        Vector3 direction = Quaternion.Euler(0, drift, 0) * transform.right;
        return direction.normalized;
    }

    private void HandlePickupRig()
    {
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(1);

        if (state.IsName("Pickup"))
        {
            float progress = state.normalizedTime;
            Vector3 directionToCan = can.position - transform.position;
            can.rotation = Quaternion.LookRotation(directionToCan);
            Vector3 targetPosition = can.transform.position + can.transform.right * 0.0969998f;
            rightTarget.position = targetPosition;
            
            
            if (progress >= 0.45f)
            {
                can.SetParent(hand);
                can.localPosition = new Vector3(0.02f, 0.0913f, 0.0748f);
                can.localRotation = Quaternion.Euler(-54.509f, -179.528f, 250.582f);
                //    float tValue = Mathf.InverseLerp(0.56f, 1, progress);
                //   handRig.weight = Mathf.Lerp(1, 0, tValue);
                rightHandRig.weight = Mathf.MoveTowards(rightHandRig.weight, 0, weightSpeed * Time.deltaTime);
                //handRig.weight = 0;
            }
            else if (progress >= 0.08f)
            {
                //  float tValue = Mathf.InverseLerp(0.3f, 0.56f, progress);
                // handRig.weight = Mathf.Lerp(0, 1, progress);
                //handRig.weight = 1;
                rightHandRig.weight = Mathf.MoveTowards(rightHandRig.weight, 1, weightSpeed * Time.deltaTime);
            }
            ani.SetBool(pickupAni, false);
        }
    }

    private void ThrowCan()
    {
        AnimatorStateInfo state = ani.GetCurrentAnimatorStateInfo(1);
        if(state.IsName("Throw"))
        {
            float progress = state.normalizedTime;
            if (progress > 0.95f)
            {
                targetHouse = null;
                shouldMove = true;
                playingDrinkingAnimation = false;
                foundHouse = false;
                ani.SetFloat(drunkAni, ani.GetFloat(drunkAni) + 0.1f);
                gm.IncreaseHousesDelivered();
            }
            else if (progress >= 0.34f && hasThrown == false)
            {
                can.SetParent(null);
                Rigidbody canRb = can.GetComponent<Rigidbody>();
                if(canRb != null)
                {
                    canRb.isKinematic = false;
                    canRb.AddForce(new Vector3(transform.forward.x, 0.5f, transform.forward.z) * 10, ForceMode.Impulse);
                    hasThrown = true;
                }
            }
            mainCamera.gameObject.SetActive(true);
            frontCamera.gameObject.SetActive(false);
        }
        else
        {
            hasThrown = false;
        }
        
    }

    private void HoldingPizza()
    {
        leftHandRig.weight = 1;
        rightHandRig.weight = 1;
        currentPizza.transform.SetParent(pizzaPlacement);
        currentPizza.transform.localPosition = Vector3.zero;
        currentPizza.transform.localRotation = Quaternion.Euler(0, -90, 0);
        rightTarget.position = currentPizza.GetRightPosition().position;
        leftTarget.position = currentPizza.GetLeftPosition().position;
        rightTarget.localRotation = Quaternion.Euler(-90, -90, -90);
        leftTarget.localRotation = Quaternion.Euler(-90, -90, -90);
    }

    private void LosePizza()
    {
        currentPizza.transform.SetParent(null);
        currentPizza.transform.position = new Vector3(0, -20, 0);
        holdingPizza = false;
    }

    public House GetTargetHouse()
    {
        return targetHouse;
    }

    private void OnTriggerEnter(Collider other)
    {
        Car car = other.gameObject.GetComponent<Car>();
        if (car != null && car.GetKillStatus() == true)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            Vector3 direction = transform.position - other.transform.position;
            Vector3 launchAngle = Vector3.Normalize(new Vector3(direction.x, direction.y + 1, direction.z));
            rb.AddForce(launchAngle * 1540);
            isDead = true;
            gm.OnDeath();
        }
        else if(other.gameObject.CompareTag("Lava"))
        {
            isDead = true;
            gm.OnDeath();
        }
        Debug.Log("Trigger Happening");
    }

    public float GetDrunkLevel()
    {
        return ani.GetFloat(drunkAni);
    }

    public void SetDrunkLevel(float value)
    {
        if(value > 1)
        {
            value = 1;
        }
        else if(value < 0)
        {
            value = 0;
        }
        ani.SetFloat(drunkAni, value);
    }

    public void SetSpeed(float speed)
    {
        ani.SetFloat(speedAni, speed);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * castDistance);
    }


}
