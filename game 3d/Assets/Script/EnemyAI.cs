using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Vị trí Player
    private NavMeshAgent agent;
    public float detectionRange = 10f; // Khoảng cách phát hiện
    public float attackRange = 2f; // Khoảng cách tấn công
    public float speed = 3f; // Tốc độ di chuyển

    public float fireRate = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float nextFireTime;

    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= detectionRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distanceToPlayer <= attackRange)
                    currentState = EnemyState.Attack;
                else if (distanceToPlayer > detectionRange)
                    currentState = EnemyState.Idle;
                else
                    ChasePlayer();
                break;
        }

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > attackRange)
            {
                agent.isStopped = false;
                isAttacking = false;
            }
            else
            {
                if (!isAttacking)
                {
                    agent.isStopped = true; // Dừng lại khi trong phạm vi tấn công
                    isAttacking = true;
                    InvokeRepeating("Shoot", 0f, fireRate); // Bắt đầu bắn mỗi 2 giây
                }
            }
        }
        else
        {
            agent.isStopped = true;
            isAttacking = false;
            CancelInvoke("Shoot"); // Ngừng bắn nếu player rời khỏi phạm vi
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(player); // Quay mặt về phía Player
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (player.position - firePoint.position).normalized;
        rb.velocity = direction * 10f;
    }
}
