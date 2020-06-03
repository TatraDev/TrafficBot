using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    private void Awake()
    {
        current = this;
    }

    public event Action onTrainTriggerEnter;
    public void TrainTriggerEnter()
    {
        onTrainTriggerEnter?.Invoke();
    }

    public event Action onGameRestart;
    public void GameRestart()
    {
        onGameRestart?.Invoke();
    }

    public event Action onAddLog;
    public void AddLog()
    {
        onAddLog?.Invoke();
    }

    public event Action onTimeChanged;
    public void TimeChanged()
    {
        onTimeChanged?.Invoke();
    }

    public event Action onBonusesChanged;
    public void BonusesChanged()
    {
        onBonusesChanged?.Invoke();
    }
}
