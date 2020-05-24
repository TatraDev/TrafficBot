﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private List<GameObject> lines = new List<GameObject>();

    public float firstTrainPosition { get; private set; }
    public float firstListLengthBefore { get; private set; }
    public float secondTrainPosition { get; private set; }
    public float secondListLengthBefore { get; private set; }

    [SerializeField]
    private Color good;
    [SerializeField]
    private Color сaution;
    [SerializeField]
    private Color prohibited;

    public void Init(List<GameObject> lines, int firstLineIndexBefore, int secondLineIndexBefore)
    {
        this.lines = lines;

        firstListLengthBefore = LengthToIntersection(lines[0].GetComponent<LineRenderer>(), firstLineIndexBefore);
        secondListLengthBefore = LengthToIntersection(lines[1].GetComponent<LineRenderer>(), secondLineIndexBefore);
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
