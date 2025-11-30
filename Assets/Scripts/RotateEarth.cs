using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEarth : MonoBehaviour
{

    private float orbitAngle = 5f;
    private float axisAngle = 1f;

    [SerializeField] private float orbitSpeed = 1f;
    [SerializeField] private float orbitRadius = 5f;
    [SerializeField] private float axisSpeed = 1f;


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

        //Rotate Earth about the Sun
        var xPos = orbitRadius * Mathf.Sin(orbitAngle);
        var zPos = orbitRadius * Mathf.Cos(orbitAngle);

        transform.localPosition = new Vector3(xPos, 0f, zPos);

        orbitAngle += orbitSpeed * Time.deltaTime;

        //Rotate eath about own Axis
        transform.Rotate(0f, axisAngle, 0f);
        //axisAngle += axisSpeed * Time.deltaTime;

    }
}