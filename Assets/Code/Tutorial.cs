using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<TutorialProperty> properties = new List<TutorialProperty>();

    [SerializeField] Image imageValue;
    [SerializeField] Text indexNumber;
    [SerializeField] Text indexTittle;
    [SerializeField] Text indexIsi;

    private int currentIndex = 0; // Menyimpan index saat ini

    void Start()
    {
        // Menampilkan data pertama pada daftar
        Manage(properties[currentIndex].sprite, properties[currentIndex].index, properties[currentIndex].tittle, properties[currentIndex].isiDeskripsi);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    void Manage(Sprite image, int index, string tittle, string Isi)
    {
        imageValue.sprite = image;
        indexNumber.text = index.ToString();
        indexTittle.text = tittle;  
        indexIsi.text = Isi;
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }
    public void Next()
    {
        // Tambahkan index jika belum mencapai batas akhir
        if (currentIndex < properties.Count - 1)
        {
            currentIndex++;
            Manage(properties[currentIndex].sprite, properties[currentIndex].index, properties[currentIndex].tittle, properties[currentIndex].isiDeskripsi);
        }
    }

    public void Prev()
    {
        // Kurangi index jika belum mencapai batas awal
        if (currentIndex > 0)
        {
            currentIndex--;
            Manage(properties[currentIndex].sprite, properties[currentIndex].index, properties[currentIndex].tittle, properties[currentIndex].isiDeskripsi);
        }
    }
}

[System.Serializable]
public class TutorialProperty
{
    public Sprite sprite;
    public int index;
    public string tittle;
    [TextArea(1, 10)] public string isiDeskripsi;
}
