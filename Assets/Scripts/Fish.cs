using System.Collections;
using System.Collections.Generic;
using UGame;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int speed;
    public bool isRotate = false;
    public int rotateSpeed;

    public float rotateAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right*speed*Time.deltaTime);
        if (isRotate)
        {
            transform.Rotate(Vector3.forward,rotateAngle*rotateSpeed*Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Border")
        {
            PoolManager.Instance.Despawn(this.gameObject);
            //Destroy(this);
        }
    }
}
