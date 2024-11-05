using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 25, -20);
    public Canvas hudCanvas;
    public HUDManager hudManager;

    public Material hiddenMaterial;
    public float transparencyFadeSpeed = 0.05f;

    private List<GameObject> hiddenObjs = new List<GameObject>();
    private List<bool> hiddenObjsTest = new List<bool>();
    private Dictionary<GameObject, Material> originalMats = new Dictionary<GameObject, Material>();

    void LateUpdate() {
        transform.position = player.transform.position + offset;

        /*if (hudCanvas != null)
        {
            hudCanvas.transform.position = transform.position + transform.forward * 10;
            hudCanvas.transform.rotation = transform.rotation;
        }*/

        for (int i = 0; i < hiddenObjsTest.Count; i++)
            hiddenObjsTest[i] = false;

        Vector3 direction = player.transform.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == player.transform)
                break;
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
                if (renderer != null && hiddenMaterial != null && renderer.gameObject.tag == "Untagged")
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
