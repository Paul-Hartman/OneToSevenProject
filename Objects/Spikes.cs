using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour

{
    private PatrolWaypoints m_partolComponent;
        

    public int DamageLife = 10;
    // Start is called before the first frame update
    void Start()
    {
        m_partolComponent = this.GetComponent<PatrolWaypoints>();
        if(m_partolComponent != null)
        {
            m_partolComponent.Activatepatrol(5);
        }

        SystemEventController.Instance.Event += ProcessSystemEvent;

    }
    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {
            this.gameObject.transform.localScale += new Vector3(1f, 1f, 1f);
            Debug.Log("<color= purple>Spikes have recieved the event of Enemy Dead!!</color>");
        }
        if (_nameEvent == SystemEventController.EVENT_NPC_DEAD)
        {
            this.gameObject.transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
