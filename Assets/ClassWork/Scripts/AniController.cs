using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class AniController : MonoBehaviour
{
    readonly static private int aniSpeedKey = Animator.StringToHash("Speed");
    readonly static private int injuryKey = Animator.StringToHash("Injury");
    readonly static private int carryingKey = Animator.StringToHash("IsCarrying");

    private Animator animator;

    private InputSystem_Actions inputSystem;

    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Player.Disable();
    }

    private void Start()
    {
        animator.SetFloat(aniSpeedKey, 1);
    }

    private void Update()
    {
        float move = inputSystem.Player.Move.ReadValue<Vector2>().y;
        transform.position += new Vector3(0, 0, move) * Time.deltaTime;

        animator.SetFloat(aniSpeedKey, Mathf.Abs(move));

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animator.SetFloat(injuryKey, 1f);
        }
        if(animator.GetFloat(injuryKey) > 0)
        {
            float newVal = Mathf.MoveTowards(animator.GetFloat(injuryKey), 0, Time.deltaTime * 0.35f);
            animator.SetFloat(injuryKey, newVal);
        }

        if(Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            animator.SetBool(carryingKey, true);
        }
        
        if(Keyboard.current.shiftKey.wasReleasedThisFrame)
        {
            animator.SetBool(carryingKey, false);
        }
    }
}
