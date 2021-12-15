using UnityEngine;
using JointState = RosMessageTypes.Sensor.JointState;

public class HandController1 : MonoBehaviour
{
    public GameObject hand;
    private ROSConnection ros;
    private ArticulationBody[] jointArticulationBodies;
    private int nJoints = 10, n = 0;

    void Start()
    {
        ros = ROSConnection.instance;
        GetHandReference();
        initJointStateCallback();
    }

    void GetHandReference()
    {
        jointArticulationBodies = new ArticulationBody[nJoints];
        string pinkie_proximal = "wrist_plate/circuit_support/palm1/palm2/pinkie/pinkie_proximal";
        jointArticulationBodies[0] = hand.transform.Find(pinkie_proximal).GetComponent<ArticulationBody>();

        string pinkie_middle = pinkie_proximal + "/pinkie_middle";
        jointArticulationBodies[1] = hand.transform.Find(pinkie_middle).GetComponent<ArticulationBody>();

        string ring_proximal = "wrist_plate/circuit_support/palm1/palm2/ring/ring_proximal";
        jointArticulationBodies[2] = hand.transform.Find(ring_proximal).GetComponent<ArticulationBody>();

        string ring_middle = ring_proximal + "/ring_middle";
        jointArticulationBodies[3] = hand.transform.Find(ring_middle).GetComponent<ArticulationBody>();

        string middle_proximal = "wrist_plate/circuit_support/palm1/palm2/middle/middle_proximal";
        jointArticulationBodies[4] = hand.transform.Find(middle_proximal).GetComponent<ArticulationBody>();

        string middle_middle = middle_proximal + "/middle_middle";
        jointArticulationBodies[5] = hand.transform.Find(middle_middle).GetComponent<ArticulationBody>();

        string index_proximal = "wrist_plate/circuit_support/palm1/palm2/index/index_proximal";
        jointArticulationBodies[6] = hand.transform.Find(index_proximal).GetComponent<ArticulationBody>();

        string index_middle = index_proximal + "/index_middle";
        jointArticulationBodies[7] = hand.transform.Find(index_middle).GetComponent<ArticulationBody>();

        string thumb = "wrist_plate/circuit_support/palm1/thumb";
        jointArticulationBodies[8] = hand.transform.Find(thumb).GetComponent<ArticulationBody>();

        string thumb_proximal = thumb + "/thumb_proximal";
        jointArticulationBodies[9] = hand.transform.Find(thumb_proximal).GetComponent<ArticulationBody>();
    }

    private void initJointStateCallback()
    {
        ros.Subscribe<JointState>("joint_states", JointStateCallback);
    }

    private void JointStateCallback(JointState joint_states)
    {
        n += 1;
        if(n == 10)
        {
            AssignJointValue(joint_states.position);
            n = 0;
        }
    }

    private void AssignJointValue(double[] joint_values)
    {
        int steps = 30;
        for (int i = 1; i <= steps; i++)
        {
            var pinkie_p = jointArticulationBodies[0].xDrive;
            pinkie_p.target = (float)((Mathf.Rad2Deg * joint_values[0] / (float)steps) * i);
            jointArticulationBodies[0].xDrive = pinkie_p;

            var pinkie_m = jointArticulationBodies[1].xDrive;
            pinkie_m.target = (float)((Mathf.Rad2Deg * joint_values[8] / (float)steps) * i);
            jointArticulationBodies[1].xDrive = pinkie_m;

            var ring_p = jointArticulationBodies[2].xDrive;
            ring_p.target = (float)((Mathf.Rad2Deg * joint_values[2] / (float)steps) * i);
            jointArticulationBodies[2].xDrive = ring_p;

            var ring_m = jointArticulationBodies[3].xDrive;
            ring_m.target = (float)((Mathf.Rad2Deg * joint_values[10] / (float)steps) * i);
            jointArticulationBodies[3].xDrive = ring_m;

            var middle_p = jointArticulationBodies[4].xDrive;
            middle_p.target = (float)((Mathf.Rad2Deg * joint_values[4] / (float)steps) * i);
            jointArticulationBodies[4].xDrive = middle_p;

            var middle_m = jointArticulationBodies[5].xDrive;
            middle_m.target = (float)((Mathf.Rad2Deg * joint_values[12] / (float)steps) * i);
            jointArticulationBodies[5].xDrive = middle_m;

            var index_p = jointArticulationBodies[6].xDrive;
            index_p.target = (float)((Mathf.Rad2Deg * joint_values[6] / (float)steps) * i);
            jointArticulationBodies[6].xDrive = index_p;

            var index_m = jointArticulationBodies[7].xDrive;
            index_m.target = (float)((Mathf.Rad2Deg * joint_values[14] / (float)steps) * i);
            jointArticulationBodies[7].xDrive = index_m;

            var thumb_p = jointArticulationBodies[8].xDrive;
            thumb_p.target = (float)((Mathf.Rad2Deg * joint_values[24] / (float)steps) * i);
            jointArticulationBodies[8].xDrive = thumb_p;

            var thumb_m = jointArticulationBodies[9].xDrive;
            thumb_m.target = (float)((Mathf.Rad2Deg * joint_values[25] / (float)steps) * i);
            jointArticulationBodies[9].xDrive = thumb_m;
        }
    }
}