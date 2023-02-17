using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AreaVisionDetection : MonoBehaviour
{
	public delegate void VisionDetectionEvent(GameObject _objectDetected);
	public delegate void VisionLostEvent(GameObject _objectDetected);

	public event VisionDetectionEvent DetectionEvent;
	public event VisionLostEvent LostEvent;

	public void DispatchVisionDetectionEvent(GameObject _objectDetected)
	{
		if (DetectionEvent != null)
			DetectionEvent(_objectDetected);
	}

	public void DispatchVisionLostEvent(GameObject _objectDetected)
	{
		if (LostEvent != null)
			LostEvent(_objectDetected);
	}

	public enum TargetCharacters { PLAYER = 0, ENEMY, NPC }

	public Material Material;
	public float DetectionDistance = 8;
	public float DetectionAngle = 40;
	public float Orientation = 90;
	public float HeightToFloor = -0.75f;
	public TargetCharacters Target = TargetCharacters.PLAYER;

	public bool UseBehavior = true;
	public float HeightDetection = 4;

	private GameObject m_planeAreaVisionDetection;
	private int m_checkRadiusInstances = 10;

	private GameObject m_currentDetection = null;

	void Start()
    {
		CreateAreaVisionDetection();
	}

	private void CreateAreaVisionDetection()
    {
        if (UseBehavior)
        {
			this.gameObject.transform.rotation = Quaternion.identity;
			if (m_planeAreaVisionDetection != null)
			{
				GameObject.Destroy(m_planeAreaVisionDetection);
			}

			m_planeAreaVisionDetection = GameObject.CreatePrimitive(PrimitiveType.Plane);
			m_planeAreaVisionDetection.GetComponent<MeshCollider>().enabled = false;
			m_planeAreaVisionDetection.AddComponent<PlaneFromPoly>();
			m_planeAreaVisionDetection.transform.parent = this.transform;


			Utilities.DrawAreaVision(this.gameObject.transform.position, Orientation, m_planeAreaVisionDetection, m_checkRadiusInstances, DetectionDistance, DetectionAngle, Material, HeightToFloor);
		}
		
	}

	public void DestroyVisualArea()
    {
		if(m_planeAreaVisionDetection != null)
        {
			GameObject.Destroy(m_planeAreaVisionDetection);
        }
    }

	public void ChangeDistanceArea(float _distanceAreaDetection)
    {
		DetectionDistance = _distanceAreaDetection;
		CreateAreaVisionDetection();

	}


	

	void Update()
    {
		if (!UseBehavior) return;
		float angle = (Mathf.Atan2(this.transform.forward.x, -this.transform.forward.z) * Mathf.Rad2Deg) - Orientation;

		switch (Target)
        {
			case TargetCharacters.PLAYER:
				if (GameController.Instance.MyPlayer != null)
                {
					float heightDistance = Mathf.Abs(this.gameObject.transform.position.y - GameController.Instance.MyPlayer.transform.position.y);
					if(heightDistance < HeightDetection)
                    {
						if (Utilities.IsInsideCone(this.gameObject, angle, GameController.Instance.MyPlayer.gameObject, DetectionDistance, DetectionAngle) > 0)
						{
							if (m_currentDetection == null)
							{
								Debug.Log("<color=red>PLAYER DETECTED!!!</color>");
								m_currentDetection = GameController.Instance.MyPlayer.gameObject;
								DispatchVisionDetectionEvent(GameController.Instance.MyPlayer.gameObject);
							}
						}
						else
						{
							if (m_currentDetection != null)
							{
								m_currentDetection = null;
								DispatchVisionLostEvent(GameController.Instance.MyPlayer.gameObject);
							}
						}
					}
					
				}
				break;

			case TargetCharacters.ENEMY:
				if (LevelController.Instance != null)
                {
					for (int i = 0; i < LevelController.Instance.Enemies.Length; i++)
					{
						float heightDistance = Mathf.Abs(this.gameObject.transform.position.y - LevelController.Instance.Enemies[i].gameObject.transform.position.y);
						if (heightDistance < HeightDetection)
                        {
							if (Utilities.IsInsideCone(this.gameObject, angle, LevelController.Instance.Enemies[i].gameObject, DetectionDistance, DetectionAngle) > 0)
							{
								if (m_currentDetection == null)
								{
									Debug.Log("<color=red>ENEMY DETECTED!!!</color>");
									m_currentDetection = LevelController.Instance.Enemies[i].gameObject;
									DispatchVisionDetectionEvent(LevelController.Instance.Enemies[i].gameObject);
								}
							}
							else
							{
								if (m_currentDetection != null)
								{
									if (m_currentDetection == LevelController.Instance.Enemies[i])
									{
										m_currentDetection = null;
										DispatchVisionLostEvent(LevelController.Instance.Enemies[i].gameObject);
									}
								}
							}
						}
							
					}
				}
				break;

			case TargetCharacters.NPC:
				if (LevelController.Instance != null)
				{
					for (int i = 0; i < LevelController.Instance.NPCs.Length; i++)
					{
						float heightDistance = Mathf.Abs(this.gameObject.transform.position.y - LevelController.Instance.NPCs[i].gameObject.transform.position.y);
						if (heightDistance < HeightDetection)
						{
							if (Utilities.IsInsideCone(this.gameObject, angle, LevelController.Instance.NPCs[i].gameObject, DetectionDistance, DetectionAngle) > 0)
							{
								if (m_currentDetection == null)
								{
									Debug.Log("<color=red>NPC DETECTED!!!</color>");
									m_currentDetection = LevelController.Instance.NPCs[i].gameObject;
									DispatchVisionDetectionEvent(LevelController.Instance.NPCs[i].gameObject);
								}
							}
							else
							{
								if (m_currentDetection != null)
								{
									if (m_currentDetection == LevelController.Instance.NPCs[i])
									{
										m_currentDetection = null;
										DispatchVisionLostEvent(LevelController.Instance.NPCs[i].gameObject);
									}
								}
							}
						}
							
					}
				}
				break;
        }
	}
}
