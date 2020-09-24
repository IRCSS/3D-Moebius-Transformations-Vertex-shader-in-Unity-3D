using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

    public float _Radius         = 2.3f;
    public float _isoclinic      = 0.0f;
    public float _qRotation      = 1.0f;
    public float _speed          = 0.1f;
    public float _maxRotat       = 0.1f;
    public bool  _CameraCenteric = false;

    private Camera main_cam;
    private float theta = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        main_cam = Camera.main;
        if (main_cam == null) Debug.LogError("Could not find the main camera");



    }


    Vector3 MobieusTrasnform(Vector3 pos)
    {
        Vector4 p = new Vector4(pos.x / _Radius, pos.y / _Radius, pos.z / _Radius, 1.0f);
        

        float wf = 1.0f + p.x * p.x + p.y * p.y + p.z * p.z;

        Matrix4x4 projectionTo3Sphere = new Matrix4x4( 
            new Vector4( 2.0f / wf,      0.0f,      0.0f,           0.0f),
            new Vector4(      0.0f, 2.0f / wf,      0.0f,           0.0f), 
            new Vector4(      0.0f,      0.0f, 2.0f / wf,           0.0f),
            new Vector4(      0.0f,      0.0f,      0.0f, (wf - 2.0f)/wf));

        p = projectionTo3Sphere * p;

        float theta = Mathf.Sin(Time.time * _speed) * Mathf.PI * _maxRotat;

        float cos_p = Mathf.Cos(_isoclinic * theta);
        float sin_p = Mathf.Sin(_isoclinic * theta);

        float cos_q = Mathf.Cos(_qRotation * theta);
        float sin_q = Mathf.Sin(_qRotation * theta);

        Matrix4x4 rotate4D = new Matrix4x4(
            new Vector4(cos_p, -sin_p,  0.0f,   0.0f),
            new Vector4(sin_p,  cos_p,  0.0f,   0.0f),
            new Vector4( 0.0f,   0.0f, cos_q, -sin_q),
            new Vector4( 0.0f,   0.0f, sin_q,  cos_q));

        p = rotate4D * p;

        p   = p / (1.0f - p.w);
        p   = p * _Radius;

        return new Vector3(p.x, p.y, p.z);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_Radius",    _Radius);
        Shader.SetGlobalFloat("_isoclinic", _isoclinic);
        Shader.SetGlobalFloat("_qRotation", _qRotation);
        Shader.SetGlobalFloat("_speed",     _speed);
        Shader.SetGlobalFloat("_maxRotat",  _maxRotat);
        Shader.SetGlobalFloat("iTime",      Time.time);
        Shader.SetGlobalInt("_CameraCenteric", _CameraCenteric ? 1 : 0);

        float change = 0.0f;
        if (Input.GetKey(KeyCode.T)) change += _speed;
        if (Input.GetKey(KeyCode.G)) change -= _speed;

        theta += change * Time.deltaTime;
        Shader.SetGlobalFloat("_theta", theta);
        

    }
}
