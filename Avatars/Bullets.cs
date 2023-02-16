using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : Avatar
{
    public int Damage;
    public int Type;
    public const int TYPE_BULLET_PLAYER = 0;
    public const int TYPE_BULLET_ENEMY = 1;
    // Start is called before the first frame update

    public override void InitLogic()
    {
        
    }

    public override void StopLogic()
    {
       
    }
    public void Shoot(int _type, Vector3 _position, Vector3 _direction)
    {
        Type = _type;
        this.transform.position = _position;
        this.transform.forward = _direction;
    }

    

    private void OnCollisionEnter(Collision _collision)
    {
        if(Type == TYPE_BULLET_PLAYER)
        {
            if(_collision.gameObject.GetComponent<Enemy>() != null)
            {
                Debug.Log(this.gameObject.name + " Collision Against Enemy");
                _collision.gameObject.GetComponent<Enemy>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
            else if (_collision.gameObject.GetComponent<NPC>() != null)
            {
                _collision.gameObject.GetComponent<NPC>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
            GameObject.Destroy(this.gameObject);

        }
        if(Type == TYPE_BULLET_ENEMY)
        {
            if(_collision.gameObject.GetComponent<Player>() != null)
            {
                Debug.Log(this.gameObject.name + " Collision Against Player");
                _collision.gameObject.GetComponent<Player>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
            GameObject.Destroy(this.gameObject);
        }


    }

    void Update()
    {
        MoveToPosition(this.gameObject.transform.forward * Speed * Time.deltaTime); 
    }

    
}
