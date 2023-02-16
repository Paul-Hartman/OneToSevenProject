using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{   
    private static LevelController _instance;
    public static LevelController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<LevelController>();
            }
            return _instance;
        }
    }

    public Enemy[] Enemies;
    public NPC[] NPCs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool PlayerHasKilledAllEnemies()
    {

        bool areAllEnemiesDead = true;
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                areAllEnemiesDead = false;
            }
        }
        return areAllEnemiesDead;
    }

    public bool AlarmEnemyNearby(float distanceDetection)
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                float distanceToPlayer = Vector3.Distance(Enemies[i].transform.position, GameController.Instance.MyPlayer.transform.position);
                if (distanceToPlayer < distanceDetection)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public bool AlarmNPCNearby(float distanceDetection)
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null)
            {
                float distanceToPlayer = Vector3.Distance(NPCs[i].transform.position, GameController.Instance.MyPlayer.transform.position);
                if (distanceToPlayer < distanceDetection)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public void InitializeLogicGameElements()
    {
        
        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].InitLogic();
        }
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i].InitLogic();
        }

    }

    public void StopLogicGameElements()
    {
        
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                Enemies[i].StopLogic();
            }

        }
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null)
            {
                NPCs[i].StopLogic();
            }

        }

    }

    public void Destroy()
    {
        if(_instance != null)
        {
            _instance = null;
            for(int i = 0;i < Enemies.Length; i++)
            {
                if(Enemies[i] != null)
                {
                    GameObject.Destroy(Enemies[i].gameObject);
                }
                
            }
            for(int i = 0;i< NPCs.Length; i++)
            {
                if (NPCs[i] != null)
                {
                    GameObject.Destroy(NPCs[i].gameObject);
                }
            }

            Bullets[] bullets = GameObject.FindObjectsOfType<Bullets>();
            for(int i = 0;bullets.Length < 0; i++)
            {
                GameObject.Destroy(bullets[i].gameObject);
            }

            GameObject.Destroy(this.gameObject);
        }
    }
}
