using System.Collections;
using UnityEngine;

public class HandRotate : MonoBehaviour
{
    public GameObject hand; 
    private ArticulationBody[] jointArticulationBodies;
    private int nJoints = 10;
    private float rot;

    void Start()
    {
        init();
        GetHandReference();
        RotateHand();
    }

    void Update()
    {
        hand.transform.rotation = Quaternion.Euler(rot, 0.0f, 0.0f);
    }

    void init()
    {
        hand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    void GetHandReference()
    {
        jointArticulationBodies = new ArticulationBody[nJoints];
        string pinkie_prox = "ar10/wrist_plate/circuit_support/palm1/palm2/pinkie/pinkie_proximal";
        jointArticulationBodies[0] = hand.transform.Find(pinkie_prox).GetComponent<ArticulationBody>();

        string pinkie_mid = pinkie_prox + "/pinkie_middle";
        jointArticulationBodies[1] = hand.transform.Find(pinkie_mid).GetComponent<ArticulationBody>();

        string ring_prox = "ar10/wrist_plate/circuit_support/palm1/palm2/ring/ring_proximal";
        jointArticulationBodies[2] = hand.transform.Find(ring_prox).GetComponent<ArticulationBody>();

        string ring_mid = ring_prox + "/ring_middle";
        jointArticulationBodies[3] = hand.transform.Find(ring_mid).GetComponent<ArticulationBody>();

        string middle_prox = "ar10/wrist_plate/circuit_support/palm1/palm2/middle/middle_proximal";
        jointArticulationBodies[4] = hand.transform.Find(middle_prox).GetComponent<ArticulationBody>();

        string middle_mid = middle_prox + "/middle_middle";
        jointArticulationBodies[5] = hand.transform.Find(middle_mid).GetComponent<ArticulationBody>();

        string index_prox = "ar10/wrist_plate/circuit_support/palm1/palm2/index/index_proximal";
        jointArticulationBodies[6] = hand.transform.Find(index_prox).GetComponent<ArticulationBody>();

        string index_mid = index_prox + "/index_middle";
        jointArticulationBodies[7] = hand.transform.Find(index_mid).GetComponent<ArticulationBody>();

        string thumb = "ar10/wrist_plate/circuit_support/palm1/thumb";
        jointArticulationBodies[8] = hand.transform.Find(thumb).GetComponent<ArticulationBody>();

        string thumb_prox = thumb + "/thumb_proximal";
        jointArticulationBodies[9] = hand.transform.Find(thumb_prox).GetComponent<ArticulationBody>();
    }

    private void RotateHand()
    {
        StartCoroutine(RotateHandCoroutine());
    }

    private IEnumerator RotateHandCoroutine()
    {
        for (int i = 0; i < 400; i++)
        {
            rot = i;
            yield return new WaitForSeconds(0.01f);
        }
    }
}