using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public enum DoctorState { Idle = 0, Listening = 1, Explaining = 2, Concerned = 3, Frustrated = 4 }

    // Animator parameter names — must exist in the AvatarControllerExample.controller
    private const string PARAM_TALKING = "isTalking";
    private const string PARAM_STATE = "doctorState";

    private AudioPlayer audioPlayer;
    private Animator animator;

    private readonly Queue<DoctorState> pendingStates = new Queue<DoctorState>();
    private readonly object stateLock = new object();
    private DoctorState currentState = DoctorState.Idle;

    void OnEnable()
    {
        RealtimeAPIWrapper.OnDoctorStateRequested += EnqueueStateString;
        RealtimeAPIWrapper.OnResponseDone += EnqueueIdle;
    }

    void OnDisable()
    {
        RealtimeAPIWrapper.OnDoctorStateRequested -= EnqueueStateString;
        RealtimeAPIWrapper.OnResponseDone -= EnqueueIdle;
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
            currentState = next.Value;
            animator.SetInteger(PARAM_STATE, (int)currentState);
            Debug.Log($"[AnimationHandler] doctorState -> {currentState}");
        }
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

    private void EnqueueIdle()
    {
        lock (stateLock) pendingStates.Enqueue(DoctorState.Idle);
    }
}
