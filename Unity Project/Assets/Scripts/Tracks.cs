using LineSegmentsIntersection;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RandomPoint
{
    public int index;
    public Vector3 from;
    public Vector3 to;
}

[System.Serializable]
public struct Track
{
    public int id;
    public LineRenderer lineRenderer;
    public RandomPoint[] randomPoints;
}

public class Tracks : MonoBehaviour
{
    public GameObject trainPrefab;
    public GameObject intersectionPrefab;
    public GameObject trackCapPrefab;
    [SerializeField] private Track[] tracks = default;
    private List<GameObject> tracksCups = new List<GameObject>();
    private List<GameObject> intersectionsObjs = new List<GameObject>();

    private bool isTrainsInts;

    public List<Train> trains { get; private set; } = new List<Train>();

    private void Start()
    {
        ResetTracksPoints();

        GameEvents.current.onGameRestart += ResetTracksPoints;
    }

    private Train InstTrain(int trackId, Color color, Vector3[] points)
    {
        Train train = Instantiate(trainPrefab).GetComponent<Train>();
        trains.Add(train);

        train.Init(trackId, color, points);

        return train;
    }

    private void InstTrains()
    {
        foreach (Track track in tracks)
        {
            Color color = track.lineRenderer.startColor;
            color.r += 0.15f;
            color.g += 0.15f;
            color.b += 0.15f;

            Vector3[] points = new Vector3[track.lineRenderer.positionCount];
            track.lineRenderer.GetPositions(points);

            Train train = InstTrain(track.id, color, points);
            train.transform.parent = track.lineRenderer.gameObject.transform;
        }

        GameManager.main.SetTrains(trains);
    }

    public void ReInitTrains()
    {
        for (int i = 0; i < trains.Count; i++)
        {
            Color color = tracks[i].lineRenderer.startColor;
            color.r += 0.15f;
            color.g += 0.15f;
            color.b += 0.15f;

            Vector3[] points = new Vector3[tracks[i].lineRenderer.positionCount];
            tracks[i].lineRenderer.GetPositions(points);

            trains[i].Init(color, points);
        }
    }

    void InstIntersections(int startId)
    {
        int id = startId + 1;

        for (int i = 0; i < tracks.Length; i++)
        {
            for (int j = i + 1; j < tracks.Length; j++)
            {
                for (int x = 1; x < tracks[i].lineRenderer.positionCount; x++)
                {
                    for (int y = 1; y < tracks[j].lineRenderer.positionCount; y++)
                    {
                        if (Math2d.LineSegmentsIntersection(tracks[i].lineRenderer.GetPosition(x - 1), tracks[i].lineRenderer.GetPosition(x), tracks[j].lineRenderer.GetPosition(y - 1), tracks[j].lineRenderer.GetPosition(y), out Vector2 intersection))
                        {
                            GameObject intersectionObj = Instantiate(intersectionPrefab);
                            intersectionObj.transform.parent = transform;
                            intersectionObj.transform.position = new Vector3(intersection.x, intersection.y, 1);

                            intersectionObj.GetComponent<Intersection>().Init(id, tracks[i].lineRenderer, tracks[j].lineRenderer, i, j, x - 1, y - 1);

                            id++;

                            intersectionsObjs.Add(intersectionObj);
                        }
                    }
                }
            }
        }
    }

    void DestroyIntersections()
    {
        foreach (GameObject intersection in intersectionsObjs)
        {
            Destroy(intersection);
        }

        intersectionsObjs.Clear();
    }

    private void InstCup(Color color, Vector3 position)
    {
        GameObject trackCap = Instantiate(trackCapPrefab);
        tracksCups.Add(trackCap);

        trackCap.GetComponent<SpriteRenderer>().color = color;
        trackCap.transform.position = position;
        trackCap.transform.parent = transform;
    }

    private void DestroyCups()
    {
        foreach (GameObject cup in tracksCups)
        {
            Destroy(cup);
        }

        tracksCups.Clear();
    }

    public void ResetTracksPoints()
    {
        DestroyIntersections();
        DestroyCups();

        int id = 0;

        for (int i = 0; i < tracks.Length; i++) { tracks[i].id = i; id = i; }

        foreach (Track track in tracks)
        {
            foreach (RandomPoint randomPoint in track.randomPoints)
            {
                Vector3 position = new Vector3(
                    Random.Range(randomPoint.from.x, randomPoint.to.x),
                    Random.Range(randomPoint.from.y, randomPoint.to.y),
                    Random.Range(randomPoint.from.z, randomPoint.to.z)
                    );

                track.lineRenderer.SetPosition(randomPoint.index, position);
            }

            InstCup(track.lineRenderer.startColor, track.lineRenderer.GetPosition(0));
            InstCup(track.lineRenderer.startColor, track.lineRenderer.GetPosition(track.lineRenderer.positionCount - 1));
        }

        if (!isTrainsInts)
        {
            InstTrains();
            isTrainsInts = true;
        }
        else
        {
            ReInitTrains();
        }

        InstIntersections(id);
    }
}
