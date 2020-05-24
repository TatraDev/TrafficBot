using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;

public class Train : MonoBehaviour
{
    private Vector2 endPoint;
    private Vector2 nextPoint;
    public int endBonuses = 5;

    private int nextPointIndex;

    public float maxSpeed { get; private set; } = 120;

    private float speed = 5f;
    public float Speed
    {
        get { return speed; }
        set
        {
            if (value <= 0) speed = 0;
            else if (value >= maxSpeed) speed = maxSpeed;
            else speed = value;
        }
    }

    private List<Vector2> linePoints = new List<Vector2>();

    public float lineDistance { get; private set; } = 0;
    public float trainPosition { get; private set; } = 0;

    private float startTime;
    public float timeToEnd { get; private set; }

    private LineRenderer line;
    private GameManager manager;
    private Rigidbody2D rb;

    private bool revers = false;

    private void Start()
    {
        line = GetComponentInParent<LineRenderer>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < line.positionCount; i++)
        {
            Vector3 point = line.GetPosition(i);

            if (i != 0)
            {
                lineDistance += (point - line.GetPosition(i - 1)).magnitude;
            }

            linePoints.Add(point);
        }

        transform.position = linePoints[0];

        nextPointIndex = 1;
        nextPoint = linePoints[nextPointIndex];
        endPoint = linePoints[line.positionCount - 1];

        startTime = Time.timeSinceLevelLoad;
    }

    private void FixedUpdate()
    {
        SetTrainPosition();
        Movement();
        Rotation();
    }

    private void Movement()
    {
        if (rb.position == endPoint)
        {
            timeToEnd = Time.timeSinceLevelLoad - startTime;
            startTime = Time.timeSinceLevelLoad;

            revers = !revers;

            speed = 0;
            nextPointIndex = 0;
            linePoints.Reverse();
            endPoint = linePoints[line.positionCount - 1];

            manager.AddBonus(endBonuses);
        }
        else if (rb.position == nextPoint)
        {
            nextPointIndex++;

            nextPoint = linePoints[nextPointIndex];
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, nextPoint, speed / 6 * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private void Rotation()
    {
        float angleZ = math.atan2(transform.position.y - nextPoint.y, transform.position.x - nextPoint.x);
        angleZ += 90 * math.PI / 180;

        Vector3 rot = Vector3.forward * angleZ;
        transform.rotation = quaternion.Euler(rot);
    }

    private void SetTrainPosition()
    {
        trainPosition = 0;

        for (int i = nextPointIndex; i < linePoints.Count; i++)
        {
            Vector3 point = linePoints[i];

            if (i != nextPointIndex)
            {
                trainPosition += (point - new Vector3(linePoints[i - 1].x, linePoints[i - 1].y, 1)).magnitude;
            }
            else if (i == nextPointIndex)
            {
                trainPosition += (transform.position - point).magnitude;
            }
        }

        if (revers)
            trainPosition = trainPosition;
        else
            trainPosition = lineDistance - trainPosition;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        manager.ResetScene("Trains Crash");
    }

    public float GetDistanceToEnd()
    {
        return (float)Math.Round(trainPosition * 100, 2);
    }

    public float GetPositionInPercent()
    {
        return math.round((100 - (trainPosition / lineDistance * 100)));
    }
}
