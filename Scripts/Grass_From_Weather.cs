using System.Collections;
using System.Collections.Generic;
//XML
using System;
using System.IO;
using System.Net;
using System.Xml;
//Unity
using UnityEngine;

public class Grass_From_Weather : MonoBehaviour {
    [Header ("Material Settings")]
    public Material Grass_Material;
    public Material Ground_Material;

    [Header ("Shader options")]
    public bool Use_Buienrader = false;
    public string Weather_Station_Code = "6260"; // UTRECHT
    public bool Use_Default_Ground = true;
    public Color Default_Ground;

    [Header ("Default Weather")]
    public int Temprature = 15;
    public int Sun_Strenght = 100;
    public int Rain_in_mmpu = 0;
    public int Wind_Direction_Degrees = 0;
    public int Wind_Strenght = 3;

    [Header ("Modifiers")]
    
    public int Sun_modifier = 1;
    public int rain_modifier = 1;

    // Start is called before the first frame update
    // This is the only thing we need since calling the XML is slow and weather doesn't change that fast
    void Start () {
        Debug.Log (Grass_Material.GetColor ("_BottomColor"));
        Debug.Log (Ground_Material.GetColor ("_GroundColor"));

        if (Use_Default_Ground) {
            Ground_Material.SetColor ("_GroundColor", Default_Ground);
        }

        if (Use_Buienrader) { // gets the data from buienrader
            using (var request = new WebClient ()) {
                //Download the data
                String URLString = "https://data.buienradar.nl/1.0/feed/xml";
                XmlTextReader reader = new XmlTextReader (URLString);
                bool is_station = false;
                string last_name = "";
                while (reader.Read ()) {
                    switch (reader.NodeType) {
                        case XmlNodeType.Element: // The node is an element.
                            while (reader.MoveToNextAttribute ()) { // Read the attributes.
                                if (reader.Name == "id") {
                                    if (reader.Value == Weather_Station_Code) {
                                        is_station = true;
                                    } else {
                                        is_station = false;
                                    }
                                }
                            }
                            if (is_station) {
                                last_name = reader.Name; // save the type
                            }
                            break;
                        case XmlNodeType.Text: //Display the text in each element.
                            if (is_station) {
                                if (last_name == "temperatuurGC") {
                                    Temprature = (int) float.Parse (reader.Value);
                                    Temprature = Temprature / 10;
                                } else if (last_name == "regenMMPU") {
                                    if (reader.Value == "-") {
                                        Rain_in_mmpu = 0;
                                    } else {
                                        Rain_in_mmpu = Int32.Parse (reader.Value);
                                    }
                                } else if (last_name == "windrichtingGR") {
                                    Wind_Direction_Degrees = Int32.Parse (reader.Value);
                                } else if (last_name == "windsnelheidBF") {
                                    if (reader.Value == "-") {
                                        Wind_Strenght = 0;
                                    } else {
                                        Wind_Strenght = Int32.Parse (reader.Value);
                                    }
                                } else if (last_name == "zonintensiteitWM2") {
                                    if (reader.Value == "-") {
                                        Sun_Strenght = 0;
                                    } else {
                                        Sun_Strenght = Int32.Parse (reader.Value);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            Debug.Log ("Weather data from Buienrader:Temp=" + Temprature +
                "|Sun=" + Sun_Strenght +
                "|Rain=" + Rain_in_mmpu +
                "|windDir=" + Wind_Direction_Degrees +
                "|wind=" + Wind_Strenght);
        }
        // CHANGE SHADER ACCORDINGLY --------------------------------------------------
        //Rain
        if (Rain_in_mmpu > 0) {
            if (Use_Default_Ground) {
                Color old_ground = Ground_Material.GetColor ("_GroundColor");
                float amount = Rain_in_mmpu;
                amount = 1 - amount / 50;
                if (amount < 0){
                    amount = 0; 
                }
                Color new_ground = new Color (old_ground.r * amount, old_ground.g * amount, old_ground.b * amount);
                Ground_Material.SetColor ("_GroundColor", new_ground);
            }

        }
        //Temprature
        float temp_normalized = Temprature;
        temp_normalized = temp_normalized / 250;
        Color top = new Color (0.4759349f + (temp_normalized * 2), 0.8679245f + temp_normalized, 0.3070487f);
        Color bottom = new Color (0.06274508f + (temp_normalized * 4), 0.3764706f + temp_normalized, 0.07058821f);
        Grass_Material.SetColor("_TopColor", top);
        Grass_Material.SetColor("_BottomColor", bottom);

        //Sun
        if (Use_Default_Ground) {
            Color old_ground = Ground_Material.GetColor ("_GroundColor");
            float amount = Sun_Strenght;
            amount = amount / (Sun_modifier*10);
            Debug.Log(amount);
            Color new_ground = new Color (old_ground.r + amount, old_ground.g + amount, old_ground.b + amount);
            Ground_Material.SetColor ("_GroundColor", new_ground);
        }
        
        //Sunny points on grass
        float amount_grass = Sun_Strenght;
        amount_grass = amount_grass / 500 - 1;
        Grass_Material.SetFloat ("_TranslucentGain", amount_grass);

        //Wind
        float wind = Wind_Strenght;
        wind = wind * 0.05f;
        Grass_Material.SetFloat("_WindStrength", wind);
        Grass_Material.SetFloat ("_WindDirection", Wind_Direction_Degrees);

        //grass curve
        float curve = 1;
        curve += Rain_in_mmpu/5;
        curve += (Temprature/6);
        if (curve > 10){
            curve = 10;
        }
        Grass_Material.SetFloat("_BladeCurve", curve);
        
        //If the ground is not specified. We use the grass color
        if (!Use_Default_Ground) {
            Ground_Material.SetColor ("_GroundColor",
                Grass_Material.GetColor ("_BottomColor"));
        }
        //
    }
}