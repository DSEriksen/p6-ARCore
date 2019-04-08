using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

    public class PetControls : MonoBehaviour {

    public Transform PetModel;
    public Camera camera;


    //Old
    //public Joystick joystick;
    //public Scrollbar scrollbar;
    //float OriginalY = 0;
    //public float moveSpeed;

   
    private void Start()
    {
  
     
    }

    void Update()
    {   
        
        



        /* Stupid shit */
        /*
        PetModel.transform.Translate(Vector3.right * Mathf.Clamp(joystick.Vertical*6 , -1,1)* moveSpeed * Time.deltaTime);
        PetModel.transform.Rotate(Vector3.up * joystick.Horizontal * 66 * Time.deltaTime);
        //PetModel.transform.position = new Vector3(PetModel.transform.position.x, OriginalY+scrollbar.value, PetModel.transform.position.z);
        PetModel.transform.localScale = new Vector3(scrollbar.value, 
                                                    scrollbar.value,
                                                    scrollbar.value);
        PetModel.transform.Rotate(90,0,0,Space.Self);
        Debug.Log(PetModel.transform.localScale);
    */
    }
}
