using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Teleport : MonoBehaviour, EffectScripts
{
    private static readonly Dictionary<string, Teleport> registry = new Dictionary<string, Teleport>();

    [SerializeField] private string doorId;
    [SerializeField] private string destinationId;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int priority;
    [SerializeField] private float delayBeforeTeleport = 0.2f;
    [SerializeField] private float delayAfterTeleport = 0.2f;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera roomCamera;

    [Header("Animation Settings")]
    [SerializeField] private Animator loadingAnimator; // Assign Animator Layar/UI jika ada
    [SerializeField] private string loadingTriggerName = "Loading";

    public int Priority => priority;
    public string DoorId => doorId;
    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;
    public CinemachineVirtualCamera RoomCamera => roomCamera;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(doorId))
        {
            return;
        }

        if (registry.TryGetValue(doorId, out Teleport existing) && existing != this)
        {
            Debug.LogWarning("doorId duplikat: " + doorId + " di " + name + " dan " + existing.name);
        }

        registry[doorId] = this;
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(doorId) && registry.TryGetValue(doorId, out Teleport existing) && existing == this)
        {
            registry.Remove(doorId);
        }
    }

    public IEnumerator playEffects()
    {
        if (!registry.TryGetValue(destinationId, out Teleport destination))
        {
            Debug.LogWarning("Tujuan teleport tidak ditemukan: " + destinationId);
            yield break;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Object dengan tag Player tidak ditemukan.");
            yield break;
        }

        // 1. Cari Animator (prioritas Inspector, jika kosong ambil dari Player)
        Animator anim = loadingAnimator != null ? loadingAnimator : player.GetComponent<Animator>();

        // 2. Dipanggil HANYA SEKALI di sini
        if (anim != null)
        {
            anim.SetTrigger(loadingTriggerName);
        }

        // 3. Jeda waktu sebelum posisi dipindahkan (sesuai durasi transisi menutup layar)
        yield return new WaitForSeconds(delayBeforeTeleport);

        // 4. Pindahkan posisi Player
        Vector3 oldPosition = player.transform.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = destination.SpawnPosition;
        }

        player.transform.position = destination.SpawnPosition;

        RoomCameraSwitcher.SwitchTo(
            destination.RoomCamera,
            player.transform,
            destination.SpawnPosition - oldPosition
        );

        // 5. Jeda waktu setelah dipindahkan
        yield return new WaitForSeconds(delayAfterTeleport);
    }
}