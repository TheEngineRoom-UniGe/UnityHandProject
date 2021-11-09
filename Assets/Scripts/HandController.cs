using UnityEngine;
//using System;
//using System.Threading;
using JointState = RosMessageTypes.Sensor.JointState;
using PoseStamped = RosMessageTypes.Geometry.PoseStamped;

public class HandController : MonoBehaviour
{
    private ROSConnection ros;
    public GameObject hand;
    public GameObject PinkieProximal, PinkieMiddle, RingProximal, RingMiddle, MiddleProximal, MiddleMiddle, IndexProximal, IndexMiddle, ThumbProximal, ThumbMiddle;
    private ArticulationBody[] jointArticulationBodies;
    private int nJoints = 10;
    private Quaternion baseOrientation = new Quaternion();

    void Start()
    {
        ros = ROSConnection.instance;
        GetHandReference();
        initJointStateCallback();
        initbaseCallback();
    }

    /*private void ComputeOffset(int phalange_index, PoseStamped poseMessage)
    {
        Debug.Log("computeoffset called " + phalange_index);
        Quaternion firstData = new Quaternion();
        if (phalange_index == (int)phalange.base_)
        {
            firstData.x = -(float)poseMessage.pose.orientation.y;
            firstData.y = -(float)poseMessage.pose.orientation.z;
            firstData.z = (float)poseMessage.pose.orientation.x;
            firstData.w = (float)poseMessage.pose.orientation.w;
            this.angleOffsets[phalange_index].x = 0;
            this.angleOffsets[phalange_index].y = firstData.eulerAngles.y;
            this.angleOffsets[phalange_index].z = 0;
        }
        else
        {
            firstData.x = (float)poseMessage.pose.orientation.y;
            firstData.y = -(float)poseMessage.pose.orientation.z;
            firstData.z = -(float)poseMessage.pose.orientation.x;
            firstData.w = (float)poseMessage.pose.orientation.w;
            this.angleOffsets[phalange_index].x = 0;
            this.angleOffsets[phalange_index].y = firstData.eulerAngles.y;
            this.angleOffsets[phalange_index].z = 0;
        }
        isRemovingYawOffset[phalange_index] = false;
    }*/

    void GetHandReference()
    {
        jointArticulationBodies = new ArticulationBody[nJoints];
        //string pinkie_proximal = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/pinkie/pinkie_proximal";
        string pinkie_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/pinkie/pinkie_proximal";
        jointArticulationBodies[0] = hand.transform.Find(pinkie_proximal).GetComponent<ArticulationBody>();

        string pinkie_middle = pinkie_proximal + "/pinkie_middle";
        jointArticulationBodies[1] = hand.transform.Find(pinkie_middle).GetComponent<ArticulationBody>();

        //string ring_proximal = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/ring/ring_proximal";
        string ring_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/ring/ring_proximal";
        jointArticulationBodies[2] = hand.transform.Find(ring_proximal).GetComponent<ArticulationBody>();

        string ring_middle = ring_proximal + "/ring_middle";
        jointArticulationBodies[3] = hand.transform.Find(ring_middle).GetComponent<ArticulationBody>();

        //string middle_proximal = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/middle/middle_proximal";
        string middle_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/middle/middle_proximal";
        jointArticulationBodies[4] = hand.transform.Find(middle_proximal).GetComponent<ArticulationBody>();

        string middle_middle = middle_proximal + "/middle_middle";
        jointArticulationBodies[5] = hand.transform.Find(middle_middle).GetComponent<ArticulationBody>();

        //string index_proximal = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/index/index_proximal";
        string index_proximal = "ar10/wrist_plate/circuit_support/palm1/palm2/index/index_proximal";
        jointArticulationBodies[6] = hand.transform.Find(index_proximal).GetComponent<ArticulationBody>();

        string index_middle = index_proximal + "/index_middle";
        jointArticulationBodies[7] = hand.transform.Find(index_middle).GetComponent<ArticulationBody>();

        //string thumb = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/thumb";
        string thumb = "ar10/wrist_plate/circuit_support/palm1/thumb";
        jointArticulationBodies[8] = hand.transform.Find(thumb).GetComponent<ArticulationBody>();

        string thumb_proximal = thumb + "/thumb_proximal";
        jointArticulationBodies[9] = hand.transform.Find(thumb_proximal).GetComponent<ArticulationBody>();

        /*string circuit_support_1 = "wrist_plate/circuit_support_1";
        jointArticulationBodies[10] = hand.transform.Find(circuit_support_1).GetComponent<ArticulationBody>();

        string circuit_support_2 = "wrist_plate/circuit_support_1/circuit_support_2";
        jointArticulationBodies[11] = hand.transform.Find(circuit_support_2).GetComponent<ArticulationBody>();

        string circuit_support_3 = "wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3";
        jointArticulationBodies[12] = hand.transform.Find(circuit_support_3).GetComponent<ArticulationBody>();*/
    }

    void init()
    {
        hand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        /*PinkieProximal.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        PinkieMiddle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        RingProximal.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        RingMiddle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        MiddleProximal.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        MiddleMiddle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        IndexProximal.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        IndexMiddle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        ThumbProximal.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        ThumbMiddle.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);*/

        /*for (int i = 0; i < 10; i++)
        {
            var index = jointArticulationBodies[i].xDrive;
            index.target = 0;
            jointArticulationBodies[i].xDrive = index;

            //var index3 = jointArticulationBodies[i].zDrive;
            //var index2 = jointArticulationBodies[i].yDrive;
            //index2.target = 0;
            //index3.target = 0;
            //jointArticulationBodies[i].yDrive = index2;
            //jointArticulationBodies[i].zDrive = index3;
        }*/
    }

    private void initJointStateCallback()
    {
        init();
        ros.Subscribe<JointState>("joint_states", JointStateCallback);
    }

    private void JointStateCallback(JointState joint_states)
    {
        AssignJointValue(joint_states);
    }

    private void AssignJointValue(JointState joint_states)
    {
        if (joint_states.name[0] == "servo6")
        {
            PinkieProximal.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var pinkie_p = jointArticulationBodies[0].xDrive;
            pinkie_p.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[0].xDrive = pinkie_p;*/
        }
        else if (joint_states.name[0] == "servo7")
        {
            PinkieMiddle.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var pinkie_m = jointArticulationBodies[1].xDrive;
            pinkie_m.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[1].xDrive = pinkie_m;*/
        }
        else if (joint_states.name[0] == "servo4")
        {
            RingProximal.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var ring_p = jointArticulationBodies[2].xDrive;
            ring_p.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[2].xDrive = ring_p;*/
        }
        else if (joint_states.name[0] == "servo5")
        {
            RingMiddle.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var ring_m = jointArticulationBodies[3].xDrive;
            ring_m.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[3].xDrive = ring_m;*/
        }
        else if (joint_states.name[0] == "servo2")
        {
            MiddleProximal.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var middle_p = jointArticulationBodies[4].xDrive;
            middle_p.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[4].xDrive = middle_p;*/
        }
        else if (joint_states.name[0] == "servo3")
        {
            MiddleMiddle.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var middle_m = jointArticulationBodies[5].xDrive;
            middle_m.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[5].xDrive = middle_m;*/
        }
        else if (joint_states.name[0] == "servo0")
        {
            IndexProximal.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var index_p = jointArticulationBodies[6].xDrive;
            index_p.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[6].xDrive = index_p;*/
        }
        else if (joint_states.name[0] == "servo1")
        {
            IndexMiddle.transform.Rotate(0.0f, 0.0f, (float)joint_states.position[0]);
            /*var index_m = jointArticulationBodies[7].xDrive;
            index_m.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[7].xDrive = index_m;*/
        }
        else if (joint_states.name[0] == "servo8")
        {
            ThumbProximal.transform.Rotate(0.0f, (float)joint_states.position[0], 0.0f);
            /*var thumb_p = jointArticulationBodies[8].xDrive;
            thumb_p.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[8].xDrive = thumb_p;*/
        }
        else if (joint_states.name[0] == "servo9")
        {
            ThumbMiddle.transform.Rotate(0.0f, (float)joint_states.position[0], 0.0f);
            /*var thumb_m = jointArticulationBodies[9].xDrive;
            thumb_m.target = (float)((Mathf.Rad2Deg * joint_states.position[0] / (float)steps) * i);
            jointArticulationBodies[9].xDrive = thumb_m;*/
        }
    }

    private void initbaseCallback()
    {
        ros.Subscribe<PoseStamped>("backPose", baseCallbackNoJoints);
    }

    private void baseCallbackNoJoints(PoseStamped poseMessage)
    {
        baseOrientation.x = (float)poseMessage.pose.orientation.x;
        baseOrientation.y = (float)poseMessage.pose.orientation.y;
        baseOrientation.z = (float)poseMessage.pose.orientation.z;
        baseOrientation.w = (float)poseMessage.pose.orientation.w;

        Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
        float tempy = baseOrientation_euler.y;
        baseOrientation_euler.y = baseOrientation_euler.x;
        baseOrientation_euler.x = tempy + 180;

        float tempz = baseOrientation_euler.z;
        baseOrientation_euler.z = baseOrientation_euler.y;
        baseOrientation_euler.y = -tempz ;

        hand.transform.rotation = Quaternion.Euler(baseOrientation_euler);
    }
}