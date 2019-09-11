#pragma warning disable 0649
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerDeathCamera : MonoBehaviour {
    private CameraController mainCamera;
    [SerializeField] private string DeathTag;
    [SerializeField] private CinemachineVirtualCamera deathCameraStart;
    [SerializeField] private CinemachineVirtualCamera deathCameraEnd;

    private void Start() {
        mainCamera = GetComponent<CameraController>();

        if(!deathCameraStart || !deathCameraEnd) {
            Debug.LogWarning("No death cameras assigned");
            return;
        }
        deathCameraStart.enabled = false;

        PlayerEngine player = FindObjectOfType<PlayerEngine>();
        if(player) {
            player.OnPlayerDeath += PlayerDied;
            player.OnPlayerRespawn += PlayerRespawned;
        }
    }

    private void PlayerDied() {
        if(!deathCameraStart) {
            return;
        }

        deathCameraStart.GetComponent<PlayableDirector>().Play();
    }

    private void PlayerRespawned() {
        if(!deathCameraStart || !deathCameraEnd) {
            return;
        }

    }

    private void OnDisable() {
        PlayerEngine player = FindObjectOfType<PlayerEngine>();
        if(player) {
            player.OnPlayerDeath -= PlayerDied;
            player.OnPlayerRespawn -= PlayerRespawned;
        }
    }
}