using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            return s_Instance;
        }

    }
    protected static GameManager s_Instance;

    public GameState[] states;
    protected List<GameState> m_StateStack = new List<GameState>();
    protected Dictionary<string, GameState> m_StateDict = new Dictionary<string, GameState>();

    public GameState topState
    {
        get
        {
            if (m_StateStack.Count == 0) return null;
            return m_StateStack[m_StateStack.Count - 1];
        }
    }
    private void Start()
    {
        s_Instance = this;
    }

    protected void OnEnable()
    {
        if (states.Length == 0) return;

        for (int i = 0; i < states.Length; i++)
        {
            states[i].manager = this;
            m_StateDict.Add(states[i].GetName(), states[i]);
        }

        m_StateStack.Clear();
    }

    protected void Update()
    {
        if (m_StateStack.Count > 0)
        {
            m_StateStack[m_StateStack.Count - 1].Tick();
        }
    }

    public void SwitchState(string newState)
    {
        GameState state = FindState(newState);
        if (state == null)
        {
            Debug.LogError("Can't find the state named " + newState);
            return;
        }

        m_StateStack[m_StateStack.Count - 1].Exit(state);
        state.Enter(m_StateStack[m_StateStack.Count - 1]);
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
        m_StateStack.Add(state);
    }

    public GameState FindState(string stateName)
    {
        GameState state;
        if (!m_StateDict.TryGetValue(stateName, out state))
        {
            return null;
        }

        return state;
    }

    public void PushState(string name)
    {
        GameState state;
        if (!m_StateDict.TryGetValue(name, out state))
        {
            Debug.LogError("Can't find the state named " + name);
            return;
        }

        if (m_StateStack.Count > 0)
        {
            m_StateStack[m_StateStack.Count - 1].Exit(state);
            m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 1]);
        }
        else
        {
            state.Enter(null);
        }

        m_StateStack.Add(state);
    }

    public void PopState()
    {
        if (m_StateStack.Count < 2)
        {
            Debug.LogError("Can't pop states because there must have one state in stack");
        }

        m_StateStack[m_StateStack.Count - 1].Exit(m_StateStack[m_StateStack.Count - 2]);
        m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 2]);
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
    }




}

public abstract class GameState : MonoBehaviour
{
    [HideInInspector]
    public GameManager manager;
    public abstract void Enter(GameState from);
    public abstract void Exit(GameState to);
    public abstract void Tick();
    public abstract string GetName();

}
