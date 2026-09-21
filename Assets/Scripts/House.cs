using UnityEngine;

public class House : MonoBehaviour
{ 
   
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z), new Vector3(2, 40, 2));
    }
}
