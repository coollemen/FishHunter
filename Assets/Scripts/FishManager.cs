using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using  UGame;

public class FishManager : MonoSingleton<FishManager>
{
    public Transform[] fishCreatePositions;

    public List<FishDef> fishDefs=new List<FishDef>();
    public float waveWaitTime = 1f;

    public float fishWaitTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < fishDefs.Count; i++)
        {
            PoolManager.Instance.CreatePool(fishDefs[i].url, fishDefs[i].prefabPath  , 10);
        }
        
        InvokeRepeating("MakeFishes",2,waveWaitTime );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeFishes()
    {
        int posIndex = Random.Range(0, fishCreatePositions.Length);
        int fishIndex = Random.Range(0, fishDefs.Count);
        var def = fishDefs[fishIndex];
        int num = Random.Range((def.maxNum/2)+1, def.maxNum);
        int speed = Random.Range(def.maxSpeed / 2, def.maxSpeed);
        StartCoroutine(CreateFish(def.url, fishCreatePositions[posIndex],  num,speed,def.spawnTimeSpan));
        
    }

    IEnumerator CreateFish(string url,Transform pos,int count,int speed,float timespan)
    {
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = PoolManager.Instance.Spawn(url);
            go.GetComponent<Fish>().speed = speed;
            go.transform.position = pos.position;
            go.transform.localRotation = pos.localRotation;
            yield return new WaitForSeconds(timespan);
                
        }

        
    }
}
