﻿using UnityEngine;
using TMPro;

public class Line : MonoBehaviour
{
    [SerializeField] private GameManager manager = default;

    [SerializeField] private GameObject trainPrefab = default;
    private GameObject train;

    [SerializeField] private GameObject pointPrefab = default;

    private LineRenderer line;

    private Color color;

    public bool randomPointPosition;

    public Vector2 from_1;
    public Vector2 to_1;

    public Vector2 from_2;
    public Vector2 to_2;
    void Awake()
    {
        line = GetComponent<LineRenderer>();
        color = line.colorGradient.colorKeys[0].color;

        RandomPointPosition();
        InstEndCaps();
    }

    public void RandomPointPosition()
    {
        if (randomPointPosition)
        {
            line.SetPosition(1,new Vector2(Random.Range(from_1.x, to_1.x), Random.Range(from_1.y, to_1.y)));
            line.SetPosition(2, new Vector2(Random.Range(from_2.x, to_2.x), Random.Range(from_2.y, to_2.y)));
            InstTrain();
            manager.trains.Add(train.GetComponent<Train>());
        }
    }

    void InstEndCaps()
    {
        GameObject start = Instantiate(pointPrefab);
        start.transform.parent = transform;
        start.transform.position = line.GetPosition(0);
        start.GetComponent<SpriteRenderer>().color = color;

        GameObject end = Instantiate(pointPrefab);
        end.transform.parent = transform;
        end.transform.position = line.GetPosition(line.positionCount - 1);
        end.GetComponent<SpriteRenderer>().color = color;
    }

    void InstTrain()
    {
        Color color = this.color;

        train = Instantiate(trainPrefab);
        train.transform.parent = transform;
        train.GetComponent<Train>().line = line;
        train.transform.position = line.GetPosition(0);

        color.r += 0.1f;
        color.g += 0.1f;
        color.b += 0.1f;

        train.GetComponent<SpriteRenderer>().color = color;
    }
}