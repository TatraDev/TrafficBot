using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using System;

public class Train : MonoBehaviour
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 nextPoint;

    private int nextPointIndex;

    public LineRenderer line;

    private float speed = 6f;
    public float Speed
    {
        get { return speed; }
        set
        {
            if (value <= 0) speed = 0;
            else if (value >= 120) speed = 120;
            else speed = value;
        }
    }
    public float MaxSpeed
    {
        get { return 120; }
    }

    private List<Vector2> path;
    private int pathPositionCount;

    private float distance = 0;
    public float PathDistance
    {
        get { return distance; }
    }

    private float trainPos = 0;
    public float TrainPosition
    {
        get { return trainPos; }
    }

    private float startTime;

    private float timeToEnd;
    public float TimeToEnd
    {
        get { return timeToEnd; }
    }

    private Rigidbody2D rb;

    private GameManager manager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        pathPositionCount = line.positionCount;

        path = new List<Vector2>();

        for (int i = 0; i < pathPositionCount; i++)
        {
            Vector3 point = line.GetPosition(i);

            if (i != 0)
            {
                distance += (point - line.GetPosition(i - 1)).magnitude;
            }

            path.Add(point);
        }

        startPoint = path[0];
        transform.position = startPoint;

        nextPointIndex = 1;
        nextPoint = path[nextPointIndex];
        endPoint = path[pathPositionCount - 1];

        startTime = Time.timeSinceLevelLoad;
    }

    void FixedUpdate()
    {
        SetTrainPosition();
        Movement();
    }

    private void Movement()
    {
        if (rb.position == endPoint)
        {
            timeToEnd = Time.timeSinceLevelLoad - startTime;
            startTime = Time.timeSinceLevelLoad;
            nextPointIndex = 0;
            path.Reverse();
            startPoint = path[0];
            endPoint = path[pathPositionCount - 1];
        }
        else if (rb.position == nextPoint)
        {
            nextPointIndex++;
            nextPoint = path[nextPointIndex];
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, nextPoint, speed / 6 * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        float angleZ = math.atan2(transform.position.y - nextPoint.y, transform.position.x - nextPoint.x);
        angleZ += 90 * math.PI / 180;

        Vector3 rot = Vector3.forward * angleZ;

        transform.rotation = quaternion.Euler(rot);
    }

    private void SetTrainPosition() 
    {
        trainPos = 0;

        for (int i = nextPointIndex; i < path.Count; i++)
        {
            Vector3 point = path[i];

            if (i != nextPointIndex)
            {
                trainPos += (point - new Vector3(path[i - 1].x, path[i - 1].y, 1)).magnitude;
            }
            else if (i == nextPointIndex)
            {
                trainPos += (transform.position - point).magnitude;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        manager.ResetScene();
    }

    public float GetDistanceToEnd()
    {
        return (float) Math.Round(PathDistance * 100 - TrainPosition * 100, 2);
    }

    public float GetPositionInPercent()
    {
        return math.round((100 - (trainPos / distance * 100)));
    }
}
