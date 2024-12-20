using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("COMPONENT UI")]
    [SerializeField] Text scoreTxt;
    [SerializeField] Text costUpHealTxt;
    [SerializeField] Text costUpDamageTxt;
    [SerializeField] Text costUpAmmoTxt;
    [SerializeField] Text costUpBaseAmmoTxt;
    [SerializeField] Text costUpSpeedAmmoTxt;
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip[] sfxClip = new AudioClip[2];

    [Header("COMPONENT UI COMPLEX")]
    [SerializeField] GameObject parameterUpHeal;
    [SerializeField] GameObject parameterUpDamage;
    [SerializeField] GameObject parameterUpAmmo;
    [SerializeField] GameObject parameterUpBaseAmmo;
    [SerializeField] GameObject parameterUpSpeedAmmo;

    [Header("COMPONENT RECOVERY")]
    private int costRecovery;
    private bool isNotifActive = false;
    private bool isHpRecovery = false;
    private bool isMoneyRecovery = false;
    [SerializeField] GameObject panelConfirm;
    [SerializeField] Image barHP;
    [SerializeField] Image barMoney;
    [SerializeField] Image iconConfirm;
    [SerializeField] Text tittleConfirm;
    [SerializeField] Text costRecoveryTxt;
    [SerializeField] Sprite iconHp;
    [SerializeField] Sprite iconMoney;
    [SerializeField] GameObject notifAnim;

    [Header("COMPONENT SCRIPT")]
    [SerializeField] GameManager gameManager;
    [SerializeField] UIFollowCursor followCursor;

    private int score;

    private int costUpHeal;
    private int costUpDamage;
    private int costUpAmmo;
    private int costUpBaseAmmo;
    private int costUpSpeedAmmo;

    private int referenceCostHeal;
    private int referenceCostAmmo;
    private int referenceCostBaseAmmo;
    private int referenceCostSpeedAmmo;
    private int referenceCostDamage;

    private int[] DamageCostIncrements = { 100, 150, 225, 338, 507, 760, 1140, 1710, 2565, 3848 };
    private int[] BaseAmmoCostIncrements = { 80, 112, 157, 220, 308, 431, 603, 844, 1181, 1653 };
    private int[] healCostIncrements = { 60, 78, 101, 133, 174, 227, 295, 383, 498, 648 };
    private int[] SpeedAmmoCostIncrements = { 50, 62, 78, 97, 122, 152, 190, 238, 298, 372 };
    private int[] AmmoCostIncrements = { 40, 48, 58, 70, 84, 100, 120, 144, 172, 206 };


    private List<GameObject> childparameterUpHeal = new List<GameObject>();
    private List<GameObject> childparameterUpAmmo = new List<GameObject>();
    private List<GameObject> childparameterUpBaseAmmo = new List<GameObject>();
    private List<GameObject> childparameterUpDamage = new List<GameObject>();
    private List<GameObject> childparameterUpSpeedAmmo = new List<GameObject>();

    void Awake()
    {
        SetCostStart(60, 100, 40, 80, 50);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = gameManager.GetScore();
        scoreTxt.text = score.ToString();

        float fill = (float)gameManager.GetHealthPlayer() / gameManager.GetHealthPoint();
        barHP.fillAmount = fill;

        float fillMoney = (float)gameManager.GetIndexBaseMoney() / gameManager.GetBaseMoney();
        barMoney.fillAmount = fillMoney;

        GetChildBar(parameterUpHeal, childparameterUpHeal);
        GetChildBar(parameterUpDamage, childparameterUpDamage);
        GetChildBar(parameterUpAmmo, childparameterUpAmmo);
        GetChildBar(parameterUpBaseAmmo, childparameterUpBaseAmmo);
        GetChildBar(parameterUpSpeedAmmo, childparameterUpSpeedAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        followCursor.SetCanShoot(false);
    }

    #region Function Upgrade
    public void UpDamage()
    {
        if (referenceCostDamage < 10)
        {
            if (referenceCostDamage < DamageCostIncrements.Length && score > costUpDamage)
            {
                gameManager.SetScore(-costUpDamage);

                // Mengaktifkan semua child dari index 0 sampai referenceCostHeal
                for (int i = 0; i <= referenceCostDamage; i++)
                {
                    if (i < childparameterUpDamage.Count) // Cek apakah index berada dalam range list
                    {
                        childparameterUpDamage[i].SetActive(true);
                    }
                }

                // Tambah referensinya
                referenceCostDamage++;

                //Tambah indexnya
                gameManager.SetUpDamage(10);
                Debug.Log("DAMAGE : " + gameManager.GetDamageBullet());

                // Perbarui costUpHeal dengan nilai dari array
                costUpDamage += DamageCostIncrements[referenceCostDamage - 1];

                if (referenceCostDamage < 10)
                {
                    costUpDamageTxt.text = costUpDamage.ToString();
                }
                else
                {
                    costUpDamageTxt.text = "MAX";
                }

                score = gameManager.GetScore();
                scoreTxt.text = score.ToString();
                sfxAudioSource.PlayOneShot(sfxClip[0]);
            }
            else
            {
                ShowNotif("Honor Not Enoguh!");
            }
        }
    }
    public void UpHeal()
    {
        if (referenceCostHeal < 10)
        {
            if (referenceCostHeal < healCostIncrements.Length && score > costUpHeal)
            {
                gameManager.SetScore(-costUpHeal);

                // Mengaktifkan semua child dari index 0 sampai referenceCostHeal
                for (int i = 0; i <= referenceCostHeal; i++)
                {
                    if (i < childparameterUpHeal.Count) // Cek apakah index berada dalam range list
                    {
                        childparameterUpHeal[i].SetActive(true);
                    }
                }

                // Tambah Heal
                referenceCostHeal++;

                //Tambah indexnya
                gameManager.SetUpHealthPoint(100);

                // Perbarui costUpHeal dengan nilai dari array
                costUpHeal += healCostIncrements[referenceCostHeal - 1];

                if (referenceCostHeal < 10)
                {
                    costUpHealTxt.text = costUpHeal.ToString();
                }
                else
                {
                    costUpHealTxt.text = "MAX";
                }

                score = gameManager.GetScore();
                scoreTxt.text = score.ToString();
                sfxAudioSource.PlayOneShot(sfxClip[0]);
            }
            else
            {
                ShowNotif("Honor Not Enoguh!");
            }
        }
    }
    public void UpAmmo()
    {
        if (referenceCostAmmo < 10)
        {
            if (referenceCostAmmo < AmmoCostIncrements.Length && score > costUpAmmo)
            {
                gameManager.SetScore(-costUpAmmo);

                // Mengaktifkan semua child dari index 0 sampai referenceCostHeal
                for (int i = 0; i <= referenceCostAmmo; i++)
                {
                    if (i < childparameterUpAmmo.Count) // Cek apakah index berada dalam range list
                    {
                        childparameterUpAmmo[i].SetActive(true);
                    }
                }

                // Tambah Heal
                referenceCostAmmo++;

                //Tambah indexnya
                gameManager.SetUpAmmo(10);

                // Perbarui costUpHeal dengan nilai dari array
                costUpAmmo += AmmoCostIncrements[referenceCostAmmo - 1];

                if (referenceCostAmmo < 10)
                {
                    costUpAmmoTxt.text = costUpAmmo.ToString();
                }
                else
                {
                    costUpAmmoTxt.text = "MAX";
                }

                score = gameManager.GetScore();
                scoreTxt.text = score.ToString();
                sfxAudioSource.PlayOneShot(sfxClip[0]);
            }
            else
            {
                ShowNotif("Honor Not Enoguh!");
            }
        }
    }
    public void UpBaseAmmo()
    {
        if (referenceCostBaseAmmo < 10)
        {
            if (referenceCostBaseAmmo < BaseAmmoCostIncrements.Length && score > costUpBaseAmmo)
            {
                gameManager.SetScore(-costUpBaseAmmo);

                // Mengaktifkan semua child dari index 0 sampai referenceCostHeal
                for (int i = 0; i <= referenceCostBaseAmmo; i++)
                {
                    if (i < childparameterUpBaseAmmo.Count) // Cek apakah index berada dalam range list
                    {
                        childparameterUpBaseAmmo[i].SetActive(true);
                    }
                }

                // Tambah Heal
                referenceCostBaseAmmo++;

                //Tambah indexnya
                gameManager.SetUpBaseAmmo(500);

                // Perbarui costUpHeal dengan nilai dari array
                costUpBaseAmmo += BaseAmmoCostIncrements[referenceCostBaseAmmo - 1];

                if (referenceCostBaseAmmo < 10)
                {
                    costUpBaseAmmoTxt.text = costUpBaseAmmo.ToString();
                }
                else
                {
                    costUpBaseAmmoTxt.text = "MAX";
                }

                score = gameManager.GetScore();
                scoreTxt.text = score.ToString();
                sfxAudioSource.PlayOneShot(sfxClip[0]);
            }
            else
            {
                ShowNotif("Honor Not Enoguh!");
            }
        }
    }
    public void UpSpeedAmmo()
    {
        if (referenceCostSpeedAmmo < 10)
        {
            if (referenceCostSpeedAmmo < SpeedAmmoCostIncrements.Length && score > costUpSpeedAmmo)
            {
                gameManager.SetScore(-costUpSpeedAmmo);

                // Mengaktifkan semua child dari index 0 sampai referenceCostHeal
                for (int i = 0; i <= referenceCostSpeedAmmo; i++)
                {
                    if (i < childparameterUpSpeedAmmo.Count) // Cek apakah index berada dalam range list
                    {
                        childparameterUpSpeedAmmo[i].SetActive(true);
                    }
                }

                // Tambah Heal
                referenceCostSpeedAmmo++;

                //Tambah indexnya
                gameManager.SetUpSpeedAmmo(1.5f);

                // Perbarui costUpHeal dengan nilai dari array
                costUpSpeedAmmo += SpeedAmmoCostIncrements[referenceCostSpeedAmmo - 1];

                if (referenceCostSpeedAmmo < 10)
                {
                    costUpSpeedAmmoTxt.text = costUpSpeedAmmo.ToString();
                }
                else
                {
                    costUpSpeedAmmoTxt.text = "MAX";
                }

                score = gameManager.GetScore();
                scoreTxt.text = score.ToString();
                sfxAudioSource.PlayOneShot(sfxClip[0]);
            }
            else
            {
                ShowNotif("Honor Not Enoguh!");
            }
        }
    }
    #endregion

    public void PanelConfirm(bool isHp)
    {
        if (isHp)
        {
            if (barHP.fillAmount != 1)
            {
                panelConfirm.SetActive(true);
                ManageRecovery(isHp);
                iconConfirm.sprite = iconHp;
                tittleConfirm.text = "Recovery Health Point?";
                costRecoveryTxt.text = costRecovery.ToString();
                isHpRecovery = true;
            }
            else
            {
                ShowNotif("Health Point Full!");
            }
        }
        else
        {
            if (barMoney.fillAmount != 1)
            {
                panelConfirm.SetActive(true);
                ManageRecovery(isHp);
                iconConfirm.sprite = iconMoney;
                tittleConfirm.text = "Recovery Base Money?";
                costRecoveryTxt.text = costRecovery.ToString();
                isMoneyRecovery = true;
            }
            else
            {
                ShowNotif("Base Money Full!");
            }
        }
    }

    private void ShowNotif(string message)
    {
        if (!isNotifActive)
        {
            Text teks = notifAnim.transform.GetChild(1).GetComponent<Text>();
            teks.text = message;
            StartCoroutine(NotifPopUp());
            sfxAudioSource.PlayOneShot(sfxClip[1]);
        }
    }
    public void ConfirmRecoveryYes()
    {
        // Pastikan salah satu kondisi utama terpenuhi
        if ((!isHpRecovery && !isMoneyRecovery) || score < costRecovery)
        {
            ShowNotif("Honor Not Enough!");
            return;
        }

        // Recovery HP
        if (isHpRecovery)
        {
            int recoveryHP = gameManager.GetHealthPoint() - gameManager.GetHealthPlayer();
            gameManager.Healing(recoveryHP);

            float fill = (float)gameManager.GetHealthPlayer() / gameManager.GetHealthPoint();
            barHP.fillAmount = fill;

            gameManager.SetScore(-costRecovery);
            panelConfirm.SetActive(false);
            sfxAudioSource.PlayOneShot(sfxClip[0]);
            isHpRecovery = false; // Reset kondisi
        }
        // Recovery Money
        else if (isMoneyRecovery)
        {
            int recoveryMoney = gameManager.GetIntBaseMoney() - gameManager.GetIntIndexBaseMoney();
            gameManager.SetBaseMoney(recoveryMoney);

            float fillMoney = (float)gameManager.GetIndexBaseMoney() / gameManager.GetBaseMoney();
            barMoney.fillAmount = fillMoney;

            gameManager.SetScore(-costRecovery);
            panelConfirm.SetActive(false);
            sfxAudioSource.PlayOneShot(sfxClip[0]);
            isMoneyRecovery = false; // Reset kondisi
        }

        // Update skor setelah operasi selesai
        score = gameManager.GetScore();
        scoreTxt.text = score.ToString();
    }


    public void ConfirmRecoveryNo()
    {
        isHpRecovery = false;
        isMoneyRecovery = false;
        panelConfirm.SetActive(false);
        sfxAudioSource.PlayOneShot(sfxClip[2]);
    }

    public void ManageRecovery(bool isHp)
    {
        if (isHp)
        {
            // Menghitung persentase HP
            float fill = (float)gameManager.GetHealthPlayer() / gameManager.GetHealthPoint();

            // Menetapkan biaya berdasarkan persentase HP
            if (fill < 0.25f)
            {
                costRecovery = 15;
            }
            else if (fill < 0.5f)
            {
                costRecovery = 10;
            }
            else if (fill < 0.75f)
            {
                costRecovery = 5;
            }
            else
            {
                costRecovery = 1;
            }
        }
        else
        {
            // Menghitung persentase uang
            float fillMoney = (float)gameManager.GetIndexBaseMoney() / gameManager.GetBaseMoney();

            // Menetapkan biaya berdasarkan persentase uang
            if (fillMoney < 0.25f)
            {
                costRecovery = 15;
            }
            else if (fillMoney < 0.5f)
            {
                costRecovery = 10;
            }
            else if (fillMoney < 0.75f)
            {
                costRecovery = 5;
            }
            else
            {
                costRecovery = 1;
            }
        }

        // Update tampilan biaya recovery pada UI
        costRecoveryTxt.text = costRecovery.ToString();
    }

    IEnumerator NotifPopUp()
    {
        isNotifActive = true;
        notifAnim.SetActive(true);
        yield return new WaitForSeconds(2f);
        notifAnim.SetActive(false);
        isNotifActive = false;
    }
    private void GetChildBar(GameObject parameter, List<GameObject> objects)
    {
        // Ambil semua child dari parameterUpHeal
        foreach (Transform child in parameter.transform)
        {
            objects.Add(child.gameObject);
        }
    }

    void SetCostStart(int hp, int damage, int ammo, int baseAmmo, int speed)
    {
        costUpHeal = hp;
        costUpDamage = damage;
        costUpAmmo = ammo;
        costUpBaseAmmo = baseAmmo;
        costUpSpeedAmmo = speed;

        costUpHealTxt.text = hp.ToString();
        costUpDamageTxt.text = damage.ToString();
        costUpAmmoTxt.text = ammo.ToString();
        costUpBaseAmmoTxt.text = baseAmmo.ToString();
        costUpSpeedAmmoTxt.text = speed.ToString();
    }
}
