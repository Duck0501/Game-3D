using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public GameObject effect;
    private Rigidbody rb;


    void Start()
    {
        Destroy(gameObject, 2f); 
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        rb.velocity = transform.forward * speed; 
        Invoke("Deactivate", lifeTime); 
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameObject eff = (GameObject)Instantiate(effect, transform.position, transform.rotation);
            Destroy(eff, 0.5f);
            other.gameObject.SetActive(false); 
            Deactivate();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void Deactivate()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}