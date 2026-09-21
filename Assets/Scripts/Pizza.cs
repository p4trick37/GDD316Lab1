using UnityEngine;

public class Pizza : MonoBehaviour
{
    [SerializeField] private Transform leftPosition;
    [SerializeField] private Transform rightPosition;
    
    public Transform GetLeftPosition()
    {
        return leftPosition;
    }

    public Transform GetRightPosition()
    {
        return rightPosition;
    }

}
