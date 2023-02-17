using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = GameController.TAG_FLOOR;
    }

    private void OnCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject.GetComponent<Player>() != null)
        {
            GameController.Instance.MyPlayer.transform.parent = this.transform;
            GameController.Instance.MyPlayer.UseRigidBody = false;
        }
        
    }

    private void OnCollisionExit(Collision _collision)
    {
        if (_collision.gameObject.GetComponent<Player>() != null)
        {
            GameController.Instance.MyPlayer.transform.parent = null;
            GameController.Instance.MyPlayer.UseRigidBody = true;
        }

    }
}
