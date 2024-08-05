using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureObject : MonoBehaviour
{
    public float minForce = 0.1f;
    public float maxForce = 1.0f;
    public float minTorque = 0.1f;
    public float maxTorque = 1.0f;
    public float changeInterval = 1.0f;
    public float angleLimit = 45f;

    private Rigidbody2D rb2d;
    private float nextChangeTime;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        nextChangeTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= nextChangeTime)
        {
            ApplyRandomForceAndTorque();
            nextChangeTime = Time.time + changeInterval;
        }
    }

    void FixedUpdate() {
        float zRotation = rb2d.rotation;
        
        if (zRotation > 180) zRotation -= 360; // 把角度转换到[-180, 180]范围内
        if (zRotation < -180) zRotation += 360;

        // 限制角度在[-angleLimit, angleLimit]范围内
        if (zRotation > angleLimit)
        {
            rb2d.rotation = angleLimit;
        }
        else if (zRotation < -angleLimit)
        {
            rb2d.rotation = -angleLimit;
        }
    }

    void ApplyRandomForceAndTorque()
    {
        // 应用随机力
        float randomForceX = Random.Range(minForce, maxForce) * (Random.value > 0.5f ? 1 : -1);
        float randomForceY = Random.Range(minForce, maxForce) * (Random.value > 0.5f ? 1 : -1);
        Vector2 randomForce = new Vector2(randomForceX, randomForceY);
        rb2d.AddForce(randomForce);

        // 应用随机扭矩
        float randomTorque = Random.Range(minTorque, maxTorque) * (Random.value > 0.5f ? 1 : -1);
        rb2d.AddTorque(randomTorque);
    }
}
