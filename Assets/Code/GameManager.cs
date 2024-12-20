using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [Header("========== DIFFICULTY ==========")]
    [SerializeField] private Difficulty difficulty;

    [Header("========== COMPONENT UI ==========")]
    [SerializeField] Text scoreTxt;
    [SerializeField] Text wavetxt;
    [SerializeField] Text startWaveTxt;
    [SerializeField] Text CountWaveTxt;
    [SerializeField] GameObject panelUpgrade;
    [SerializeField] GameObject panelInGame;
    [SerializeField] GameObject panelResume;
    [SerializeField] GameObject panelGameover;
    [SerializeField] Image timerImage;
    [SerializeField] Text reloadMoney;
    [SerializeField] Text BaseReloadMoney;
    [SerializeField] GameObject reloadImage;
    [SerializeField] Image barHP;
    [SerializeField] GameObject instruksi10wave;

    [Header("========== COMPONENT SCRIPT OBJECT ==========")]
    [SerializeField] SpawnEnemyManager spawnEnemyManagerDepan;
    [SerializeField] SpawnEnemyManager spawnEnemyManagerBelakang;
    [SerializeField] UIFollowCursor uIFollowCursor;
    [SerializeField] Transform cameraPlayer;

    #region HealthPoint Player
    private int currentHP;

    #endregion

    #region Score and Wave component
    private float timeRemaining = 60; // Hitungan dalam detik
    private float updatetimeRemaining;
    private int indexScore;
    private int indexWave;
    #endregion

    #region HP Enemy component and Index Score Enemy
    private int maxHpSmall;
    private int maxHpMedium;
    private int maxHpLarge;
    private int ScoreSmall;
    private int ScoreMedium;
    private int ScoreLarge;
    #endregion

    #region Double POV wave component
    private bool isDoublePOV;
    private bool isForward = true;
    private int previousWave;
    private int lastCheckedWave = 0;
    #endregion

    #region Reload component
    private int referenceReloadIndex;
    private int shotsFired;
    #endregion

    #region Status Player Upgrading
    private int indexHealthPoint = 100;
    private int indexBaseMoney;
    private int indexReloadMoney;
    private int indexDamage;
    private float indexSpeedAmmo;
    private int baseMoney;
    #endregion

    [Header("========== CROSHAIR AND BGM SFX ==========")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioClip normalBgm;
    [SerializeField] AudioClip gameOverBgm;
    [SerializeField] AudioClip fufufafaBgm;
    [SerializeField] AudioClip reloadSfx;
    [SerializeField] Image CrosshairPlayer;
    [SerializeField] Sprite[] croshair = new Sprite[5];

    [Header("========== COMPONENT NAME PLAYER ==========")]
    [SerializeField] Text namePlayertxt;
    [SerializeField] Image iconPlayer;

    [Header("========== GAMEOVER VARIABEL ==========")]
    [SerializeField] Text honorGameOverTxt;

    [Header("========== SHAKE CAMERA ==========")]
    [SerializeField] CameraShake cameraShake;

    private List<GameObject> enemiesList = new List<GameObject>();
    private bool isPaused = false;
    private bool isDead = false;
    private float indexSfx;
    private float indexBgm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameStart();
        LoadImageProfil();
        LoadPlayerName();
        updatetimeRemaining = timeRemaining;
        currentHP = indexHealthPoint;
        previousWave = indexWave;
        scoreTxt.text = indexScore.ToString();
        
        //Inisialisasi BGM dari playerperfs
        if (PlayerPrefs.GetInt("BGM") == 0)
        {
            audioSource.clip = normalBgm;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = fufufafaBgm;
            audioSource.Play();
        }

        //Inisialisasi croshair dari playerperfs
        switch (PlayerPrefs.GetInt("Croshair"))
        {
            case 0:
                CrosshairPlayer.sprite = croshair[0];
                break;
            case 1:
                CrosshairPlayer.sprite = croshair[1];
                break;
            case 2:
                CrosshairPlayer.sprite = croshair[2];
                break;
            case 3:
                CrosshairPlayer.sprite = croshair[3];
                break;
            case 4:
                CrosshairPlayer.sprite = croshair[4];
                break;
        }
        StartCoroutine(StartCountdown());
        StartCoroutine(WaveTxt());
    }

    // Update is called once per frame
    void Update()
    {
        instruksi10wave.SetActive(isDoublePOV);
        
        if (isDoublePOV)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (isForward)
                {
                    cameraPlayer.rotation = Quaternion.Euler(0f, 180f, 0f);
                    isForward = false;
                }
                else
                {
                    cameraPlayer.rotation = Quaternion.Euler(0f, 0f, 0f);
                    isForward = true;
                }
            }
        }
        if (indexReloadMoney < referenceReloadIndex)
        {
            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(Reloading());
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!isPaused)
            {
                OnPaused();
            }
        }

        if (currentHP <= 0)
        {
            if (!isDead)
            {
                panelGameover.SetActive(true);
                uIFollowCursor.SetCanShoot(false);
                spawnEnemyManagerDepan.SetIsCanSpawn(false);
                spawnEnemyManagerBelakang.SetIsCanSpawn(false);
                audioSource.clip = gameOverBgm;
                audioSource.Play();
                honorGameOverTxt.text = indexScore.ToString();
                isDead = true;

                if (difficulty == Difficulty.Easy)
                {
                    if (indexScore > PlayerPrefs.GetInt("HighScoreEasy"))
                    {
                        PlayerPrefs.SetInt("HighScoreEasy", indexScore);
                    }
                }
                else if (difficulty == Difficulty.Medium)
                {
                    if (indexScore > PlayerPrefs.GetInt("HighScoreMedium"))
                    {
                        PlayerPrefs.SetInt("HighScoreMedium", indexScore);
                    }
                }
                else if (difficulty == Difficulty.Hard)
                {
                    if (indexScore > PlayerPrefs.GetInt("HighScoreHard"))
                    {
                        PlayerPrefs.SetInt("HighScoreHard", indexScore);
                    }
                }
            }
        }
    }
    void GameStart()
    {
        if (difficulty == Difficulty.Easy)
        {
            maxHpSmall = 100;
            maxHpMedium = 200;
            maxHpLarge = 400;

            ScoreSmall = 2;
            ScoreMedium = 6;
            ScoreLarge = 20;

            indexDamage = 10;
            indexHealthPoint = 100;
            indexReloadMoney = 100;
            indexBaseMoney = 2750;
            indexSpeedAmmo = 1.5f;
        }
        else if (difficulty == Difficulty.Medium)
        {
            maxHpSmall = 150;
            maxHpMedium = 250;
            maxHpLarge = 450;

            ScoreSmall = 2;
            ScoreMedium = 6;
            ScoreLarge = 20;

            indexDamage = 10;
            indexHealthPoint = 100;
            indexReloadMoney = 85;
            indexBaseMoney = 2250;
            indexSpeedAmmo = 1.5f;
        }
        else if (difficulty == Difficulty.Hard)
        {
            maxHpSmall = 200;
            maxHpMedium = 300;
            maxHpLarge = 500;

            ScoreSmall = 2;
            ScoreMedium = 6;
            ScoreLarge = 20;

            indexDamage = 10;
            indexHealthPoint = 100;
            indexReloadMoney = 70;
            indexBaseMoney = 1750;
            indexSpeedAmmo = 1.5f;
        }

        spawnEnemyManagerDepan.SetRandomTime(3, 15); // 3 to 15 default
        spawnEnemyManagerBelakang.SetRandomTime(3, 15); // 3 to 15 default
        referenceReloadIndex = indexReloadMoney;
        baseMoney = indexBaseMoney;
        reloadMoney.text = indexReloadMoney.ToString();
        BaseReloadMoney.text = indexBaseMoney.ToString();

        // Load nilai SFX dan BGM dari PlayerPrefs untuk slider dan audio volume
        indexSfx = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default ke 1.0 jika tidak ada nilai tersimpan
        indexBgm = PlayerPrefs.GetFloat("BGMVolume", 1.0f);  // Default ke 1.0 jika tidak ada nilai tersimpan
        audioSource.volume = indexBgm;
        sfx.volume = indexSfx;
    }
    private void UpdateHealthBar()
    {
        float fillAmount = (float)currentHP / indexHealthPoint;
        barHP.fillAmount = fillAmount; // Mengatur fill amount dari Image
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, indexHealthPoint); // Membatasi agar tidak melebihi MaxHP atau kurang dari 0
        cameraShake.TriggerShake();
        UpdateHealthBar();
    }
    public void Healing(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, indexHealthPoint); // Membatasi agar tidak melebihi MaxHP atau kurang dari 0
        UpdateHealthBar();
    }
    public void SetBaseMoney(int amount)
    {
        indexBaseMoney += amount;
        BaseReloadMoney.text = indexBaseMoney.ToString();
    }
    public void SetScore(int score)
    {
        this.indexScore += score;
        scoreTxt.text = indexScore.ToString();
    }
    private void SetWave(int wave)
    {
        this.indexWave += wave;
        CountWaveTxt.text = indexWave.ToString();

        // Cek jika wave adalah kelipatan 5
        if (indexWave % 5 == 0 && indexWave != lastCheckedWave)
        {
            maxHpSmall += 50;
            maxHpMedium += 50;
            maxHpLarge += 50;
            ScoreSmall += 1;
            ScoreMedium += 3;
            ScoreLarge += 10;
            lastCheckedWave = indexWave;
        }

        isDoublePOV = (indexWave % 10 == 0) ? true : false;

        spawnEnemyManagerBelakang.gameObject.SetActive(isDoublePOV);
    }

    IEnumerator WaveTxt()
    {
        startWaveTxt.gameObject.SetActive(true);
        startWaveTxt.text = "WAVE " + indexWave.ToString();
        yield return new WaitForSeconds(3);
        startWaveTxt.gameObject.SetActive(false);
    }
    void LoadImageProfil()
    {
        // Muat gambar profil dari PlayerPrefs jika ada
        string savedImageBase64 = PlayerPrefs.GetString("PlayerProfileImage", string.Empty);
        if (!string.IsNullOrEmpty(savedImageBase64))
        {
            byte[] imageBytes = Convert.FromBase64String(savedImageBase64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            iconPlayer.sprite = sprite;
        }
    }
    void LoadPlayerName()
    {
        string playerName = PlayerPrefs.GetString("NamePlayer", "");

        if (!string.IsNullOrEmpty(playerName))
        {
            namePlayertxt.text = playerName;
        }
        else
        {
            namePlayertxt.text = "Fulan";
        }
    }
    #region Function untuk menembak money dan reload
    public void SetShootMoney(int shootMoney)
    {
        indexReloadMoney -= shootMoney;
        reloadMoney.text = indexReloadMoney.ToString();
        shotsFired += shootMoney;

        if (indexReloadMoney <= 0)
        {
            StartCoroutine(SetReload());
        }
    }

    IEnumerator SetReload()
    {
        uIFollowCursor.SetCanShoot(false);
        reloadImage.SetActive(true);

        yield return new WaitForSeconds(3);

        indexReloadMoney = referenceReloadIndex;
        indexBaseMoney -= referenceReloadIndex;

        uIFollowCursor.SetCanShoot(true);

        reloadImage.SetActive(false);
        reloadMoney.text = indexReloadMoney.ToString();
        BaseReloadMoney.text = indexBaseMoney.ToString();
        shotsFired = 0;
    }

    IEnumerator Reloading()
    {
        sfx.PlayOneShot(reloadSfx);
        uIFollowCursor.SetCanShoot(false);
        reloadImage.SetActive(true);

        indexBaseMoney -= shotsFired;

        yield return new WaitForSeconds(3);

        indexReloadMoney = referenceReloadIndex;

        uIFollowCursor.SetCanShoot(true);

        reloadImage.SetActive(false);
        reloadMoney.text = indexReloadMoney.ToString();
        BaseReloadMoney.text = indexBaseMoney.ToString();
        shotsFired = 0;
    }
    #endregion

    #region Function untuk wave
    IEnumerator StartCountdown()
    {
        float initialTime = timeRemaining;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            // Update fill amount based on remaining time
            timerImage.fillAmount = timeRemaining / initialTime;

            yield return null;  // Wait for the next frame
        }

        // When countdown reaches 0
        timerImage.fillAmount = 0; // Set to empty

        if (currentHP > 0)
        {
            OnCountdownEnd();
        }
    }

    //Fungsi wave selesai
    void OnCountdownEnd()
    {
        Debug.Log("Countdown has ended!");

        // Cari semua GameObject dengan tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Masukkan semua enemy ke dalam List enemiesList
        enemiesList.AddRange(enemies);

        // Hancurkan semua GameObject dalam enemiesList
        foreach (var enemy in enemiesList)
        {
            Destroy(enemy);
        }

        spawnEnemyManagerDepan.SetIsCanSpawn(false);
        spawnEnemyManagerBelakang.SetIsCanSpawn(false);

        wavetxt.gameObject.SetActive(true);

        // Memulai Coroutine untuk mengaktifkan panelUpgrade setelah 3 detik
        StartCoroutine(ActivatePanelUpgrade());
    }

    //Aktifkan panel upgrade
    IEnumerator ActivatePanelUpgrade()
    {
        yield return new WaitForSeconds(3);  // Tunggu 3 detik

        wavetxt.gameObject.SetActive(false); // Matikan text wave
        panelInGame.SetActive(false);        // Matikan panel in game
        panelUpgrade.SetActive(true);        // Aktifkan panelUpgrade

        enemiesList.Clear(); //Clear isi dari list

        isPaused = true;
        Debug.Log("panelUpgrade activated after 3 seconds!");
    }

    // Untuk mulai wave kembali
    public void RestartCountdown()
    {
        // Simpan nilai sebelumnya sebelum memperbarui
        previousWave = indexWave;
        isPaused = false;

        //Tambahkan indexWavenya dan update
        SetWave(1);

        //Rotasi camera jika selesai wave double POV
        if (previousWave % 10 == 0) { cameraPlayer.rotation = Quaternion.Euler(0f, 0f, 0f); isForward = true; }

        // Reset waktu
        timeRemaining = updatetimeRemaining;
        panelUpgrade.SetActive(false);
        panelInGame.SetActive(true);

        StartCoroutine(uIFollowCursor.ShootingCooldown(3f));
        spawnEnemyManagerDepan.SetIsCanSpawn(true);

        if (indexWave % 10 == 0) { spawnEnemyManagerBelakang.SetIsCanSpawn(true); timeRemaining = updatetimeRemaining * 2f; } //Jika kelipatan 10
        if (indexWave % 25 == 0) { updatetimeRemaining += 60f; }

        StartCoroutine(StartCountdown()); // Mulai kembali hitung mundur
        StartCoroutine(WaveTxt());
    }
    #endregion

    #region Set Stats
    public void SetUpDamage(int damage)
    {
        indexDamage += damage;
    }
    public void SetUpHealthPoint(int healthPoint)
    {
        indexHealthPoint += healthPoint;
    }
    public void SetUpAmmo(int ammo)
    {
        indexReloadMoney += ammo;
        referenceReloadIndex = indexReloadMoney;
    }
    public void SetUpBaseAmmo(int baseammo)
    {
        indexBaseMoney += baseammo;
        baseMoney = indexBaseMoney;
    }
    public void SetUpSpeedAmmo(float speedAmmo)
    {
        indexSpeedAmmo += speedAmmo;
    }
    #endregion

    #region GET FUNCTION
    public int GetHPSmall() => maxHpSmall;
    public int GetHpMedium() => maxHpMedium;
    public int GetHpLarge() => maxHpLarge;
    public int GetScore() => indexScore;
    public int GetScoreSmall() => ScoreSmall;
    public int GetScoreMedium() => ScoreMedium;
    public int GetScoreLarge() => ScoreLarge;

    public int GetDamageBullet() => indexDamage;
    public int GetHealthPoint() => indexHealthPoint;
    public float GetSpeedAmmo() => indexSpeedAmmo;

    public int GetHealthPlayer() => currentHP;
    public float GetTimer() => timeRemaining;
    public float GetMaxTimer() => updatetimeRemaining;
    public float GetIndexBaseMoney() => indexBaseMoney;
    public float GetBaseMoney() => baseMoney;
    public int GetIntIndexBaseMoney() => indexBaseMoney;
    public int GetIntBaseMoney() => baseMoney;
    #endregion

    #region Paused Function
    private void OnPaused()
    {
        isPaused = true;
        uIFollowCursor.SetCanShoot(false);
        panelResume.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnResumeButtonClicked()
    {
        panelResume.SetActive(false);
        isPaused = false;
        StartCoroutine(uIFollowCursor.ShootingCooldown(1f));
        Time.timeScale = 1;
    }
    public void OnRetryButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    #endregion
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
