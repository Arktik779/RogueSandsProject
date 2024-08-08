using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource hubAudioSource;
    public AudioSource combatAudioSource;

    private void Start()
    {
        PlayHubTrack();
    }

    public void PlayHubTrack()
    {
        if (combatAudioSource.isPlaying)
        {
            combatAudioSource.Stop();
        }
        hubAudioSource.Play();
    }

    public void PlayCombatTrack()
    {
        if (hubAudioSource.isPlaying)
        {
            hubAudioSource.Stop();
        }
        combatAudioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnterCombat"))
        {
            PlayCombatTrack();
        }
        else if (other.CompareTag("ExitCombat"))
        {
            PlayHubTrack();
        }
    }
}