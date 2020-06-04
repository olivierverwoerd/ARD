Shader "ARD/ARD_Grass_Ground"
{
    Properties {
        _GroundColor ("Color", Color) = (1,1,1)
    }
    
    SubShader {
        Color [_GroundColor]
        Pass {}
    }
}