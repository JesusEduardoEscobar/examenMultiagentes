using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Make the camera look at the object, then add a movement to it in Update function.
//Add the below code to your camera, and set the target to the object in unity inspector window.

//Key codes returned by Event.keyCode. These map directly to a physical key on the keyboard.
//Key codes can be used to detect key down and key up events, using Input.GetKeyDown and Input.GetKeyUp:


public class LookAtCamera : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * Time.deltaTime * 2);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Application.Quit() does not work on the editor
            //isPlaying cannot work on a built game (no editor)
            //We use Preprocessor Directives for conditional compilation
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            UnityEngine.Application.Quit();
        }
    }
}