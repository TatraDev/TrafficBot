using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using System;

public class Train : MonoBehaviour
{
    private Vector2 endPoint;
    private Vector2 nextPoint;

    private int nextPointIndex;

    public LineRenderer line;

    public float MaxSpeed { get; private set; } = 120;

    private float speed = 5f;
    public float Speed
    {
        get { return speed; }
        set
        {
            if (value <= 0) speed = 0;
            else if (value >= MaxSpeed) speed = MaxSpeed;
            else speed = value;
        }
    }

    private List<Vector2> linePoints;

    public float LineDistance { get; private set; } = 0;
    public float TrainPosition { get; private set; } = 0;

    private float startTime;
    public float TimeToEnd { get; private set; }

    private GameManager manager;
    private Rigidbody2D rb;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();

        linePoints = new List<Vector2>();

        for (int i = 0; i < line.positionCount; i++)
        {
            Vector3 point = line.GetPosition(i);

            if (i != 0)
            {
                LineDistance += (point - line.GetPosition(i - 1)).magnitude;
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
            TimeToEnd = Time.timeSinceLevelLoad - startTime;
            startTime = Time.timeSinceLevelLoad;
            manager.PlusBonus(15);
            nextPointIndex = 0;
            linePoints.Reverse();
            endPoint = linePoints[line.positionCount - 1];
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
        TrainPosition = 0;

        for (int i = nextPointIndex; i < linePoints.Count; i++)
        {
            Vector3 point = linePoints[i];

            if (i != nextPointIndex)
            {
                TrainPosition += (point - new Vector3(linePoints[i - 1].x, linePoints[i - 1].y, 1)).magnitude;
            }
            else if (i == nextPointIndex)
            {
                TrainPosition += (transform.position - point).magnitude;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        manager.ResetScene();
    }

    public float GetDistanceToEnd()
    {
        return (float)Math.Round(LineDistance * 100 - TrainPosition * 100, 2);
    }

    public float GetPositionInPercent()
    {
        return math.round((100 - (TrainPosition / LineDistance * 100)));
    }
}
