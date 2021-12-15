using System.Collections;
using UnityEngine;

public class HandClose : MonoBehaviour
{
    public GameObject hand;
    private ArticulationBody[] jointArticulationBodies;
    private double[] targets_close = { 70.0f, 65.0f, 60.0f, 65.0f, 70.0f, 65.0f, 70.0f, 65.0f, 80.0f, 50.0f };
    private int nJoints = 10;

    void Start()
    {
        GetHandReference();
        init();
        CloseHand();
    }

    void init()
    {
        // inizializzazione a zero degli angoli dei vari giunti.
        for (int i = 0; i < 10; i++)
        {
            var index = jointArticulationBodies[i].xDrive;
            index.target = 0;
            jointArticulationBodies[i].xDrive = index;
        }
    }

    void GetHandReference()
    {
        jointArticulationBodies = new ArticulationBody[nJoints];
        string pinkie_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/pinkie/pinkie_proximal";
        jointArticulationBodies[0] = hand.transform.Find(pinkie_proximal).GetComponent<ArticulationBody>();

        string pinkie_middle = pinkie_proximal + "/pinkie_middle";
        jointArticulationBodies[1] = hand.transform.Find(pinkie_middle).GetComponent<ArticulationBody>();

        string ring_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/ring/ring_proximal";
        jointArticulationBodies[2] = hand.transform.Find(ring_proximal).GetComponent<ArticulationBody>();

        string ring_middle = ring_proximal + "/ring_middle";
        jointArticulationBodies[3] = hand.transform.Find(ring_middle).GetComponent<ArticulationBody>();
        
        string middle_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/middle/middle_proximal";
        jointArticulationBodies[4] = hand.transform.Find(middle_proximal).GetComponent<ArticulationBody>();

        string middle_middle = middle_proximal + "/middle_middle";
        jointArticulationBodies[5] = hand.transform.Find(middle_middle).GetComponent<ArticulationBody>();

        string index_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/index/index_proximal";
        jointArticulationBodies[6] = hand.transform.Find(index_proximal).GetComponent<ArticulationBody>();

        string index_middle = index_proximal + "/index_middle";
        jointArticulationBodies[7] = hand.transform.Find(index_middle).GetComponent<ArticulationBody>();

        string thumb = "ar10/wrist_plate/circuit_support/palm1/thumb";
        jointArticulationBodies[8] = hand.transform.Find(thumb).GetComponent<ArticulationBody>();

        string thumb_proximal = thumb + "/thumb_proximal";
        jointArticulationBodies[9] = hand.transform.Find(thumb_proximal).GetComponent<ArticulationBody>();
    }

    private void CloseHand()
    {
        StartCoroutine(CloseHandCoroutine());
    }

    private IEnumerator CloseHandCoroutine()
    {
        int steps = 30;
        for (int i = 0; i <= steps; i++)
        {
            var pinkie_p = jointArticulationBodies[0].xDrive;
            pinkie_p.target = (float)((targets_close[0] / (float)steps) * i);
            jointArticulationBodies[0].xDrive = pinkie_p;

            var pinkie_m = jointArticulationBodies[1].xDrive;
            pinkie_m.target = (float)((targets_close[1] / (float)steps) * i);
            jointArticulationBodies[1].xDrive = pinkie_m;

            var ring_p = jointArticulationBodies[2].xDrive;
            ring_p.target = (float)((targets_close[2] / (float)steps) * i);
            jointArticulationBodies[2].xDrive = ring_p;

            var ring_m = jointArticulationBodies[3].xDrive;
            ring_m.target = (float)((targets_close[3] / (float)steps) * i);
            jointArticulationBodies[3].xDrive = ring_m;

            var middle_p = jointArticulationBodies[4].xDrive;
            middle_p.target = (float)((targets_close[4] / (float)steps) * i);
            jointArticulationBodies[4].xDrive = middle_p;

            var middle_m = jointArticulationBodies[5].xDrive;
            middle_m.target = (float)((targets_close[5] / (float)steps) * i);
            jointArticulationBodies[5].xDrive = middle_m;

            var index_p = jointArticulationBodies[6].xDrive;
            index_p.target = (float)((targets_close[6] / (float)steps) * i);
            jointArticulationBodies[6].xDrive = index_p;

            var index_m = jointArticulationBodies[7].xDrive;
            index_m.target = (float)((targets_close[7] / (float)steps) * i);
            jointArticulationBodies[7].xDrive = index_m;

            var thumb_p = jointArticulationBodies[8].xDrive;
            thumb_p.target = (float)((targets_close[8] / (float)steps) * i);
            jointArticulationBodies[8].xDrive = thumb_p;

            var thumb_m = jointArticulationBodies[9].xDrive;
            thumb_m.target = (float)((targets_close[9] / (float)steps) * i);
            jointArticulationBodies[9].xDrive = thumb_m;

            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(4.0f);
        GoToRestPos();
    }

    public void GoToRestPos()
    {
        StartCoroutine(GoToRestPosCoroutine());
    }

    private IEnumerator GoToRestPosCoroutine()
    {
        int steps = 6;
        for (double i = 1; i <= steps; i += 0.2)
        {
            var pinkie_p = jointArticulationBodies[0].xDrive;
            pinkie_p.target = (float)(targets_close[0] / i);
            jointArticulationBodies[0].xDrive = pinkie_p;

            var pinkie_m = jointArticulationBodies[1].xDrive;
            pinkie_m.target = (float)(targets_close[1] / i);
            jointArticulationBodies[1].xDrive = pinkie_m;

            var ring_p = jointArticulationBodies[2].xDrive;
            ring_p.target = (float)(targets_close[2] / i);
            jointArticulationBodies[2].xDrive = ring_p;

            var ring_m = jointArticulationBodies[3].xDrive;
            ring_m.target = (float)(targets_close[3] / i);
            jointArticulationBodies[3].xDrive = ring_m;

            var middle_p = jointArticulationBodies[4].xDrive;
            middle_p.target = (float)(targets_close[4] / i);
            jointArticulationBodies[4].xDrive = middle_p;

            var middle_m = jointArticulationBodies[5].xDrive;
            middle_m.target = (float)(targets_close[5] / i);
            jointArticulationBodies[5].xDrive = middle_m;

            var index_p = jointArticulationBodies[6].xDrive;
            index_p.target = (float)(targets_close[6] / i);
            jointArticulationBodies[6].xDrive = index_p;

            var index_m = jointArticulationBodies[7].xDrive;
            index_m.target = (float)(targets_close[7] / i);
            jointArticulationBodies[7].xDrive = index_m;

            var thumb_p = jointArticulationBodies[8].xDrive;
            thumb_p.target = (float)(targets_close[8] / i);
            jointArticulationBodies[8].xDrive = thumb_p;

            var thumb_m = jointArticulationBodies[9].xDrive;
            thumb_m.target = (float)(targets_close[9] / i);
            jointArticulationBodies[9].xDrive = thumb_m;

            yield return new WaitForSeconds(0.02f);
        }
    }
}
