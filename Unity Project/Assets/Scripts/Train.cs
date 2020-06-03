using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Train : MonoBehaviour
{
    private Vector3[] points;
    private float speed = 0;
    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            if (value <= 0) speed = 0;
            else if (value >= maxSpeed) speed = maxSpeed;
            else speed = value;
        }
    }

    public float maxSpeed { get; private set; } = 80;
    public float trackDistance { get; private set; }
    public float distanceToEnd { get; private set; }
    public float distanceToTrain;
    public float distanceToIntersection;
    public bool isMoveBack { get; private set; } = false;

    public bool isAddBonusesLook;

    private int nextPointIndex = 0;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Color color, Vector3[] points)
    {
        GetComponent<SpriteRenderer>().color = color;
        this.points = points;
        transform.position = points[0];
        trackDistance = TrackDistance(points);
        nextPointIndex = 0;
    }

    private int NextPointIndex(int pointIndex, int pointsLength)
    {
        if (pointIndex == pointsLength - 1)
        {
            isMoveBack = true;
            speed = 0;

            if (!isAddBonusesLook)
            {
                GameManager.main.AddBonuses();
            }
        }
        else if (pointIndex == 0)
        {
            isMoveBack = false;
            speed = 0;

            if (!isAddBonusesLook)
            {
                GameManager.main.AddBonuses();
            }
        }

        if (!isMoveBack)
        {
            pointIndex++;
        }
        else
        {
            pointIndex--;
        }

        return pointIndex;
    }

    private void Rotation(Vector3 lookAt)
    {
        float angleZ = math.atan2(rb.position.y - lookAt.y, rb.position.x - lookAt.x);
        angleZ += 90 * math.PI / 180;

        Vector3 rot = Vector3.forward * angleZ;
        transform.rotation = quaternion.Euler(rot);
    }

    private void FixedUpdate()
    {
        if (new Vector3(rb.position.x, rb.position.y, 1) == points[nextPointIndex])
        {
            nextPointIndex = NextPointIndex(nextPointIndex, points.Length);
        }

        distanceToEnd = DistanceToEnd();

        float step = Speed / 6 * Time.fixedDeltaTime;

        Vector2 newPos = Vector2.MoveTowards(rb.position, points[nextPointIndex], step);
        rb.MovePosition(newPos);

        Rotation(points[nextPointIndex]);
    }

    private float TrackDistance(Vector3[] points)
    {
        float distance = 0;

        for (int i = 1; i < points.Length; i++)
        {
            distance += (points[i] - points[i - 1]).magnitude;
        }

        return distance;
    }

    private float DistanceToEnd()
    {
        float distanceToEnd = 0;
        int nextPointIndex = this.nextPointIndex;

        if (isMoveBack)
        {
            nextPointIndex = this.nextPointIndex + 1;
        }

        for (int i = nextPointIndex; i < points.Length; i++)
        {
            Vector3 point = points[i];

            if (i != nextPointIndex)
            {
                distanceToEnd += (point - points[i - 1]).magnitude;
            }
            else if (i == nextPointIndex)
            {
                distanceToEnd += (point - transform.position).magnitude;
            }
        }

        return distanceToEnd;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameEvents.current.TrainTriggerEnter();
    }
}
