using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public Transform target;
    public float detectionRange = 10f;

    public float attackRange = 7f;
    public float fireRate = 2f; 
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float nextFireTime;
    private bool isAttacking = false;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position); // Di chuyển đến player
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

    void Shoot()
    {
        if (target == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (target.position - firePoint.position).normalized;
        direction.y = 0;
        rb.velocity = direction * 10f; // Đạn bay về hướng player
    }



    //if (target != null && agent.isOnNavMesh)
    //{
    //    float distanceToTarget = Vector3.Distance(transform.position, target.position);

    //    if (distanceToTarget <= chaseDistance)
    //    {
    //        agent.SetDestination(target.position);
    //        animator.SetBool("run", true);
    //    }
    //    else
    //    {
    //        agent.ResetPath();
    //        animator.SetBool("run", false);
    //    }
    //}
}
