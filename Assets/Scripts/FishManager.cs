using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UGame;

public class FishManager : MonoSingleton<FishManager>
{
    public Transform[] fishCreatePositions;

    public GameObject[] fishPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeFishes()
    {
        int posIndex = Random.Range(0, fishCreatePositions.Length);
        int fishIndex = Random.Range(0, fishPrefabs.Length);
        var fishAtt = fishPrefabs[fishIndex].GetComponent<Fish>();
        int maxNum = fishAtt.maxCount;
        int maxSpeed = fishAtt.maxSpeed;
        int num = Random.Range((maxNum/2)+1, maxNum);
        int speed = Random.Range(maxSpeed / 2, maxSpeed);

    }
}
