using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public enum DoctorState { Idle = 0, Listening = 1, Explaining = 2, Concerned = 3, Frustrated = 4 }

    private const string PARAM_TALKING = "isTalking";
    private const string PARAM_STATE = "doctorState";

    [SerializeField] private float autoIdleDelay = 2.5f;

    private AudioPlayer audioPlayer;
    private Animator animator;

    private readonly Queue<DoctorState> pendingStates = new Queue<DoctorState>();
    private readonly object stateLock = new object();
    private DoctorState currentState = DoctorState.Idle;
    private float pendingIdleTimer = -1f;

    void OnEnable()
    {
        RealtimeAPIWrapper.OnDoctorStateRequested += EnqueueStateString;
        RealtimeAPIWrapper.OnResponseDone += ScheduleAutoIdle;
    }

    void OnDisable()
    {
        RealtimeAPIWrapper.OnDoctorStateRequested -= EnqueueStateString;
        RealtimeAPIWrapper.OnResponseDone -= ScheduleAutoIdle;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void Update()
    {
        if (animator == null) return;

        animator.SetBool(PARAM_TALKING, audioPlayer != null && audioPlayer.IsAudioPlaying());

        DoctorState? next = null;
        lock (stateLock)
        {
            if (pendingStates.Count > 0) next = pendingStates.Dequeue();
        }

        if (next.HasValue && next.Value != currentState)
        {
            ApplyState(next.Value);
            pendingIdleTimer = -1f;
        }

        if (pendingIdleTimer > 0f)
        {
            pendingIdleTimer -= Time.deltaTime;
            if (pendingIdleTimer <= 0f)
            {
                if (currentState != DoctorState.Idle) ApplyState(DoctorState.Idle, autoTriggered: true);
                pendingIdleTimer = -1f;
            }
        }
    }

    private void ApplyState(DoctorState s, bool autoTriggered = false)
    {
        currentState = s;
        animator.SetInteger(PARAM_STATE, (int)s);
        Debug.Log($"[AnimationHandler] doctorState -> {s}{(autoTriggered ? " (auto-idle)" : "")}");
    }

    private void EnqueueStateString(string state)
    {
        if (System.Enum.TryParse<DoctorState>(state, true, out var parsed))
        {
            lock (stateLock) pendingStates.Enqueue(parsed);
        }
        else
        {
            Debug.LogWarning($"[AnimationHandler] unknown doctor state from LLM: {state}");
        }
    }

    private void ScheduleAutoIdle()
    {
        pendingIdleTimer = autoIdleDelay;
    }
}
