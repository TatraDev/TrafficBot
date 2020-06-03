using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    private GameManager gameManager;

    public TMP_Text timeLable;
    public TMP_Text bonusesLable;
    public TMP_Text popupLable;
    public TMP_Text trainInfoLable;
    public TMP_Text logLable;
    public Scrollbar logScrollbar;
    private Coroutine popup;
    public AnimationCurve popupCurve;

    private void Start()
    {
        GameEvents.current.onAddLog += AddLog;
        GameEvents.current.onTimeChanged += TimeInfo;
        GameEvents.current.onBonusesChanged += BonusesInfo;
        gameManager = GameManager.main;
    }

    private void TimeInfo()
    {
        timeLable.text = $"Time: {gameManager.time}";
    }

    private void BonusesInfo()
    {
        bonusesLable.text = $"{gameManager.bonuses} x{gameManager.bonusFactor}";
    }

    private void Update()
    {
        TrainsInfo();
    }

    private string TrainInfo(int index, Train train)
    {
        return $"Train[{index}]\n" +
            $"Track distance: {train.trackDistance:F2}\n" +
            $"Position: {train.distanceToEnd:F2}\n" +
            $"Speed: {train.Speed:F2}";
    }

    private void TrainsInfo()
    {
        trainInfoLable.text = "";
        for (int i = 0; i < gameManager.trains.Count; i++)
        {
            string color = ColorUtility.ToHtmlStringRGB(gameManager.trains[i].GetComponent<SpriteRenderer>().color);
            trainInfoLable.text += $"<color=#{color}>";
            trainInfoLable.text += TrainInfo(i, gameManager.trains[i]) + "\n\n";
        }
    }

    private void AddLog()
    {
        PopupText($"Restart!\n{gameManager.bonuses} bonuses received");
        Log();
    }

    private void Log()
    {
        string text = $"Time: {gameManager.time}, {gameManager.bonuses} bonuses received;\n";
        logLable.text += text;

        if (logLable.text.Length > 2000)
        {
            logLable.text = logLable.text.Remove(0, text.Length - 1);
        }

        logScrollbar.value = 0;
    }

    private IEnumerator Popup(TMP_Text tMP_Text, string text)
    {
        tMP_Text.text = text;

        for (float i = 0; i < 1; i += Time.deltaTime / 2.5f)
        {
            tMP_Text.alpha = Mathf.Lerp(0f, 0.6f, popupCurve.Evaluate(i));

            yield return null;
        }

        tMP_Text.alpha = 0;
    }

    public void PopupText(string text)
    {
        if (popup != null)
            StopCoroutine(popup);

        popup = StartCoroutine(Popup(popupLable, text));
    }
}
