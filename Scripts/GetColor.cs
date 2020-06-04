using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetColor : MonoBehaviour
{
    // Start is called before the first frame update
    static WebCamTexture camTexture;
    static WebCamDevice cam;
    public bool enable_camera;
    public float contrast = 5;
    public Color color;
    MeshRenderer r;
    void Start()
    {
        if (enable_camera){
            if (camTexture == null)
                camTexture = new WebCamTexture();
                //GetComponent<Renderer>().material.mainTexture = camTexture;

            if (!camTexture.isPlaying)
                camTexture.Play();
        } else{
             GetComponent<Renderer>().material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enable_camera){
            Color cc = camTexture.GetPixel(camTexture.width / 2, camTexture.height);
            cc.r = 0 - (contrast/20) + cc.r * (10/contrast);
            cc.g = 0 - (contrast/20) + cc.g * (10/contrast);
            cc.b = 0 - (contrast/20) + cc.b * (10/contrast);
            if (cc.r > 1){cc.r = 1;}
            if (cc.g > 1){cc.g = 1;}
            if (cc.b > 1){cc.b = 1;}
            if (cc.r < 0){cc.r = 0;}
            if (cc.g < 0){cc.g = 0;}
            if (cc.b < 0){cc.b = 0;}
            Debug.Log("Color = " + cc);
            GetComponent<Renderer>().material.color = cc;
        }
    }
}
