using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AntiFloating : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        rb.useGravity = true;

       
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
       
        if (transform.position.y > 0.1f && rb.velocity.y == 0)
        {
            rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
        }
    }
}
