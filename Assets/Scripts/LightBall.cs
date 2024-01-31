using UnityEngine;

public class LightBall : MonoBehaviour
{
    public Transform target; // 目標位置
    public float speed = 1.0f; // 移動速度
    public float jumpForce = 2.0f; // ジャンプ力

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = jumpForce; // Y軸方向に力を加える
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

