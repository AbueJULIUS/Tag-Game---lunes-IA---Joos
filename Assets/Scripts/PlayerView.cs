using UnityEngine;

public class PlayerView : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("isRunning", rb.linearVelocity.magnitude);
    }
    public void Jump()
    {
        anim.SetTrigger("isJumping");
    }
}
