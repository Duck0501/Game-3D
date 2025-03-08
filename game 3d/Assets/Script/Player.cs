using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Animator Anim;
    private string currentAni;

    private float horizontal;
    private float vertical;

    public int speed = 10;
    [SerializeField] private float jumpSpeed = 5;
    [SerializeField] private float attackEndTime = 0f;
    [SerializeField] private float shootEndTime = 0f;

    private bool isGround;
    [SerializeField] private Transform camera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private CinemachineVirtualCamera outsideCam;

    [SerializeField] private CinemachineFreeLook insideCam;

    public Slider slider;
    public int maxHp;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();

        if (!camera)
        {
            camera = Camera.main.transform;
        }
        if (!virtualCamera)
        {
            virtualCamera = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;

        slider.maxValue = maxHp;
        slider.value = maxHp;
    }

    void Update()
    {
        Movement();
        Jump();
        Attack();
        Shoot();

        if (currentAni == "punch" && Time.time >= attackEndTime)
        {
            changeAnim("idle");
        }
        if (currentAni == "Gunplay" && Time.time >= shootEndTime)
        {
            changeAnim("idle");
        }
    }

    private void Movement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > Mathf.Epsilon || Mathf.Abs(vertical) > Mathf.Epsilon)
        {
            if (isGround)
            {
                changeAnim("run");
            }

            Vector3 move = new Vector3(horizontal, 0, vertical).normalized;
            rb.MovePosition(transform.position + move * Time.deltaTime * speed);

            move = camera.TransformDirection(move);
            move.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = targetRotation;
        }
        else if (isGround)
        {
            changeAnim("idle");
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            changeAnim("jump");
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            isGround = false;
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGround)
        {
            changeAnim("punch");
            attackEndTime = Time.time + 2f;
        }
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            changeAnim("Gunplay");
            shootEndTime = Time.time + 1f;

            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = transform.rotation;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * 20f;
            }
        }
    }

    private void changeAnim(string AniName)
    {
        if (currentAni != AniName)
        {
            Anim.ResetTrigger(currentAni);
            currentAni = AniName;
            Anim.SetTrigger(currentAni);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            if (currentAni == "jump")
            {
                changeAnim("idle");
            }
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            slider.value--;
            if (slider.value <= 0)
            {
                SceneManager.LoadScene("Game Over");
            }

            CameraShake cameraShake = FindObjectOfType<CameraShake>();
            if (cameraShake != null)
            {
                cameraShake.Shake();
            }
        }
        if (collision.gameObject.CompareTag("obj"))
        {
            outsideCam.Priority = 5;
            insideCam.Priority = 10;
        }
    }
}
