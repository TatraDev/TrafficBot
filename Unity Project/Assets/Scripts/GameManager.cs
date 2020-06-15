using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    public List<Train> trains { get; private set; }
    public int bonuses { get; private set; }
    public int bonusFactor { get; private set; } = 1;

    public int time { get; private set; }
    private bool restart;

    private Coroutine timer;

    public bool isNetworkWorking;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        GameEvents.current.onTrainTriggerEnter += OnTrainCrash;
        GameEvents.current.onGameRestart += Restart;

        timer = StartCoroutine(Timer());
    }

    void Update()
    {
        trains[1].isLookAddBonuses = true;
        trains[1].Speed = 80;
        //trains[2].isLookAddBonuses = true;
        //trains[2].Speed = 40;
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            int seconds = 1;
            yield return new WaitForSecondsRealtime(seconds);
            time += seconds;
            GameEvents.current.TimeChanged();
        }
    }

    public void AddBonuses()
    {
        bonuses += 1 * bonusFactor;
        bonusFactor++;

        GameEvents.current.BonusesChanged();
    }

    public string Info()
    {
        string sTrains = trains.Count + ",";
        foreach (var train in trains)
        {
            sTrains += Convert.ToString((float)Math.Round(train.GetDistanceToIntersection() / train.trackDistance, 3), CultureInfo.InvariantCulture) + ",";

            if (!train.isMoveBack) sTrains += Convert.ToString(
                (float)Math.Round(train.distanceToEnd / train.trackDistance, 3), 
                CultureInfo.InvariantCulture) + ",";
            else sTrains += Convert.ToString(
                (float)Math.Round(1 - train.distanceToEnd / train.trackDistance, 3), 
                CultureInfo.InvariantCulture) + ",";
        }

        return sTrains +
            Mathf.RoundToInt(bonuses) + "," +
            Convert.ToInt32(restart);
    }

    private void OnTrainCrash()
    {
        restart = true;
    }

    private void Restart()
    {
        if (timer != null)
            StopCoroutine(timer);

        timer = StartCoroutine(Timer());

        restart = false;
        bonuses = 0;
        bonusFactor = 1;

        GameEvents.current.BonusesChanged();

        time = 0;
    }

    public void SetTrains(List<Train> trains)
    {
        this.trains = trains;
    }
}
