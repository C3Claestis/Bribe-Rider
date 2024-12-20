using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using AnotherFileBrowser.Windows;
using System.Collections;
using System;

public class MainMenuManager : MonoBehaviour
{
    [Header("========== COMPONENT PANEL ==========")]
    [SerializeField] GameObject panelDifficulty;
    [SerializeField] GameObject panelCredits;
    [SerializeField] GameObject panelSettings;
    [SerializeField] GameObject panelTutorial;

    #region Diffulty Item
    [Header("========== COMPONENT START GAME ==========")]
    [SerializeField] Text namePanelTxt;
    [SerializeField] InputField enterName;
    [SerializeField] Text nameProfil;
    [SerializeField] Image fotoProfil;
    #endregion

    #region Settings Item
    private float indexSfx;
    private float indexBgm;

    [Header("========== COMPONENT SETTINGS ==========")]
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Image croshairImage;
    [SerializeField] Toggle toggleNormalBgm;
    [SerializeField] Toggle toggleFufufafaBgm;
    [SerializeField] AudioClip normalBGM;
    [SerializeField] AudioClip fufufafaBGM;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource sfx;
    [SerializeField] Toggle[] croshair = new Toggle[5];
    [SerializeField] Sprite[] spritesCroshair = new Sprite[5];
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PanelActived(panelDifficulty.name, "START GAME");

        LoadPlayerName();
        LoadImageProfil();

        // Load pilihan BGM dari PlayerPrefs
        int savedBgmIndex = PlayerPrefs.GetInt("BGM", 0);  // Default ke 0 (normalBGM) jika belum ada nilai tersimpan
        if (savedBgmIndex == 0)
        {
            toggleNormalBgm.isOn = true;
            audioSource.clip = normalBGM;
            toggleNormalBgm.interactable = false;  // Nonaktifkan interaksi toggle normalBGM
        }
        else
        {
            toggleFufufafaBgm.isOn = true;
            audioSource.clip = fufufafaBGM;
            toggleFufufafaBgm.interactable = false;  // Nonaktifkan interaksi toggle fufufafaBGM
        }
        audioSource.Play();

        // Set up event listeners for toggles
        toggleNormalBgm.onValueChanged.AddListener(delegate { OnToggleChanged(toggleNormalBgm, normalBGM, toggleFufufafaBgm, 0); });
        toggleFufufafaBgm.onValueChanged.AddListener(delegate { OnToggleChanged(toggleFufufafaBgm, fufufafaBGM, toggleNormalBgm, 1); });

        // Set up event listeners for crosshair toggles
        for (int i = 0; i < croshair.Length; i++)
        {
            int index = i;
            croshair[i].onValueChanged.AddListener(delegate { OnCrosshairToggleChanged(index); });
        }

        // Load nilai SFX dan BGM dari PlayerPrefs untuk slider dan audio volume
        indexSfx = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default ke 1.0 jika tidak ada nilai tersimpan
        indexBgm = PlayerPrefs.GetFloat("BGMVolume", 1.0f);  // Default ke 1.0 jika tidak ada nilai tersimpan
        sfxSlider.value = indexSfx;
        bgmSlider.value = indexBgm;
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);

        sfx.volume = indexSfx;
        audioSource.volume = indexBgm;

        // Mengambil nilai croshair dari PlayerPrefs
        int savedCroshairIndex = PlayerPrefs.GetInt("Croshair", 0);  // Default ke 0 jika tidak ada nilai tersimpan
        if (savedCroshairIndex >= 0 && savedCroshairIndex < croshair.Length)
        {
            croshair[savedCroshairIndex].isOn = true;
            croshairImage.sprite = spritesCroshair[savedCroshairIndex];
            croshair[savedCroshairIndex].interactable = false; // Nonaktifkan interaksi toggle crosshair yang disimpan
        }
    }

    public void OnDifficultyClicked()
    {
        PanelActived(panelDifficulty.name, "START GAME");
    }
    public void OnCreditsClicked()
    {
        PanelActived(panelCredits.name, "CREDITS");
    }
    public void OnSettingsClicked()
    {
        PanelActived(panelSettings.name, "SETTINGS");
    }
    public void OnTutorialClicked()
    {
        panelTutorial.SetActive(true);
    }
    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnEnterGame(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void OnClickedInputName()
    {
        if (!string.IsNullOrEmpty(enterName.text))
        {
            PlayerPrefs.SetString("NamePlayer", enterName.text);
            nameProfil.text = enterName.text;
        }
        else
        {
            PlayerPrefs.SetString("NamePlayer", "Fulan");
            nameProfil.text = "Fulan";
        }
    }

    [Obsolete]
    public void CustomImage()
    {
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            StartCoroutine(LoadImage(path));
        });
    }

    [Obsolete]
    IEnumerator LoadImage(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);  // Membaca file gambar sebagai byte array
        Texture2D texture = new Texture2D(2, 2);  // Membuat objek Texture2D untuk menampung gambar

        texture.LoadImage(imageData);  // Memuat gambar dari byte array ke dalam texture

        // Mengonversi Texture2D ke Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Menampilkan gambar di Image
        fotoProfil.sprite = sprite;  // 'fotoProfil' adalah komponen Image Anda

        // Menyimpan gambar dalam PlayerPrefs sebagai Base64 string
        string base64Image = Convert.ToBase64String(imageData);
        PlayerPrefs.SetString("PlayerProfileImage", base64Image);

        yield return null;
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
            fotoProfil.sprite = sprite;
        }
    }
    void LoadPlayerName()
    {
        string playerName = PlayerPrefs.GetString("NamePlayer", "");

        if (!string.IsNullOrEmpty(playerName))
        {
            nameProfil.text = playerName;
        }
        else
        {
            nameProfil.text = "Profil";
        }
    }

    private void OnSfxSliderChanged(float value)
    {
        indexSfx = value;
        sfx.volume = indexSfx;  // Mengatur volume SFX sesuai nilai slider
        PlayerPrefs.SetFloat("SFXVolume", indexSfx);  // Simpan ke PlayerPrefs
    }

    private void OnBgmSliderChanged(float value)
    {
        indexBgm = value;
        audioSource.volume = indexBgm;  // Mengatur volume BGM sesuai nilai slider
        PlayerPrefs.SetFloat("BGMVolume", indexBgm);  // Simpan ke PlayerPrefs
    }
    // Fungsi untuk menangani perubahan pada toggle dan audio
    private void OnToggleChanged(Toggle toggle, AudioClip clip, Toggle otherToggle, int bgmIndex)
    {
        if (toggle.isOn)
        {
            audioSource.clip = clip;
            audioSource.Play();
            PlayerPrefs.SetInt("BGM", bgmIndex);

            // Nonaktifkan interaksi toggle yang sedang aktif, dan aktifkan kembali yang lain
            toggle.interactable = false;
            otherToggle.interactable = true;
            otherToggle.isOn = false;
        }
    }
    // Fungsi untuk menangani perubahan pada toggle crosshair
    private void OnCrosshairToggleChanged(int index)
    {
        if (croshair[index].isOn)
        {
            croshairImage.sprite = spritesCroshair[index];
            PlayerPrefs.SetInt("Croshair", index);

            // Nonaktifkan interaksi toggle crosshair yang sedang aktif
            croshair[index].interactable = false;

            // Aktifkan kembali interaksi toggle crosshair lainnya
            for (int i = 0; i < croshair.Length; i++)
            {
                if (i != index)
                {
                    croshair[i].isOn = false;
                    croshair[i].interactable = true;
                }
            }
        }
    }
    private void PanelActived(string panelActived, string namePanel)
    {
        panelDifficulty.SetActive(panelActived.Equals(panelDifficulty.name));
        panelSettings.SetActive(panelActived.Equals(panelSettings.name));
        panelCredits.SetActive(panelActived.Equals(panelCredits.name));

        namePanelTxt.text = namePanel;
    }
}