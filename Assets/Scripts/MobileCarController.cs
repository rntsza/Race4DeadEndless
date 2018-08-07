using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DotTruck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn; 
}

public class MobileCarController : MonoBehaviour {
	 
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<DotTruck> truck_Infos;
	public AudioSource audioEngine;
	float motor;
	public VirtualJoystick joystick;
	public void VisualizeWheel(DotTruck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}
	void Start(){
		audioEngine = GetComponent<AudioSource>();
	}
	
	public void Update()
	{
		//motor = maxMotorTorque * Input.GetAxis("Vertical");
		motor = maxMotorTorque * joystick.Horizontal();
		float steering = maxSteeringAngle * joystick.Vertical();
		//float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
		if (brakeTorque > 0.001) {
			brakeTorque = maxMotorTorque;
			motor = 0;
		} else {
			brakeTorque = 0;
		}

		foreach (DotTruck truck_Info in truck_Infos)
		{
			if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true)
			{
				truck_Info.leftWheel.motorTorque = motor;
				truck_Info.rightWheel.motorTorque = motor;
			}

			truck_Info.leftWheel.brakeTorque = brakeTorque;
			truck_Info.rightWheel.brakeTorque = brakeTorque;

			VisualizeWheel(truck_Info);
		}
		EngineSound();
	}
	private void OnCollisionEnter(Collision tank) {
		if(tank.gameObject.CompareTag("TankZombie")){
			Time.timeScale = 0f;
		}
	} 
	public void EngineSound(){
		audioEngine.pitch = motor/maxMotorTorque + 0x1;
	} 
}