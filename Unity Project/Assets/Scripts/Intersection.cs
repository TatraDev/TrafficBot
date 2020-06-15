using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntersectionData 
{
    public Intersection intersection;
    public int id;
    public float distanceToPosition;
    public int otherTrainIndex;
}

public class Intersection : MonoBehaviour
{
    public Train firstTrain { get; private set; }
    private float firstTrackDistance;
    public float firstTrainPosition { get; private set; }
    private float firstDistBefore;

    public Train secondTrain { get; private set; }
    private float secondTrackDistance;
    public float secondTrainPosition { get; private set; }
    public float firstTrainDistanceToIntersection { get; private set; }
    public float secondTrainDistanceToIntersection { get; private set; }

    private float secondDistBefore;

    public Color good;
    public Color сaution;
    public Color prohibited;

    private float DistanceToIntersection(LineRenderer line, int LineIndexBefore)
    {
        float LineLengthBefore = 0;

        for (int i = 1; i < LineIndexBefore + 1; i++)
        {
            LineLengthBefore += (line.GetPosition(i) - line.GetPosition(i - 1)).magnitude;
        }
        LineLengthBefore += (transform.position - line.GetPosition(LineIndexBefore)).magnitude;

        return LineLengthBefore;
    }

    public void Init(int id, LineRenderer firstLine, LineRenderer secondLine, int firstLineIndex, int secondLineIndex, int firstLineIndexBefore, int secondLineIndexBefore)
    {
        firstTrain = GameManager.main.trains[firstLineIndex].GetComponent<Train>();
        secondTrain = GameManager.main.trains[secondLineIndex].GetComponent<Train>();

        firstTrackDistance = firstTrain.trackDistance;
        secondTrackDistance = secondTrain.trackDistance;

        firstDistBefore = DistanceToIntersection(firstLine, firstLineIndexBefore);
        secondDistBefore = DistanceToIntersection(secondLine, secondLineIndexBefore);

        IntersectionData data;
        data.intersection = this;
        data.id = id;

        data.distanceToPosition = firstDistBefore;
        data.otherTrainIndex = 1;
        firstTrain.intersectionDatas.Add(data);

        data.distanceToPosition = secondDistBefore;
        data.otherTrainIndex = 0;
        secondTrain.intersectionDatas.Add(data);
    }

    private float DistanceToIntersection(Train train, float trackDistance, float distBeforeIntercetion)
    {
        float trainDistanceToEnd = train.distanceToEnd;
        float trainDistanceToIntersection;

        if (!train.isMoveBack)
        {
            trainDistanceToIntersection = distBeforeIntercetion - (trackDistance - trainDistanceToEnd);
            if (trainDistanceToIntersection < 0)
            {
                trainDistanceToIntersection = -trainDistanceToIntersection + 2 * trainDistanceToEnd;
            }
        }
        else
        {
            trainDistanceToIntersection = trackDistance - distBeforeIntercetion - trainDistanceToEnd;
            if (trainDistanceToIntersection < 0)
            {
                trainDistanceToIntersection = -trainDistanceToIntersection + 2 * (trackDistance - trainDistanceToEnd);
            }
        }

        return trainDistanceToIntersection;
    }

    private void FixedUpdate()
    {
        firstTrainDistanceToIntersection = DistanceToIntersection(firstTrain, firstTrackDistance, firstDistBefore);
        secondTrainDistanceToIntersection = DistanceToIntersection(secondTrain, secondTrackDistance, secondDistBefore);

        float distanceToEachOther = Mathf.Abs(firstTrainDistanceToIntersection) + Mathf.Abs(secondTrainDistanceToIntersection);

        if (distanceToEachOther < 2f)
            GetComponent<SpriteRenderer>().color = prohibited;
        else if (distanceToEachOther < 3f)
            GetComponent<SpriteRenderer>().color = сaution;
        else
            GetComponent<SpriteRenderer>().color = good;
    }
}
