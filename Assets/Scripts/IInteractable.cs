public interface IInteractable
{
    // Teks yang akan ditampilkan di UI (misal: "[E] Bicara", "[E] Elus Kucing")
    string InteractionText { get; }

    // Mengecek apakah objek sedang/bisa diinteraksi
    bool CanInteract();

    // Memulai proses interaksi
    void Interact();
}