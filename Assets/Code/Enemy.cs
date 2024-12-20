using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    [SerializeField] private Type tipe;
    [SerializeField] private float speed;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material bajuKoruptor;
    private Animator animator;
    private Collider enemyCollider;
    private GameManager gameManager;
    private int HP;
    private float valueSpeed;
    private float valueSpeedRage;
    private Transform target;
    private NavMeshAgent navMeshAgent;

    private bool isDead;
    private bool isAttack;
    private bool isKnocked;
    private bool isReachedPoint;

    private int indexScore;
    private Coroutine knockCoroutine; // Menyimpan referensi coroutine

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        valueSpeed = speed;
        valueSpeedRage = valueSpeed * 2;
        target = FindAnyObjectByType<Camera>().GetComponent<Transform>();
        gameManager = FindAnyObjectByType<GameManager>();
        enemyCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        // Tentukan target awal
        Vector3 randomTarget = GenerateRandomTarget();
        navMeshAgent.SetDestination(randomTarget);
        navMeshAgent.speed = speed;

        if (tipe == Type.small)
        {
            HP = gameManager.GetHPSmall();
            indexScore = gameManager.GetScoreSmall();
        }
        else if (tipe == Type.medium)
        {
            HP = gameManager.GetHpMedium();
            indexScore = gameManager.GetScoreMedium();
        }
        else if (tipe == Type.large)
        {
            HP = gameManager.GetHpLarge();
            indexScore = gameManager.GetScoreLarge();
        }
    }

    void Update()
    {
        // Cek apakah sudah sampai di tujuan NavMesh
        if (!isReachedPoint && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isReachedPoint = true; // Tandai bahwa sudah sampai
            navMeshAgent.enabled = false; // Matikan NavMeshAgent untuk berpindah ke pergerakan manual
        }

        if (isReachedPoint)
        {
            MoveToTarget();
        }

        if (HP <= 0 && !isDead)
        {
            isDead = true;
            navMeshAgent.speed = 0;
            animator.SetBool("dead", true);
            enemyCollider.enabled = false;
            gameManager.SetScore(indexScore);
            Destroy(gameObject, 4f);
        }
    }

    // Fungsi untuk menghasilkan target random
    Vector3 GenerateRandomTarget()
    {
        float randomX = Random.Range(-8f, 8f);
        float fixedY = transform.position.y;  
        float targetZ = 5f; 

        return new Vector3(randomX, fixedY, targetZ); // Kembalikan posisi target
    }

    void MoveToTarget()
    {
        if (!isDead && !isAttack && !isKnocked) // Cek isKnocked
        {
            float fixedY = transform.position.y; // Simpan posisi Y asli
            Vector3 targetPosition = new Vector3(target.position.x, fixedY, target.position.z); // Tetapkan Y tetap

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

            // Membuat objek selalu menghadap ke arah target
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        }
    }

    void AttackConfirm()
    {
        if (isAttack)
        {
            StartCoroutine(AttackPerSecond());
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Duit"))
        {
            BulletMoney bulletMoney = collider.GetComponent<BulletMoney>();

            animator.SetTrigger("damage");
            HP -= bulletMoney.GetDamage();

            if (HP > 0 && HP < 50)
            {
                animator.SetFloat("HP", 1);
                valueSpeed = valueSpeedRage;
                skinnedMeshRenderer.material = bajuKoruptor;
            }

            // Hentikan coroutine sebelumnya jika ada
            if (knockCoroutine != null)
            {
                StopCoroutine(knockCoroutine);
            }
            knockCoroutine = StartCoroutine(Knock()); // Mulai coroutine baru

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Barier"))
        {
            animator.SetBool("attack", true);
            isAttack = true;
            AttackConfirm();
        }
    }

    IEnumerator Knock()
    {
        isKnocked = true; // Tandai sedang knock
        speed = 0; // Set speed ke 0 selama knock
        navMeshAgent.speed = speed;
        yield return new WaitForSeconds(3f);
        speed = valueSpeed; // Kembalikan speed
        isKnocked = false; // Selesai knock
        knockCoroutine = null; // Reset referensi coroutine
        navMeshAgent.speed = speed;
    }

    IEnumerator AttackPerSecond()
    {
        while (isAttack)
        {
            gameManager.TakeDamage(10);
            yield return new WaitForSeconds(1f); // Menunggu 1 detik sebelum serangan berikutnya
            Debug.Log("DARAH BERKURANG MENJADI = " + gameManager.GetHealthPlayer());
        }
    }
}

public enum Type
{
    small,
    medium,
    large
}
