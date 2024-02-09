using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Header("Sounds")]
    public Sound[] sounds;

    [Header("References")]
    public GameObject audioSourcePrefab;

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            /*s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * volume;
            s.source.pitch = s.pitch * pitch;
            s.source.loop = s.loop;*/
        }
    }

    public void Play(string name)
    {
        Play(name, new Vector3(0f, 0f, 0f), 0f);
    }

    public void Play(string name, Vector3 position, float spatialBlend = 1f)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Could not find sound " + name);
            return;
        }
        else
        {
            AudioSource source = Instantiate(audioSourcePrefab, position, Quaternion.identity).GetComponent<AudioSource>();
            source.clip = s.clip;
            source.volume = s.volume * volume;
            source.pitch = s.pitch * pitch;
            source.loop = s.loop;
            source.spatialBlend = spatialBlend;
            source.Play();
            StartCoroutine(DestroyAudioSource(source.gameObject, source.clip.length));
        }
    }
    public IEnumerator DestroyAudioSource(GameObject audioSource, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(audioSource);
    }
}
