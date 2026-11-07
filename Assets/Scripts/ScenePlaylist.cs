using UnityEngine;

public class ScenePlaylist : MonoBehaviour
{
    [SerializeField] private string[] playlistNames;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[ScenePlaylist] AudioManager tidak ada di scene!");
            return;
        }

        AudioManager.Instance.PlayPlaylist(playlistNames);
    }
}