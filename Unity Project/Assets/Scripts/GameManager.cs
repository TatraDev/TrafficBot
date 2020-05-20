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
    public int bonus;
    public int time;
    private int overTime = 100;

    public bool reset = false;
    public bool starting = false;

    public TMP_Text timeText;
    public TMP_Text bonusText;
    public TMP_Text train_1StatusText;
    public TMP_Text train_2StatusText;

    private GameObject[] lineObjs;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<Line> lineScripts = new List<Line>();

    public GameObject trafficLightPrefab;
    private List<GameObject> trafficObjs = new List<GameObject>();
    private List<TrafficLight> trafficLights = new List<TrafficLight>();

    public List<Train> trains = new List<Train>();

    private Network network;
    void Start()
    {
        InvokeRepeating("AddOneBonus", 1, 1);
        InvokeRepeating("AddOneSecond", 1, 1);

        network = GetComponent<Network>();

        InitLines();
        network.Inst(this);
    }

    void Update()
    {
        timeText.text = "Time: " + time;
        bonusText.text = Convert.ToString(bonus);

        train_1StatusText.text = TrainInfo(trains[0]);
        train_1StatusText.color = trains[0].GetComponent<SpriteRenderer>().color;

        train_2StatusText.text = TrainInfo(trains[1]);
        train_2StatusText.color = trains[1].GetComponent<SpriteRenderer>().color;

        if (Input.GetKeyDown(KeyCode.R))
            ResetScene();

        if (overTime <= time)
            ResetScene();

        trains[1].Speed = 5;
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
                            trafficObjs.Add(tLObj);
                        }
                    }
                }
            }
        }
    }

    public string Info()
    {
        string sTrains = "";
        foreach (var train in trains)
        {
            sTrains += Convert.ToString((float)Math.Round(train.Speed / train.MaxSpeed, 4), CultureInfo.InvariantCulture) + ",";
        }

        return sTrains +
            bonus + "," +
            Convert.ToInt32(reset) + "," +
            SubInfo();
    }

    public string TrainInfo(Train train)
    {
        return "Train" +
            "\n   Distance: " +
            math.round(train.lineDistance * 100) +
            " m" +
            "\n   Position: " +
            train.GetDistanceToEnd() +
            " m" +
            "\n   Speed: " +
            train.Speed +
            " km/h" +
            "\n   Dist / speed * 60: " +
            train.lineDistance / 10 / train.Speed * 60 +
            " min" +
            "\n   Time end: " +
            train.timeToEnd +
            " min";
    }

    public string SubInfo()
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

    public void ResetScene()
    {
        reset = true;
        foreach (var train in trains)
        {
            Destroy(train.gameObject);
        }

        trains.Clear();

        foreach (Line script in lineScripts)
        {
            script.InstTrain();
            script.RandomisePointPosition();
        }

        foreach (var obj in trafficObjs)
        {
            Destroy(obj.gameObject);
        }

        trafficObjs.Clear();
        trafficLights.Clear();
        InstTrafficLight();

        time = 0;
    }

    public void AddBonus(int count)
    {
        if (starting)
            bonus += count;
    }

    public void AddOneBonus()
    {
        if (starting)
            bonus++;
    }

    private void AddOneSecond()
    {
        if (starting)
            time++;
    }

    public void SetTrainSpeed(int index, float speed)
    {
        trains[index].GetComponent<Train>().Speed = speed;
    }

    public void EnableScript()
    {
        Debug.Log(Application.dataPath);
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = Application.dataPath + "/TrafficBot/traffic_env.py";
        proc.Start();
    }
}
