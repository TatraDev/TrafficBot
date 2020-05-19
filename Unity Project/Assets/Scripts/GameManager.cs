using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LineSegmentsIntersection;
using Unity.Mathematics;
using System;
using System.Globalization;


public class GameManager : MonoBehaviour
{
    public int bonus;

    private int overTime = 100;
    public int time;

    public bool reset = false;
    public bool starting = false;

    public TMP_Text timeText;
    public TMP_Text bonusText;
    public TMP_Text train_1StatusText;
    public TMP_Text train_2StatusText;

    private GameObject[] lineObjs;

    public List<LineRenderer> lines;
    public List<Line> lineScripts;
    public List<GameObject> trafficObjs;

    public Vector2[] intersections;

    public List<Train> trains;

    public GameObject trafficLightPrefab;

    private NetworkCon network;
    void Start()
    {
        InitLines();
        InvokeRepeating("AddOneBonus", 1, 1);
        InvokeRepeating("AddOneTime", 1, 1);

        network = GetComponent<NetworkCon>();
        network.trains = trains;
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
    }

    void InitLines()
    {
        lineObjs = GameObject.FindGameObjectsWithTag("Path");

        foreach (GameObject line in lineObjs)
        {
            lines.Add(line.GetComponent<LineRenderer>());
            lineScripts.Add(line.GetComponent<Line>());
        }

        InstTrafficLight();
    }

    void InstTrafficLight()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = i + 1; j < lines.Count; j++)
            {
                for (int x = 1; x < lines[i].positionCount; x++)
                {
                    for (int y = 1; y < lines[j].positionCount; y++)
                    {
                        if (Math2d.LineSegmentsIntersection(lines[i].GetPosition(x - 1), lines[i].GetPosition(x), lines[j].GetPosition(y - 1), lines[j].GetPosition(y), out Vector2 intersection))
                        {
                            GameObject tLObj = Instantiate(trafficLightPrefab);

                            tLObj.GetComponent<TrafficLight>().lines.Add(lineObjs[i]);
                            tLObj.GetComponent<TrafficLight>().lines.Add(lineObjs[j]);
                            tLObj.transform.position = intersection;

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
        foreach (Train train in trains)
        {
            sTrains += Convert.ToString((float)Math.Round(train.Speed / train.MaxSpeed, 4), CultureInfo.InvariantCulture) + ",";
        }

        return sTrains +
            bonus + "," +
            Convert.ToInt32(reset);
    }

    public string TrainInfo(Train train)
    {
        return "Train" +
            "\n   Distance: " +
            math.round(train.LineDistance * 100) +
            " m" +
            "\n   Position: " +
            train.GetDistanceToEnd() +
            " m" +
            "\n   Speed: " +
            train.Speed +
            " km/h" +
            "\n   Dist / speed * 60: " +
            train.LineDistance / 10 / train.Speed * 60 +
            " min" +
            "\n   Time end: " +
            train.TimeToEnd +
            " min";
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
        InstTrafficLight();

        time = 0;

        network.trains = trains;
    }

    public void AddBonus(int count)
    {
        if (starting)
            bonus += count;
    }

    public void AddOneBonus()
    {
        if (starting)
            bonus ++;
    }

    private void AddOneTime()
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
