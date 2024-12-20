using Unity.VisualScripting;
using UnityEngine;

public class BulletMoney : MonoBehaviour
{
    private int damage;
    public void SetDamage(int SetDamage) => this.damage = SetDamage;
    public int GetDamage() => this.damage;
    private GameManager gameManager;
    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        damage = gameManager.GetDamageBullet();
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Plane")
        {
            Destroy(gameObject);
        }
    }
}
