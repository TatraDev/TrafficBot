using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LineSegmentsIntersection;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    private int bonus;
    private int time;
    private int overTime = 100;
    private int runsCount = 0;

    public bool reset = false;
    public bool isReady = false;
    public bool isTrainsCrash = false;

    public TMP_Text timeText;
    public TMP_Text bonusText;
    public TMP_Text bonusesAddedText;
    public TMP_Text resetLable;
    public TMP_Text trainStatusText;
    public TMP_Text log;

    private GameObject[] lineObjs;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<Line> lineScripts = new List<Line>();

    [SerializeField]
    private GameObject trafficLightPrefab;

    private List<GameObject> trafficLightObjs = new List<GameObject>();
    private List<TrafficLight> trafficLights = new List<TrafficLight>();

    public List<Train> trains = new List<Train>();

    private Network network;

    [SerializeField]
    private AnimationCurve TextCurve;

    void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }

        main = this;

        bonus = 0;
        time = 0;
    }

    void Start()
    {
        InvokeRepeating("AddOneBonus", 1, 1);
        InvokeRepeating("AddOneSecond", 1, 1);

        InitLines();

        isReady = true;

        network = GetComponent<Network>();
        network.Inst();
    }

    void Update()
    {
        trainStatusText.text = "";
        for (int i = 0; i < trains.Count; i++)
        {
            string color = ColorUtility.ToHtmlStringRGB(trains[i].GetComponent<SpriteRenderer>().color);
            trainStatusText.text += "<color=#" + color + ">";
            trainStatusText.text += TrainInfo(trains[i], i) + "\n\n";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScore();
            ResetScene();
        }

        if (Input.GetKeyDown(KeyCode.T))
            time = 95;

        if (overTime <= time)
        {
            ResetScene("Time Over");
        }

        trains[1].Speed = 50;
        trains[1].endBonuses = 0;
    }

    void InitLines()
    {
        lineObjs = GameObject.FindGameObjectsWithTag("Path");

        foreach (GameObject line in lineObjs)
        {
            lineRenderers.Add(line.GetComponent<LineRenderer>());
            lineScripts.Add(line.GetComponent<Line>());
        }

        InstTrafficLight();
    }

    void InstTrafficLight()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            for (int j = i + 1; j < lineRenderers.Count; j++)
            {
                for (int x = 1; x < lineRenderers[i].positionCount; x++)
                {
                    for (int y = 1; y < lineRenderers[j].positionCount; y++)
                    {
                        if (Math2d.LineSegmentsIntersection(lineRenderers[i].GetPosition(x - 1), lineRenderers[i].GetPosition(x), lineRenderers[j].GetPosition(y - 1), lineRenderers[j].GetPosition(y), out Vector2 intersection))
                        {
                            GameObject tLObj = Instantiate(trafficLightPrefab);

                            List<GameObject> tLObjs = new List<GameObject>();
                            tLObjs.Add(lineObjs[i]);
                            tLObjs.Add(lineObjs[j]);

                            tLObj.transform.position = new Vector3(intersection.x, intersection.y, 1);

                            tLObj.GetComponent<TrafficLight>().Init(tLObjs, x - 1, y - 1);

                            trafficLights.Add(tLObj.GetComponent<TrafficLight>());
                            trafficLightObjs.Add(tLObj);
                        }
                    }
                }
            }
        }
    }

    public string Info()
    {
        string sTrains = trains.Count + ",";
        foreach (var train in trains)
        {
            sTrains += Convert.ToString((float)Math.Round(train.Speed / train.maxSpeed, 4), CultureInfo.InvariantCulture) + ",";
        }

        return sTrains +
            bonus + "," +
            Convert.ToInt32(reset) + "," +
            TrafficLightsInfo();
    }

    public string TrainInfo(Train train, int index)
    {
        return "Train_" +
            index +
            "\n   Distance: " +
            math.round(train.lineDistance * 100) +
            " m" +
            "\n   Position: " +
            train.GetDistanceToEnd() +
            " m" +
            "\n   Speed: " +
            (float)Math.Round(train.Speed, 2) +
            " km/h" +
            "\n   Time on end: " +
            train.timeToEnd +
            " min";
    }

    public string TrafficLightsInfo()
    {
        string info = "";

        foreach (var tLight in trafficLights)
        {
            info += Convert.ToString((float)Math.Round(tLight.firstTrainPosition, 3), CultureInfo.InvariantCulture) + ",";
            info += Convert.ToString((float)Math.Round(tLight.firstListLengthBefore, 3), CultureInfo.InvariantCulture) + ",";
            info += Convert.ToString((float)Math.Round(tLight.secondTrainPosition, 3), CultureInfo.InvariantCulture) + ",";
            info += Convert.ToString((float)Math.Round(tLight.secondListLengthBefore, 3), CultureInfo.InvariantCulture) + ",";
        }
        info = info.Remove(info.Length - 1);

        return info;
    }

    public IEnumerator BonusedAddedInfo(int countAdd)
    {
        bonusesAddedText.text = "+ " + countAdd;
        yield return new WaitForSeconds(0.4f);
        bonusesAddedText.text = "";
    }

    private IEnumerator ResetInfo(string cause)
    {
        resetLable.text = cause + "\n" + bonus + " bonuses received";
        log.text += "Run:  " + runsCount + ". Time: " + time + ". " + cause + ". " + bonus + " bonuses received" + ";\n";
        runsCount++;

        for (float i = 0; i < 1; i += Time.deltaTime / 3)
        {
            resetLable.alpha = Mathf.Lerp(0f, 0.6f, TextCurve.Evaluate(i));

            yield return null;
        }

        resetLable.alpha = 0;
    }

    private void ResetObjectsAndСhangeLines()
    {
        isReady = false;

        reset = true;
        foreach (var train in trains)
        {
            Destroy(train.gameObject);
        }

        trains.Clear();

        foreach (var lineScript in lineScripts)
        {
            lineScript.InstTrain();
            lineScript.RandomisePointPosition();
        }

        foreach (var trafficLightObj in trafficLightObjs)
        {
            Destroy(trafficLightObj.gameObject);
        }

        trafficLightObjs.Clear();
        trafficLights.Clear();
        InstTrafficLight();

        isReady = true;
    }

    public void ResetScore()
    {
        bonus = 0;
        time = 0;
    }

    public void ResetScene()
    {
        ResetObjectsAndСhangeLines();
        time = 0;
    }

    public void ResetScene(string cause)
    {
        StartCoroutine(ResetInfo(cause));
        ResetObjectsAndСhangeLines();
        time = 0;
    }

    public void AddBonus(int count)
    {
        if (count > 0)
        {
            StopCoroutine("BonusedAddedInfo");
            StartCoroutine(BonusedAddedInfo(count));
            bonus += count;
            bonusText.text = Convert.ToString(bonus);
        }
    }

    private void AddOneBonus()
    {
        StartCoroutine(BonusedAddedInfo(1));
        bonus++;
        bonusText.text = Convert.ToString(bonus);

    }

    private void AddOneSecond()
    {
        time++;
        timeText.text = "Time: " + time;
    }

    public void EnableScript()
    {
        ResetScene();
        Debug.Log(Application.dataPath);
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = Application.dataPath + "/TrafficBot/traffic_env.py";
        proc.Start();
    }
}
