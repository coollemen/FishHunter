using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFollow : MonoBehaviour
{
    public Transform gunPos;

    public Transform gun;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       var mousePos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
       //清除深度值
       mousePos.z = 0;
       Debug.Log(mousePos);
       float z;
       if (mousePos.x > gunPos.position.x)
       {
           z = -Vector3.Angle(Vector3.up, mousePos - gunPos.position);
       }
       else
       {
           z = Vector3.Angle(Vector3.up, mousePos - gunPos.position);
       }
       gun.localRotation=Quaternion.Euler(0,0,z);
    }
}
