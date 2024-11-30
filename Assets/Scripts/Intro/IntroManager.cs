using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public float skyboxRotationSpeed = 1.0f;

    private GameObject title, cam, panel0, panel1, panel2, buttonNext, instruction;
    private Material skybox;
    private AudioClip soundEffect;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        GameObject mainMenu = GameObject.Find("Main Menu");
        title = mainMenu.transform.Find("Title").gameObject;
        buttonNext = mainMenu.transform.Find("Button-Next").gameObject;
        panel0 = mainMenu.transform.Find("Panel-Tutorial").gameObject;
        panel1 = mainMenu.transform.Find("Panel-World1").gameObject;
        panel2 = mainMenu.transform.Find("Panel-World2").gameObject;
        instruction = mainMenu.transform.Find("Instructions").gameObject;

        skybox = Instantiate(RenderSettings.skybox);
        RenderSettings.skybox = skybox;

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
        panel0.SetActive(true);
        panel0.GetComponent<Animator>().SetBool("triggered", true);
        cam.GetComponent<AudioSource>().PlayOneShot(soundEffect);
    }

    public void StartNextPanelAnim(GameObject g)
    {
        if (g == panel0)
        {
            panel1.SetActive(true);
            panel1.GetComponent<Animator>().SetBool("triggered", true);
        }
        else if (g == panel1)
        {
            panel2.SetActive(true);
            panel2.GetComponent<Animator>().SetBool("triggered", true);
        }
    }

    public void StartInstructionAnim()
    {
        instruction.SetActive(true);
    }
}
