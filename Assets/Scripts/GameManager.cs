using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LineSegmentsIntersection;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public int bonus;

    private int overTime = 100;
    public int time;

    public bool reset = false;

    public TMP_Text timeText;
    public TMP_Text bonusText;
    public TMP_Text trainStatusText;

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
        InvokeRepeating("PlusBonus", 1, 1);
        InvokeRepeating("PlusTime", 1, 1);

        network = GetComponent<NetworkCon>();
        network.trains = trains;
    }

    void Update()
    {
        timeText.text = "Time: " + time;
        bonusText.text = Convert.ToString(bonus);
        trainStatusText.text = TrainInfo(trains[0]);
        trainStatusText.color = trains[0].GetComponent<SpriteRenderer>().color;

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
        return Convert.ToString(trains[0].Speed / trains[0].MaxSpeed, CultureInfo.InvariantCulture) + "," + Convert.ToString(trains[1].Speed / trains[1].MaxSpeed, CultureInfo.InvariantCulture) + "," + bonus + "," + Convert.ToInt32(reset);
    }

    public string TrainInfo(Train train)
    { 
            return "Train" +
            "\n   Distance: " +
            math.round(train.PathDistance * 100) +
            " m" +
            "\n   Position: " +
            train.GetDistanceToEnd() +
            " m" +
            "\n   Speed: " +
            train.Speed +
            " km/h" +
            "\n   Dist / speed * 60: " +
            train.PathDistance / 10 / train.Speed * 60 +
            " min" +
            "\n   Time end: " +
            train.TimeToEnd +
            " min";
    }

    public void ResetScene()
    {
        reset = true;
        foreach (var obj in trains)
        {
            Destroy(obj.gameObject);
        }

        trains.Clear();

        foreach (Line script in lineScripts)
        {
            script.RandomPointPosition();      
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

    private void PlusBonus()
    {
        bonus++;
    }

    private void PlusTime()
    {
        time++;
    }

    public void SetTrainSpeed(int index, float speed)
    {
        trains[index].GetComponent<Train>().Speed = speed;
    }
}
