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

    public float maxSpeed { get; private set; } = 100;
    public float trackDistance { get; private set; }
    public List<IntersectionData> intersectionDatas = new List<IntersectionData>();
    private List<float> distanceToIntersections = new List<float>();
    public float distanceToEnd { get; private set; }
    public bool isMoveBack { get; private set; } = false;
    public bool isLookAddBonuses;
    private int nextPointIndex = 0;

    private int trackId;

    public float distToImportantPoint { get; private set; }
    public float otherTrainDistToInter { get; private set; }
    public bool isNextImpPointFree { get; private set; }

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Color color, Vector3[] points)
    {
        intersectionDatas.Clear();
        GetComponent<SpriteRenderer>().color = color;
        this.points = points;
        transform.position = points[0];
        trackDistance = TrackDistance(points);
        nextPointIndex = 0;
    }

    public void Init(int trackId, Color color, Vector3[] points)
    {
        this.trackId = trackId;
        Init(color, points);
    }

    private int NextPointIndex(int pointIndex, int pointsLength)
    {
        if (pointIndex == pointsLength - 1)
        {
            speed = 0;

            if (!isLookAddBonuses && !isMoveBack)
            {
                GameManager.main.AddBonuses();
            }

            isMoveBack = true;
        }
        else if (pointIndex == 0)
        {
            speed = 0;

            if (!isLookAddBonuses && isMoveBack)
            {
                GameManager.main.AddBonuses();
            }

            isMoveBack = false;
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

    private void NextImportantPoint()
    {
        distanceToIntersections.Clear();
        for (int i = 0; i < intersectionDatas.Count; i++)
        {
            if (!isMoveBack)
                distanceToIntersections.Add(intersectionDatas[i].distanceToPosition - (trackDistance - distanceToEnd));
            else
                distanceToIntersections.Add(trackDistance - intersectionDatas[i].distanceToPosition - distanceToEnd);
        }

        float distToImportantPoint = 250;
        float otherTrainDistToInter = 1;
        int nextImportantPointId = trackId;

        bool isChange = false;

        for (int i = 0; i < distanceToIntersections.Count; i++)
        {
            if (distToImportantPoint > distanceToIntersections[i])
            {
                if (distanceToIntersections[i] >= 0)
                {
                    distToImportantPoint = distanceToIntersections[i] / trackDistance;
                    nextImportantPointId = intersectionDatas[i].id;

                    if (intersectionDatas[i].otherTrainIndex == 0)
                        otherTrainDistToInter = intersectionDatas[i].intersection.firstTrainDistanceToIntersection / intersectionDatas[i].intersection.firstTrain.trackDistance;
                    else
                        otherTrainDistToInter = intersectionDatas[i].intersection.secondTrainDistanceToIntersection / intersectionDatas[i].intersection.secondTrain.trackDistance;

                    isChange = true;
                }
            }
        }

        if (!isChange)
        {
            if (!isMoveBack)
                distToImportantPoint = distanceToEnd / trackDistance;
            else
                distToImportantPoint = (trackDistance - distanceToEnd) / trackDistance;
        }

        this.distToImportantPoint = distToImportantPoint;

        if (otherTrainDistToInter > 1) otherTrainDistToInter = 1;
        this.otherTrainDistToInter = otherTrainDistToInter;
        isNextImpPointFree = !isChange;
    }

    private void LookAt(Vector3 point)
    {
        float angleZ = math.atan2(rb.position.y - point.y, rb.position.x - point.x);
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

        LookAt(points[nextPointIndex]);
    }

    private void Update()
    {
        //if (!isLookAddBonuses)
        //    NextImportantPoint();
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

    public float GetDistanceToIntersection()
    {
        float distanceToIntersection;

        if (!isMoveBack) distanceToIntersection = intersectionDatas[0].distanceToPosition - (trackDistance - distanceToEnd);
        else distanceToIntersection = trackDistance - intersectionDatas[0].distanceToPosition - distanceToEnd;

        return distanceToIntersection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameEvents.current.TrainTriggerEnter();
    }
}
