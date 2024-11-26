using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // For adjusting camera pos to player (ball)
    private BallControl ball;
    private Vector3 offset = new Vector3(0, 25, -20);

    private Material hiddenMaterial;
    public float transparencyFadeSpeed = 0.05f;

    private List<GameObject> hiddenObjs = new List<GameObject>();
    private List<bool> hiddenObjsTest = new List<bool>();
    private Dictionary<GameObject, Material> originalMats = new Dictionary<GameObject, Material>();

    private void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
        transform.LookAt(ball.transform.position);

        hiddenMaterial = Resources.Load<Material>("Materials/Transparent");
    }

    private void Update()
    {
        transform.position = ball.transform.position + offset;

        for (int i = 0; i < hiddenObjsTest.Count; i++)
            hiddenObjsTest[i] = false;

        Vector3 direction = ball.transform.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == ball.transform)
                continue;
            int index = hiddenObjs.IndexOf(hit.collider.gameObject);
            if (index != -1)
            {
                hiddenObjsTest[index] = true;
                Renderer renderer = hit.collider.transform.GetComponent<Renderer>();
                Color rgba = renderer.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
                rgba.a = Mathf.Max(rgba.a - transparencyFadeSpeed, 0.0f);
                renderer.material.SetColor("_Color", rgba);
            }
            else
            {
                Renderer renderer = hit.collider.transform.GetComponent<Renderer>();
                if (renderer != null && renderer.gameObject.tag == "Untagged")
                {
                    Color rgba = renderer.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");

                    hiddenObjs.Add(hit.collider.gameObject);
                    hiddenObjsTest.Add(true);
                    if (!originalMats.ContainsKey(hit.collider.gameObject))
                        originalMats[hit.collider.gameObject] = renderer.material;
                    Material mat = Instantiate(hiddenMaterial);
                    mat.SetColor("_Color", rgba);
                    renderer.material = mat;
                    renderer.gameObject.tag = "Hidden";
                }
            }
        }
        
        for (int i = hiddenObjs.Count - 1; i >= 0; i--)
        {
            if (!hiddenObjsTest[i])
            {
                Renderer renderer = hiddenObjs[i].GetComponent<Renderer>();
                if (renderer != null && originalMats.ContainsKey(hiddenObjs[i]))
                {
                    Material mat = renderer.material;
                    Color rgba = renderer.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
                    if (rgba.a >= 1.0f)
                    {
                        renderer.material = originalMats[hiddenObjs[i]];
                        renderer.gameObject.tag = "Untagged";
                        originalMats.Remove(hiddenObjs[i]);
                        hiddenObjs.RemoveAt(i);
                        hiddenObjsTest.RemoveAt(i);
                    }
                    else
                    {
                        rgba.a = Mathf.Min(rgba.a + transparencyFadeSpeed, 1.0f);
                        renderer.material.SetColor("_Color", rgba);
                    }
                }
            }
        }
    }

    public void ChangeMat(GameObject obj, Color color)
    {
        originalMats[obj].color = color;
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = renderer.material;
        Color rgba = renderer.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
        rgba.r = color.r;
        rgba.g = color.g;
        rgba.b = color.b;
        renderer.material.SetColor("_Color", rgba);
    }
}
