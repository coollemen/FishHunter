using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using  UGame;

public class FishManager : MonoSingleton<FishManager>
{
    public Transform[] fishCreatePositions;

    public GameObject[] fishPrefabs;

    public float waveWaitTime = 1f;

    public float fishWaitTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("MakeFishes",2,waveWaitTime );
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
        StartCoroutine(CreateFish(fishIndex, posIndex, speed, num));
        
    }

    IEnumerator CreateFish(int prefabIndex,int posIndex, int speed,int count)
    {
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(fishPrefabs[prefabIndex],fishCreatePositions[posIndex]);
            yield return new WaitForSeconds(fishWaitTime);
                
        }

        
    }
}
