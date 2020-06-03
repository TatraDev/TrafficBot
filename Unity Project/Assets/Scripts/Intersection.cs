using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public float firstTrainPosition { get; private set; }
    public float firstListLengthBefore;
    public float secondTrainPosition { get; private set; }
    public float secondListLengthBefore;

    private int firstLineIndex;
    private int secondLineIndex;

    public Color good;
    public Color сaution;
    public Color prohibited;

    public void Init(LineRenderer firstLine, LineRenderer secondLine, int firstLineIndex, int secondLineIndex, int firstLineIndexBefore, int secondLineIndexBefore)
    {
        this.firstLineIndex = firstLineIndex;
        this.secondLineIndex = secondLineIndex;

        firstListLengthBefore = LengthToIntersection(firstLine, firstLineIndexBefore);
        secondListLengthBefore = LengthToIntersection(secondLine, secondLineIndexBefore);
    }

    private void Update()
    {
        Light();
    }

    private float LengthToIntersection(LineRenderer line, int LineIndexBefore)
    {
        float LineLengthBefore = 0;

        for (int i = 1; i < LineIndexBefore + 1; i++)
        {
            LineLengthBefore += (line.GetPosition(i) - line.GetPosition(i - 1)).magnitude;
        }
        LineLengthBefore += (transform.position - line.GetPosition(LineIndexBefore)).magnitude;

        return LineLengthBefore;
    }


    private void Light()
    {
        firstTrainPosition = GameManager.main.trains[firstLineIndex].GetComponent<Train>().distanceToEnd;
        secondTrainPosition = GameManager.main.trains[secondLineIndex].GetComponent<Train>().distanceToEnd;

        float firstTrackDistance = GameManager.main.trains[firstLineIndex].GetComponent<Train>().trackDistance;
        float secondTrackDistance = GameManager.main.trains[secondLineIndex].GetComponent<Train>().trackDistance;

        float firstDistanceToIntersection = Mathf.Abs(firstListLengthBefore - (firstTrackDistance - firstTrainPosition));
        float secondDistanceToIntersection = Mathf.Abs(secondListLengthBefore - (secondTrackDistance - secondTrainPosition));

        float distanceToEachOther = firstDistanceToIntersection + secondDistanceToIntersection;

        GameManager.main.trains[firstLineIndex].GetComponent<Train>().distanceToIntersection = firstDistanceToIntersection;
        GameManager.main.trains[secondLineIndex].GetComponent<Train>().distanceToIntersection = secondDistanceToIntersection;

        GameManager.main.trains[firstLineIndex].GetComponent<Train>().distanceToTrain = distanceToEachOther;
        GameManager.main.trains[secondLineIndex].GetComponent<Train>().distanceToTrain = distanceToEachOther;

        if (distanceToEachOther < 2f)
            GetComponent<SpriteRenderer>().color = prohibited;
        else if (distanceToEachOther < 3f)
            GetComponent<SpriteRenderer>().color = сaution;
        else
            GetComponent<SpriteRenderer>().color = good;
    }
}
