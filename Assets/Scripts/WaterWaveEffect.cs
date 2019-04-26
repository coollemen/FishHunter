using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaveEffect : MonoBehaviour
{
    public Texture[] textures;

    public Material mat;

    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        InvokeRepeating("ChangeTexture",0,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeTexture()
    {
        mat.mainTexture = textures[index];
        if (index >= (textures.Length-1))
        {
            index = 0;
        }
        else
        {
            index++;
        }
    }
}
