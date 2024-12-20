using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class FileDownloader : MonoBehaviour
{
    private bool isNotifActive = false;
    private string fileName = "BribeRiderCredits.pdf"; // Nama file yang ingin di-download
    [SerializeField] GameObject notifAnim;

    public void SaveFileToComputer()
    {
        // Path file di StreamingAssets
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // Path tujuan untuk menyimpan file di komputer player
        string targetPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), // Misalnya Desktop
            fileName
        );

        try
        {
            // Baca file dari StreamingAssets
            byte[] fileData;

#if UNITY_EDITOR || UNITY_STANDALONE
            fileData = File.ReadAllBytes(sourcePath); // Baca langsung dari file lokal
#else
            // Untuk platform mobile/other, gunakan UnityWebRequest
            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(sourcePath))
            {
                www.SendWebRequest();
                while (!www.isDone) { }

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    fileData = www.downloadHandler.data;
                }
                else
                {
                    Debug.LogError($"Error loading file: {www.error}");
                    return;
                }
            }
#endif

            // Simpan file ke targetPath
            File.WriteAllBytes(targetPath, fileData);
            ShowNotif("Download Complete In Desktop");

            Debug.Log($"File berhasil disimpan ke: {targetPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Gagal menyimpan file: {ex.Message}");
        }
    }
    private void ShowNotif(string message)
    {
        if (!isNotifActive)
        {
            Text teks = notifAnim.transform.GetChild(1).GetComponent<Text>();
            teks.text = message;
            StartCoroutine(NotifPopUp());
        }
    }
    IEnumerator NotifPopUp()
    {
        isNotifActive = true;
        notifAnim.SetActive(true);
        yield return new WaitForSeconds(2f);
        notifAnim.SetActive(false);
        isNotifActive = false;
    }
}
