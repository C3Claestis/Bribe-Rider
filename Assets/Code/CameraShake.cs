using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0.5f;  // Durasi shake
    private float shakeMagnitude = 5f; // Intensitas shake
    private float dampingSpeed = 1.0f;   // Kecepatan meredam getaran agar lebih halus

    private Vector3 originalPosition; // Posisi awal kamera
    private float currentShakeDuration;

    void Start()
    {
        originalPosition = transform.localPosition; // Simpan posisi awal kamera
    }

    // Method untuk memulai efek kamera shake
    public void TriggerShake()
    {
        currentShakeDuration = shakeDuration; // Set durasi shake ke nilai awal
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        while (currentShakeDuration > 0)
        {
            // Buat posisi acak di sekitar posisi awal kamera berdasarkan shakeMagnitude
            Vector3 randomOffset = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            // Lerp posisi kamera untuk transisi yang lebih smooth ke randomOffset
            transform.localPosition = Vector3.Lerp(transform.localPosition, randomOffset, Time.deltaTime * dampingSpeed);

            currentShakeDuration -= Time.deltaTime;
            yield return null;
        }

        // Kembalikan kamera ke posisi awal setelah efek shake selesai
        transform.localPosition = originalPosition;
    }
}
