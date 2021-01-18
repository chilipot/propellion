using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class BemisCommsBehavior : MonoBehaviour
{

    private AudioSource bemisAudioSource;

    [CanBeNull] private static AudioClip persistentAudioClip;
    private static float? persistentAudioClipTime;

    private void Start()
    {
        bemisAudioSource = GetComponent<AudioSource>();
        if (persistentAudioClipTime.HasValue && persistentAudioClip != null)
        {
            bemisAudioSource.clip = persistentAudioClip;
            bemisAudioSource.time = persistentAudioClipTime.Value;
            bemisAudioSource.Play();
            persistentAudioClip = null;
            persistentAudioClipTime = null;
        }
    }

    public void SavePersistentVoiceline()
    {
        if (!PersistentVoicelinePlaying()) return;
        persistentAudioClip = bemisAudioSource.clip;
        persistentAudioClipTime = bemisAudioSource.time;
    }

    public bool PersistentVoicelinePlaying()
    {
        return bemisAudioSource.isPlaying &&
               VoicelineManager.PersistentVoicelineNames.Contains(bemisAudioSource.clip.name);
    }
}
