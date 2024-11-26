using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public float skyboxRotationSpeed = 1.0f;

    private GameObject title, cam, panel0, panel1, panel2, buttonNext;
    private Material skybox;
    private AudioClip soundEffect;

    void Start()
    {
        title = GameObject.Find("Title");
        cam = GameObject.Find("Main Camera");
        panel0 = GameObject.Find("Panel-Tutorial");
        panel1 = GameObject.Find("Panel-World1");
        panel2 = GameObject.Find("Panel-World2");
        buttonNext = GameObject.Find("Button-Next");

        skybox = RenderSettings.skybox;

        soundEffect = Resources.Load<AudioClip>("Sounds/sfx_slide");
    }

    // Update is called once per frame
    void Update()
    {
        float skyboxRot = skybox.GetFloat("_Rotation");
        skyboxRot += skyboxRotationSpeed * Time.deltaTime;
        if (skyboxRot >= 360.0f)
            skyboxRot -= 360.0f;
        skybox.SetFloat("_Rotation", skyboxRot);
    }

    public void OnButtonNextClick()
    {
        title.GetComponent<Animator>().SetBool("Button-Next", true);
        cam.GetComponent<Animator>().SetBool("Button-Next", true);
        buttonNext.GetComponent<Animator>().SetBool("triggered", true);
    }

    public void StartFirstPanelAnim()
    {
        panel0.GetComponent<Animator>().SetBool("triggered", true);
        cam.GetComponent<AudioSource>().PlayOneShot(soundEffect);
    }

    public void StartNextPanelAnim(GameObject g)
    {
        if (g == panel0)
            panel1.GetComponent<Animator>().SetBool("triggered", true);
        else if (g == panel1)
            panel2.GetComponent<Animator>().SetBool("triggered", true);
    }
}
