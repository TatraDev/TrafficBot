using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private List<GameObject> lines = new List<GameObject>();

    public float firstTrainPosition;
    public float secondTrainPosition;

    public float firstListLengthBefore;
    public float secondListLengthBefore;

    public Color good;
    public Color сaution;
    public Color prohibited;

    public void Init(List<GameObject> lines, int firstLineIndexBefore, int secondLineIndexBefore)
    {
        this.lines = lines;

        LengthToIntersection(lines[0].GetComponent<LineRenderer>(), firstLineIndexBefore, out firstListLengthBefore);
        LengthToIntersection(lines[1].GetComponent<LineRenderer>(), secondLineIndexBefore, out secondListLengthBefore);
    }

    private void Update()
    {
        Light();
    }

    private void LengthToIntersection(LineRenderer line, int LineIndexBefore, out float LineLengthBefore)
    {
        LineLengthBefore = 0;

        for (int i = 1; i < LineIndexBefore + 1; i++)
        {
            LineLengthBefore += (line.GetPosition(i) - line.GetPosition(i - 1)).magnitude;
        }
        LineLengthBefore += (transform.position - line.GetPosition(LineIndexBefore)).magnitude;
    }


    private void Light()
    {
        firstTrainPosition = lines[0].GetComponent<Line>().train.GetComponent<Train>().trainPosition;
        secondTrainPosition = lines[1].GetComponent<Line>().train.GetComponent<Train>().trainPosition;

        if ((firstListLengthBefore - firstTrainPosition < 0.8f ^ firstListLengthBefore - firstTrainPosition < -0.8f) && (secondListLengthBefore - secondTrainPosition < 0.8f ^ secondListLengthBefore - secondTrainPosition < -0.8f))
            GetComponent<SpriteRenderer>().color = prohibited;
        else if ((firstListLengthBefore - firstTrainPosition < 1.8f ^ firstListLengthBefore - firstTrainPosition < -1.8f) && (secondListLengthBefore - secondTrainPosition < 1.8f ^ secondListLengthBefore - secondTrainPosition < -1.8f))
            GetComponent<SpriteRenderer>().color = сaution;
        else
            GetComponent<SpriteRenderer>().color = good;
    }
}
