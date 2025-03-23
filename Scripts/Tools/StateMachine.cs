using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    [System.Serializable]
    public class StateEntry
    {
        public string stateName;
        public List<string> subStates = new List<string>();
    }

    [Header("State Configuration")]
    [SerializeField] private List<StateEntry> states = new List<StateEntry>();
    [SerializeField] private string initialState;
    [SerializeField] private string initialSubState;

    [Header("Current State")]
    [SerializeField] private string currentState;
    [SerializeField] private string currentSubState;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<string> OnStateChanged;
    public UnityEngine.Events.UnityEvent<string> OnSubStateChanged;

    private Dictionary<string, List<string>> stateDictionary = new Dictionary<string, List<string>>();

    private void Awake()
    {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateDictionary.Clear();
        foreach (var state in states)
        {
            stateDictionary[state.stateName] = new List<string>(state.subStates);
        }

        SetState(initialState);
        SetSubState(initialSubState);
    }

    public void SetState(string newState)
    {
        if (!stateDictionary.ContainsKey(newState) || currentState == newState) return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState);
        
        // Automatically reset substate if invalid for new state
        if (!IsValidSubState(currentSubState))
            SetSubState("");
    }

    public void SetSubState(string newSubState)
    {
        if (!IsValidSubState(newSubState) || currentSubState == newSubState) return;

        currentSubState = newSubState;
        OnSubStateChanged?.Invoke(currentSubState);
    }

    public bool IsValidSubState(string subState)
    {
        return stateDictionary[currentState].Contains(subState);
    }

    public string GetCurrentState() => currentState;
    public string GetCurrentSubState() => currentSubState;

    // Editor helper methods
    public void AddNewState(string stateName)
    {
        states.Add(new StateEntry { stateName = stateName });
    }

    public void AddSubStateToCurrent(string subState)
    {
        if (states.Exists(s => s.stateName == currentState))
        {
            states.Find(s => s.stateName == currentState).subStates.Add(subState);
        }
    }
}