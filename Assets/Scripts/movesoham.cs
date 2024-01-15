using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class movesoham : MonoBehaviour
{
    enum joint
    {
        knee,
        elbow,
        wrist,
    };
    enum angle{
        x,
        y,
        xy,
    };
   public int max , min=0;
    [SerializeField]
    angle ang= new angle();
    [SerializeField]
    joint js = new joint();
    [SerializeField]
    int kneedeviation;
    [SerializeField]
    int elbowdeviation;
    [SerializeField]
    int wristdeviationx;
    [SerializeField]
    int wristdeviationy;
    float gyrox;
    float gyroy;
    float accx;
    float accy;
    float accz;

    float errgx,errgy,errax,erray,erraz;

    public string[] data;
    
    // Kalman filter parameters
    KalmanFilter kalmanX;
    KalmanFilter kalmanY;
    KalmanFilter kalmanYa;
    KalmanFilter kalmanXa;
    [SerializeField]
    TMP_Text angText;
    public int gg;
    int sample_size = 500;
    int scale = 10;

    bool cal=false;
    float alpha = 0.91f;

    public GameObject leg;

    void Start()
    {
        // Initialize Kalman filters
        kalmanX = new KalmanFilter();
        kalmanY = new KalmanFilter();
        kalmanXa=new KalmanFilter();
        kalmanYa=new KalmanFilter();
        kalmanX.SetState(gyrox);
        kalmanY.SetState(gyroy);
        errgx=PlayerPrefs.GetFloat("errgx");
        errgy=PlayerPrefs.GetFloat("errgy");
        errax=PlayerPrefs.GetFloat("errax");
        erray=PlayerPrefs.GetFloat("erray");
    }

    void calibration()
    {
        // Calibration code...
        float sumgx = 0;
        float sumgy = 0;
        float sumax = 0;
        int i = 0;
        cal=false;
        while (i < sample_size)
        {   
            gyrox = float.Parse(data[0]);
            gyroy = float.Parse(data[1]);
            accx = float.Parse(data[2]);
            sumgx += gyrox;
            sumgy += gyroy;
            sumax += accx;
            i += 1;
        }
        errgx = sumgx / sample_size;
        errgy = sumgy / sample_size;
        errax = sumax / sample_size;
        // Initialize Kalman filter states after calibration
        PlayerPrefs.SetFloat("errgx", errgx);
        PlayerPrefs.SetFloat("errgy", errgy);
        PlayerPrefs.SetFloat("errax", errax);
        Debug.Log("cali complete");
        resetminmax();
    }
    public void cancal()
    {
        cal=true;
    }
    void FixedUpdate()
    {
        if (data.Length >= 3)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {   
                Debug.Log("lol");
                cancal();
            }

            if(cal)
            {
                calibration();
            }
            // Update sensor readings
            gyrox = float.Parse(data[0]) + (-1) * errgx;
            gyroy = float.Parse(data[1]) + (-1) * errgy;
            accx = float.Parse(data[2]) + (-1) * errax;
            
            if(accy>10)
            {
                Debug.Log("Punched");
            }
            // Kalman filter prediction and update
            /*float filteredX = kalmanX.PredictAndUpdate(gyrox);
            float filteredY = kalmanY.PredictAndUpdate(gyroy);
            float acccx = kalmanY.PredictAndUpdate(accx);
            float acccy = kalmanY.PredictAndUpdate(accy);*/

            int angle_x = (int)gyrox;//(int)((alpha * accx + (1 - alpha) * filteredX) * scale);
            int angle_y = (int)gyroy;//(int)((alpha * accy + (1 - alpha) * filteredY) * scale);
            int angle_z = (int)accx;
          
            switch (js)
            {
                
                case joint.elbow:
                    leg.transform.localEulerAngles =  Vector3.Lerp(new Vector3(leg.transform.localRotation.x,leg.transform.localRotation.y,leg.transform.localRotation.z),(new Vector3(-angle_z,leg.transform.localRotation.y,leg.transform.localRotation.z)),1f);
                    gg=angle_z;
                if(gg>max)
                    {
                        max=gg;
                    }
                    if(gg<min)
                    {
                        min=gg;
                    }
                angText.text = "ANGLE: "+gg.ToString()+"° ";
                    break;

                case joint.knee:
                    angle_y=-1*angle_y+kneedeviation;
                    leg.transform.localEulerAngles =  Vector3.Lerp(new Vector3(leg.transform.localRotation.x,leg.transform.localRotation.y,leg.transform.localRotation.z),(new Vector3(-1*angle_y,leg.transform.localRotation.y,leg.transform.localRotation.z)),1f);  
                    gg=((int)(-2 * (Mathf.Rad2Deg * Mathf.Acos(leg.transform.localRotation.w))))*-1;
                if(gg>max)
                    {
                        max=gg;
                    }
                    if(gg<min)
                    {
                        min=gg;
                    }
                angText.text = "ANGLE: "+gg.ToString()+"° ";
                    break;
                case joint.wrist:
                     leg.transform.localEulerAngles =  Vector3.Lerp(new Vector3(leg.transform.localRotation.x,leg.transform.localRotation.y,leg.transform.localRotation.z),(new Vector3(leg.transform.localRotation.x,-1*angle_x-wristdeviationx+kneedeviation,angle_y-wristdeviationy)),1f);
                     break;
            }
            switch (ang){
            case angle.x:
                break;
            case angle.y:
                break;
            case angle.xy:
                angText.text = "ANGLEX: "+(int)(2 * (Mathf.Rad2Deg * Mathf.Acos(leg.transform.localRotation.z)))+"° ANGLEY:"+(int)(2 * (Mathf.Rad2Deg * Mathf.Acos(leg.transform.localRotation.y)));
                break;
            }
            
        }
    }
    public void resetminmax()
    {
        max=0;
        min=0;
    }
}

public class KalmanFilter
{
    private float Xk; // State estimate
    private float Pk; // Estimate error covariance

    private float Q = 0.01f; // Process noise covariance
    private float R = 0.1f;  // Measurement noise covariance

    public void SetState(float initialState)
    {
        Xk = initialState;
        Pk = 1.0f; // Initial covariance estimation
    }

    public float PredictAndUpdate(float measurement)
    {
        // Prediction
        float Xk_ = Xk;
        float Pk_ = Pk + Q;

        // Kalman gain
        float K = Pk_ / (Pk_ + R);

        // Update state
        Xk = Xk_ + K * (measurement - Xk_);
        Pk = (1 - K) * Pk_;

        return Xk;
    }
}