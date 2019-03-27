using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetControls : MonoBehaviour {

    // Use this for initialization

    public Transform PetModel;
    public float moveSpeed;
    public Joystick joystick;


    public Scrollbar scrollbar;

    float OriginalY = 0;

   
    private void Start()
    {
        OriginalY = PetModel.transform.position.y;
        //scrollbar.minValue = 0.1;
        //scrollbar.MaxValue = 1.0;
     
    }

    void Update()
    {   
        PetModel.transform.Translate(Vector3.right * Mathf.Clamp(joystick.Vertical*6 , -1,1)* moveSpeed * Time.deltaTime);
        PetModel.transform.Rotate(Vector3.up * joystick.Horizontal * 66 * Time.deltaTime);



        //PetModel.transform.position = new Vector3(PetModel.transform.position.x, OriginalY+scrollbar.value, PetModel.transform.position.z);
        PetModel.transform.localScale = new Vector3(PetModel.transform.localScale.x, 
                                                    PetModel.transform.localScale.y,
                                                    scrollbar.value);
        Debug.Log(PetModel.transform.localScale);
    }
}
