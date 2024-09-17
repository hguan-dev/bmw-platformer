using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity.Examples;
using M2MqttUnity;


public class ButtonInput : MonoBehaviour
{
    public M2MqttUnityTest m2mqtt;
    public string Input;

    public void PublishInput(){
        m2mqtt.PublishButtonInput(Input);
        
    }


}
