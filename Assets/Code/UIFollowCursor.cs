using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFollowCursor : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private GameObject bulletPrefab;  // Prefab peluru
    [SerializeField] private Camera mainCamera;  // Referensi ke kamera utama
    [SerializeField] private GameManager gameManager; //Referensi gamemanager
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioClip moneyBullet;
    private float bulletSpeed;  // Kecepatan peluru
    private bool isCanShoot;

    void Start()
    {
        StartCoroutine(GetSpeedMoney());
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(ShootingCooldown(3f));
    }

    void Update()
    {
        // Mengambil posisi kursor dalam screen space
        Vector2 cursorPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            mainCamera,
            out cursorPosition
        );

        // Update posisi UI Image sesuai posisi kursor
        rectTransform.localPosition = cursorPosition;

        if (isCanShoot)
        {
            // Menembak peluru ketika mouse kiri ditekan
            if (Input.GetMouseButtonDown(0))
            {
                ShootBullet();
            }
        }
    }

    void ShootBullet()
    {
        // Dapatkan posisi Image dalam world space
        Vector3 imageWorldPosition = rectTransform.position;

        // Posisi spawn peluru di posisi kamera
        Vector3 spawnPosition = mainCamera.transform.position;

        // Menghitung arah peluru menuju posisi Image dari posisi kamera
        Vector3 shootDirection = (imageWorldPosition - spawnPosition).normalized;

        // Membuat peluru di posisi kamera dan tanpa rotasi
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Memberikan kecepatan ke peluru agar bergerak ke arah yang sudah dihitung
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = shootDirection * bulletSpeed;  // Gerak lurus ke arah Image
        }

        gameManager.SetShootMoney(1);

        sfx.PlayOneShot(moneyBullet);
    }

    IEnumerator GetSpeedMoney()
    {
        while (bulletSpeed == 0)
        {
             bulletSpeed = gameManager.GetSpeedAmmo();
             yield return null;
        }
    }
    public IEnumerator ShootingCooldown(float delay)
    {
        isCanShoot = false;  // Matikan kemampuan menembak
        yield return new WaitForSeconds(delay);
        isCanShoot = true;   // Aktifkan kembali setelah jeda waktu
    }
    public void SetCanShoot(bool canShoot) => this.isCanShoot = canShoot;
}
