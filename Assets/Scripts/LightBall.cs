using UnityEngine;

public class LightBall : MonoBehaviour
{
    public Transform target; // �ڕW�ʒu
    public float speed = 1.0f; // �ړ����x
    public float jumpForce = 2.0f; // �W�����v��

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = jumpForce; // Y�������ɗ͂�������
        rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    void Update()
    {
        if (rb.velocity.magnitude < 0.01f)
        {
            rb.velocity = Vector3.zero;
        }
    }
}

