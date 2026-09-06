using System.Collections;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public AudioClip walkSound;
    public AudioClip sprintSound;
    public AudioClip landingSound;
    public AudioClip damageSound;

    public float footstepVolume = 0.4f;
    public float landingVolume = 0.8f;

    public float maxFootstepDuration = 0.3f;
    public float walkStepInterval = 0.45f;
    public float sprintStepInterval = 0.28f;

    private float stepTimer;
    private CharacterController controller;
    private bool wasGrounded;
    private Coroutine stopAudioCoroutine;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.playOnAwake = false;

        if (controller != null)
        {
            wasGrounded = controller.isGrounded;
        }
    }

    private void Update()
    {
        HandleAudioLogic();
    }

    private void HandleAudioLogic()
    {
        bool isGrounded = controller != null ? controller.isGrounded : true;

        // --- LANDING SOUND (Bina kisi duration cut-off ke poori chalegi) ---
        if (isGrounded && !wasGrounded)
        {
            if (landingSound != null)
            {
                PlayFullAudio(landingSound, landingVolume);
            }
            stepTimer = 0.1f;
        }

        // --- FOOTSTEPS SOUNDS ---
        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);

        if (isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                AudioClip clipToPlay = isSprinting ? sprintSound : walkSound;
                float interval = isSprinting ? sprintStepInterval : walkStepInterval;

                if (clipToPlay != null)
                {
                    PlayTimedAudio(clipToPlay, footstepVolume, maxFootstepDuration);
                    stepTimer = interval;
                }
            }
        }
        else
        {
            if (audioSource.isPlaying && (audioSource.clip == walkSound || audioSource.clip == sprintSound))
            {
                StopAudioImmediate();
            }
            stepTimer = 0f;
        }

        wasGrounded = isGrounded;
    }

    // Footsteps ke liye timed audio player (0.3s par cut)
    private void PlayTimedAudio(AudioClip clip, float volume, float duration)
    {
        StopAudioImmediate();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        stopAudioCoroutine = StartCoroutine(StopAudioAfterDuration(duration));
    }

    // Landing / Damage sound ke liye full audio player (Bina kisi cut ke)
    private void PlayFullAudio(AudioClip clip, float volume)
    {
        StopAudioImmediate();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    private IEnumerator StopAudioAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
    }

    private void StopAudioImmediate()
    {
        if (stopAudioCoroutine != null)
        {
            StopCoroutine(stopAudioCoroutine);
            stopAudioCoroutine = null;
        }
        audioSource.Stop();
    }

    public void PlayDamageSound()
    {
        if (damageSound != null)
        {
            PlayFullAudio(damageSound, 1f);
        }
    }
}