
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Vector3 GetDirection(Vector3 target, Vector3 origin)
    {
        return (target - origin).normalized;
    }

	public static float IsInsideCone(GameObject _source, float _angle, GameObject _objective, float _rangeDetection, float _angleDetection)
	{
		float distance = Vector3.Distance(new Vector3(_source.transform.position.x, 0, _source.transform.position.z),
										 new Vector3(_objective.transform.position.x, 0, _objective.transform.position.z));
		if (distance < _rangeDetection)
		{
			float yaw = _angle * Mathf.Deg2Rad;
			Vector2 pos = new Vector2(_source.transform.position.x, _source.transform.position.z);

			Vector2 v1 = new Vector2((float)Mathf.Cos(yaw), (float)Mathf.Sin(yaw));
			Vector2 v2 = new Vector2(_objective.transform.position.x - pos.x, _objective.transform.position.z - pos.y);

			// Angle detection
			float moduloV2 = v2.magnitude;
			if (moduloV2 == 0)
			{
				v2.x = 0.0f;
				v2.y = 0.0f;
			}
			else
			{
				v2.x = v2.x / moduloV2;
				v2.y = v2.y / moduloV2;
			}
			float angleCreated = (v1.x * v2.x) + (v1.y * v2.y);
			float angleResult = Mathf.Cos(_angleDetection * Mathf.Deg2Rad);

			if (angleCreated > angleResult)
			{
				return (distance);
			}
			else
			{
				return (-1);
			}
		}
		else
		{
			return (-1);
		}
	}

	public static void DrawAreaVision(Vector3 _posOrigin, float _orientation, GameObject _planeAreaVision, int _checkRadiusInstances, float _viewDistance, float _angleView, Material _material, float _heightToFloor)
	{
		List<Vector3> areaDetection = new List<Vector3>();
		Vector3 posOrigin = _posOrigin;
		posOrigin.y += _heightToFloor;
		areaDetection.Add(posOrigin);

		float totalAngle = 2 * _angleView * Mathf.Deg2Rad;
		float entryAngle = (_orientation + _angleView) * Mathf.Deg2Rad;
		float x = _viewDistance * Mathf.Cos(entryAngle);
		float z = _viewDistance * Mathf.Sin(entryAngle);

		Vector3 posTarget = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
		areaDetection.Add(posTarget);

		float thetaScale = totalAngle / _checkRadiusInstances;
		for (int i = 0; i < _checkRadiusInstances; i++)
		{
			entryAngle -= thetaScale;
			x = _viewDistance * Mathf.Cos(entryAngle);
			z = _viewDistance * Mathf.Sin(entryAngle);

			Vector3 posTargetRadius = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
			areaDetection.Add(posTargetRadius);
		}

		float endAngle = (_orientation - _angleView) * Mathf.Deg2Rad;
		x = _viewDistance * Mathf.Cos(endAngle);
		z = _viewDistance * Mathf.Sin(endAngle);
		Vector3 posTargetEnd = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
		areaDetection.Add(posTargetEnd);
		areaDetection.Add(posOrigin);

		_planeAreaVision.GetComponent<PlaneFromPoly>().Init(areaDetection.ToArray(), _material);
		_planeAreaVision.GetComponent<PlaneFromPoly>().Logic(new Vector3(posOrigin.x, posOrigin.y, posOrigin.z), posOrigin.y);
	}

}
