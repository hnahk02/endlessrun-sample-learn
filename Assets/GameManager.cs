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




}

public abstract class GameState : MonoBehaviour
{
    [HideInInspector]
    public GameManager manager;
    public abstract void Enter(GameState from);
    public abstract void Exit(GameState to);
    public abstract void Update();
    public abstract string GetName();

}
