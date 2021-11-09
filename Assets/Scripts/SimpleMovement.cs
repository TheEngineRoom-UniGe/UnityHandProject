using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using PoseStamped = RosMessageTypes.Geometry.PoseStamped;
using JointState = RosMessageTypes.Sensor.JointState;
using Str = RosMessageTypes.Std.String;
using Quaternion = UnityEngine.Quaternion;
using Transform = UnityEngine.Transform;
using Vector3 = UnityEngine.Vector3;

//using Unity.Robotics.ROSTCPConnector;

public class SimpleMovement : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    public GameObject hand;
    //public GameObject root;
    // Hardcoded variables 
    private int numRobotJoints = 21;
    private readonly float jointAssignmentWait = 0.005f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    // Publish the hand's orientation every N seconds
    public float publishMessageFrequency = 0.15f;

    // Flag used to send or not data to unity. 
    public bool isSendingMessages = false;

    private ArticulationBody[] jointArticulationBodies;

    // Variables used to send the current position to ros
    Vector3 indexProximalPosition = new Vector3();
    Vector3 indexIntermedPosition = new Vector3();

    // Hand Base Quaternion def..
    private Quaternion baseOrientation = new Quaternion();

    // Thumb Quaternion definitions
    private Quaternion proximalThumbOrientation = new Quaternion();
    private Quaternion distalThumbOrientation = new Quaternion();

    // Proximal Quaternion definitions
    private Quaternion proximalIndexOrientation = new Quaternion();
    private Quaternion proximalMiddleOrientation = new Quaternion();
    private Quaternion proximalRingOrientation = new Quaternion();
    private Quaternion proximalPinkieOrientation = new Quaternion();

    // Intermediate quaternion definitions...
    private Quaternion interIndexOrientation = new Quaternion();
    private Quaternion interMiddleOrientation = new Quaternion();
    private Quaternion interRingOrientation = new Quaternion();
    private Quaternion interPinkieOrientation = new Quaternion();

    // Flag used to remove the yaw offset from the input imu data
    private bool[] isRemovingYawOffset = { true, true, true, true, true, true, true, true, true, true,true,false,false };
    //public bool[] isRemovingYawOffset = { false, false, false, false, false, false, false, false, false, false, false };
    //public bool[] isRemovingYawOffset = new bool[11];

    // Used to remove the initial yaw-offset
    public Vector3[] angleOffsets = new Vector3[11];

    private void initJointArticArray()
    {
        // General
        jointArticulationBodies[(int)joint.base_0] = hand.transform.Find(Path.base_0).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.base_] = hand.transform.Find(Path.base_).GetComponent<ArticulationBody>();
        //jointArticulationBodies[(int)joint.root] = hand.transform.Find(Path.root).GetComponent<ArticulationBody>();

        // Thumb
        jointArticulationBodies[(int)joint.thumb_meta] = hand.transform.Find(Path.thumb_meta).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.thumb_proximal] = hand.transform.Find(Path.thumb_proximal).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.thumb_distal] = hand.transform.Find(Path.thumb_distal).GetComponent<ArticulationBody>();

        // Index
        jointArticulationBodies[(int)joint.index_meta] = hand.transform.Find(Path.index_meta).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.index_proximal] = hand.transform.Find(Path.index_proximal).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.index_inter] = hand.transform.Find(Path.index_inter).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.index_distal] = hand.transform.Find(Path.index_distal).GetComponent<ArticulationBody>();

        // Middle finger
        jointArticulationBodies[(int)joint.middle_meta] = hand.transform.Find(Path.middle_meta).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.middle_proximal] = hand.transform.Find(Path.middle_proximal).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.middle_inter] = hand.transform.Find(Path.middle_inter).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.middle_distal] = hand.transform.Find(Path.middle_distal).GetComponent<ArticulationBody>();

        // Ring 
        jointArticulationBodies[(int)joint.ring_meta] = hand.transform.Find(Path.ring_meta).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.ring_proximal] = hand.transform.Find(Path.ring_proximal).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.ring_inter] = hand.transform.Find(Path.ring_inter).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.ring_distal] = hand.transform.Find(Path.ring_distal).GetComponent<ArticulationBody>();

        // Pinkie
        jointArticulationBodies[(int)joint.pinky_meta] = hand.transform.Find(Path.pinky_meta).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.pinky_proximal] = hand.transform.Find(Path.pinky_proximal).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.pinky_inter] = hand.transform.Find(Path.pinky_inter).GetComponent<ArticulationBody>();
        jointArticulationBodies[(int)joint.pinky_distal] = hand.transform.Find(Path.pinky_distal).GetComponent<ArticulationBody>();
    }

    private void initThumbCallbacks()
    {
        ros.Subscribe<PoseStamped>(RosTopicsNames.thumbProximal, ThumbProximalCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.thumbDistal, ThumbDistalCallback);
    }

    private void initProximalCallbacks()
    {
        ros.Subscribe<PoseStamped>(RosTopicsNames.basejoint, baseCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.indexProximal, IndexProximalCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.middleProximal, MiddleProximalCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.ringProximal, RingProximalCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.pinkieProximal, PinkieProximalCallback);
    }

    private void initIntermediateCallbacks()
    {
        ros.Subscribe<PoseStamped>(RosTopicsNames.indexIntermediate, IndexIntermediateCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.middleIntermediate, MiddleIntermediateCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.ringIntermediate, RingIntermediateCallback);
        ros.Subscribe<PoseStamped>(RosTopicsNames.pinkieIntermediate, PinkieIntermediateCallback);
    }

    public void Move()
    {
        StartCoroutine(PerformMovement());
    }
    private IEnumerator PerformMovement()
    {
        yield return new WaitForSeconds(jointAssignmentWait);
    }

    private void ComputeOffset(int phalange_index, PoseStamped poseMessage)
    {
        Debug.Log("computeoffset called " + phalange_index);
        Quaternion firstData = new Quaternion();
        //firstData.x = -(float)poseMessage.pose.orientation.y;
        //firstData.y = -(float)poseMessage.pose.orientation.z;
        //firstData.z = (float)poseMessage.pose.orientation.x;
        //firstData.w = (float)poseMessage.pose.orientation.w;
        if (phalange_index == (int)phalange.base_ )
        {
            firstData.x = -(float)poseMessage.pose.orientation.y;
            firstData.y = -(float)poseMessage.pose.orientation.z;
            firstData.z = (float)poseMessage.pose.orientation.x;
            firstData.w = (float)poseMessage.pose.orientation.w;
            this.angleOffsets[phalange_index].x = 0;// firstData.eulerAngles.x;   //0
            this.angleOffsets[phalange_index].y = firstData.eulerAngles.y;
            this.angleOffsets[phalange_index].z = 0;// firstData.eulerAngles.z;//0
        }
        else
        { 
            firstData.x = (float)poseMessage.pose.orientation.y;
            firstData.y = -(float)poseMessage.pose.orientation.z;
            firstData.z = -(float)poseMessage.pose.orientation.x;
            firstData.w = (float)poseMessage.pose.orientation.w;
            this.angleOffsets[phalange_index].x = 0;//firstData.eulerAngles.x;   //0
            this.angleOffsets[phalange_index].y = firstData.eulerAngles.y;
            this.angleOffsets[phalange_index].z = 0;//  firstData.eulerAngles.z; //0
        }
        isRemovingYawOffset[phalange_index] = false;
    }

    //===================================== THUMB =============================================
    void ThumbProximalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.thumb_proximal;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            proximalThumbOrientation.x = (float)poseMessage.pose.orientation.y;
            proximalThumbOrientation.y = -(float)poseMessage.pose.orientation.z;
            proximalThumbOrientation.z = -(float)poseMessage.pose.orientation.x;
            proximalThumbOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = proximalThumbOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            proximalThumbOrientation = Quaternion.Euler(temp);
            proximalThumbOrientation = Quaternion.Euler(proximalThumbOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("TH_I" + proximalThumbOrientation.eulerAngles);
    }

    void ThumbDistalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.thumb_distal;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            distalThumbOrientation.x = (float)poseMessage.pose.orientation.y;
            distalThumbOrientation.y = -(float)poseMessage.pose.orientation.z;
            distalThumbOrientation.z = -(float)poseMessage.pose.orientation.x;
            distalThumbOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = distalThumbOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            distalThumbOrientation = Quaternion.Euler(temp);
            distalThumbOrientation = Quaternion.Euler(distalThumbOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("TH_I" + distalThumbOrientation.eulerAngles);
    }

    //===================================== INDEX ==============================================
    void IndexProximalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.index_proximal;

        if (isRemovingYawOffset[(int)phalange.index_proximal])
            ComputeOffset(i, poseMessage);
        else
        {
            proximalIndexOrientation.x = (float)poseMessage.pose.orientation.y;
            proximalIndexOrientation.y = -(float)poseMessage.pose.orientation.z;
            proximalIndexOrientation.z = -(float)poseMessage.pose.orientation.x;
            proximalIndexOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = proximalIndexOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            proximalIndexOrientation = Quaternion.Euler(temp);

            proximalIndexOrientation = Quaternion.Euler(proximalIndexOrientation.eulerAngles - this.angleOffsets[i]);
            //Debug.Log("IP" + proximalIndexOrientation.eulerAngles);

            // Add offset
            //Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
            //Debug.Log("PROVA: x:" + baseOrientation_euler.x + " y: " + baseOrientation_euler.y + " z: " + baseOrientation_euler.z);
            //baseOrientation_euler.x = baseOrientation_euler.x + 180;
            //baseOrientation_euler.z = baseOrientation_euler.z + 90;
            //baseOrientation_euler.y = baseOrientation_euler.y + 180;
        }
    }

    void IndexIntermediateCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.index_inter;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            interIndexOrientation.x = (float)poseMessage.pose.orientation.y;
            interIndexOrientation.y = -(float)poseMessage.pose.orientation.z;
            interIndexOrientation.z = -(float)poseMessage.pose.orientation.x;
            interIndexOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = interIndexOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            interIndexOrientation = Quaternion.Euler(temp);
            interIndexOrientation = Quaternion.Euler(interIndexOrientation.eulerAngles - this.angleOffsets[i]);
        }
        Debug.Log("II" + interIndexOrientation.eulerAngles);
    }

    //===================================== MIDDLE ==============================================
    void MiddleProximalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.middle_proximal;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            proximalMiddleOrientation.x = (float)poseMessage.pose.orientation.y;
            proximalMiddleOrientation.y = -(float)poseMessage.pose.orientation.z;
            proximalMiddleOrientation.z = -(float)poseMessage.pose.orientation.x;
            proximalMiddleOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = proximalMiddleOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            proximalMiddleOrientation = Quaternion.Euler(temp);
            proximalMiddleOrientation = Quaternion.Euler(proximalMiddleOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("MP:"+ proximalMiddleOrientation.eulerAngles);
    }
    void MiddleIntermediateCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.middle_inter;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            interMiddleOrientation.x = (float)poseMessage.pose.orientation.y;
            interMiddleOrientation.y = -(float)poseMessage.pose.orientation.z;
            interMiddleOrientation.z = -(float)poseMessage.pose.orientation.x;
            interMiddleOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = interMiddleOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            interMiddleOrientation = Quaternion.Euler(temp);
            interMiddleOrientation = Quaternion.Euler(interMiddleOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("MI" + interMiddleOrientation.eulerAngles);
    }

    //===================================== RING FINGER =========================================
    void RingProximalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.ring_proximal;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            proximalRingOrientation.x = (float)poseMessage.pose.orientation.y;
            proximalRingOrientation.y = -(float)poseMessage.pose.orientation.z;
            proximalRingOrientation.z = -(float)poseMessage.pose.orientation.x;
            proximalRingOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = proximalRingOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            proximalRingOrientation = Quaternion.Euler(temp);
            proximalRingOrientation = Quaternion.Euler(proximalRingOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("RP" + proximalRingOrientation.eulerAngles);
    }

    void RingIntermediateCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.ring_inter;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            interRingOrientation.x = (float)poseMessage.pose.orientation.y;
            interRingOrientation.y = -(float)poseMessage.pose.orientation.z;
            interRingOrientation.z = -(float)poseMessage.pose.orientation.x;
            interRingOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = interRingOrientation.eulerAngles;
            temp.x = (temp.x + 180);
            interRingOrientation = Quaternion.Euler(temp);
            interRingOrientation = Quaternion.Euler(interRingOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("RI" + interRingOrientation.eulerAngles);
    }

    //===================================== PINKIE ==============================================
    void PinkieProximalCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.pinkie_proximal;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            proximalPinkieOrientation.x = (float)poseMessage.pose.orientation.y;
            proximalPinkieOrientation.y = -(float)poseMessage.pose.orientation.z;
            proximalPinkieOrientation.z = -(float)poseMessage.pose.orientation.x;
            proximalPinkieOrientation.w = (float)poseMessage.pose.orientation.w;
            Vector3 temp = proximalPinkieOrientation.eulerAngles;
            temp.x =(temp.x + 180);

            //Debug.Log("PP" + temp.x + " " +temp.y + " " + temp.z);
            proximalPinkieOrientation = Quaternion.Euler(temp);
            proximalPinkieOrientation = Quaternion.Euler(proximalPinkieOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("PP" + proximalPinkieOrientation.eulerAngles);
    }

    void PinkieIntermediateCallback(PoseStamped poseMessage)
    {
        //Debug.Log(isRemovingYawOffset);
        //Debug.Log(" is calculating offset " + isRemovingYawOffset[(int)phalange.pinkie_inter]);
        int i = (int)phalange.pinkie_inter;
        if (isRemovingYawOffset[i])
            ComputeOffset(i, poseMessage);
        else
        {
            interPinkieOrientation.x = (float)poseMessage.pose.orientation.y; //  (float)poseMessage.pose.orientation.y;
            interPinkieOrientation.y = -(float)poseMessage.pose.orientation.z; //-(float)poseMessage.pose.orientation.z;
            interPinkieOrientation.z = -(float)poseMessage.pose.orientation.x; // -(float)poseMessage.pose.orientation.x;
            interPinkieOrientation.w = (float)poseMessage.pose.orientation.w;

            Vector3 temp = interPinkieOrientation.eulerAngles;
            // Debug.Log("PROVA: x:" + baseOrientation_euler.x + " y: " + baseOrientation_euler.y + " z: " + baseOrientation_euler.z);
            temp.x = (temp.x + 180);
            interPinkieOrientation = Quaternion.Euler(temp);
            interPinkieOrientation = Quaternion.Euler(interPinkieOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("PI"+ interPinkieOrientation.eulerAngles);
    }

    //===================================== BASE ORIENTATION =====================================
    void baseCallback(PoseStamped poseMessage)
    {
        int i = (int)phalange.base_;
        if (isRemovingYawOffset[i])
        {
            ComputeOffset(i, poseMessage);
            //this.angleOffsets[i].y = this.angleOffsets[i].y + (float)Math.PI;
        }
        else
        {
            baseOrientation.x = -(float)poseMessage.pose.orientation.y; // apposto 16/07  -(float)poseMessage.pose.orientation.y;
            baseOrientation.y = -(float)poseMessage.pose.orientation.z; // apposto 16/07  -(float)poseMessage.pose.orientation.z;
            baseOrientation.z = (float)poseMessage.pose.orientation.x;  // apposto 16/07  (float)poseMessage.pose.orientation.x;
            baseOrientation.w = (float)poseMessage.pose.orientation.w;  // apposto 16/07

            // Add offset
            Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
            //Debug.Log("PROVA: x:" + baseOrientation_euler.x + " y: " + baseOrientation_euler.y + " z: " + baseOrientation_euler.z);
            baseOrientation_euler.z -= 90;
            baseOrientation = Quaternion.Euler(baseOrientation_euler);
            baseOrientation = Quaternion.Euler(baseOrientation.eulerAngles - this.angleOffsets[i]);
            //interMiddleOrientation = Quaternion.Euler(baseOrientation.eulerAngles - this.angleOffsets[i]);
        }
        //Debug.Log("BASE orientation"+ baseOrientation.eulerAngles);
    }

    //======================================================================================================================================================
    //======================================================================================================================================================
    private void sendToRos(int current_joint, string current_topic_name, Vector3 position)
    {
        //Build Message
        RosMessageTypes.Geometry.PoseStamped message = new PoseStamped();
        message.header.frame_id = "base_link";
        //message.header.stamp = 0;
        message.pose.position.x = position.x;
        message.pose.position.y = position.y;
        message.pose.position.z = position.z;

        message.pose.orientation.x = jointArticulationBodies[current_joint].transform.rotation.z;
        message.pose.orientation.y = -jointArticulationBodies[current_joint].transform.rotation.x;
        message.pose.orientation.z = -jointArticulationBodies[current_joint].transform.rotation.y;
        message.pose.orientation.w = jointArticulationBodies[current_joint].transform.rotation.w;

        ros.Send(current_topic_name + "_u", message);
    }

    //====================================================================================================================================================
    //================================================== Unity Start & Update ============================================================================
    //====================================================================================================================================================

    private void Start()
    {
        ros = ROSConnection.instance;
        jointArticulationBodies = new ArticulationBody[numRobotJoints];
        initJointArticArray();
        // Init callbacks
        //ros.Subscribe<PoseStamped>("/base_Pose", baseCallback);
        //initProximalCallbacks();
        //initIntermediateCallbacks();
        //initThumbCallbacks();
        initJointStateCallback();
    }

    private void initJointStateCallback()
    {
        ros.Subscribe<JointState>("joint_states", JointStateCallback);
    }

    private void JointStateCallback(JointState joint_states)
    {
        // aggiustare il valore del giunto 
        Debug.Log("joint states " + joint_states.name[0]+ "valore" + joint_states.position[0]);
    }

    private void Update()
    {
        //    // Update current time
        //    if (this.isSendingMessages == true)
        //    {
        //        // Update only if we need to send messages back to ros
        //        timeElapsed += Time.deltaTime;
        //    }

        //    // Update BaseOrientation 
        //    root.transform.rotation = baseOrientation;

        //    baseOrientation = proximalMiddleOrientation;
        //    baseOrientation = Quaternion.Euler(baseOrientation.eulerAngles.x + 180, baseOrientation.eulerAngles.y, -baseOrientation.eulerAngles.z);

        //    // Update hand base...
        //    jointArticulationBodies[(int)joint.base_].transform.rotation = baseOrientation;

        //    // Update thumb
        //    jointArticulationBodies[(int)joint.thumb_meta].transform.rotation = proximalThumbOrientation;
        //    jointArticulationBodies[(int)joint.thumb_proximal].transform.rotation = distalThumbOrientation;

        //    // Update proximal joints
        //    jointArticulationBodies[(int)joint.index_proximal].transform.rotation = proximalIndexOrientation;
        //    jointArticulationBodies[(int)joint.middle_proximal].transform.rotation = proximalMiddleOrientation;
        //    jointArticulationBodies[(int)joint.ring_proximal].transform.rotation = proximalRingOrientation;
        //    jointArticulationBodies[(int)joint.pinky_proximal].transform.rotation = proximalPinkieOrientation;

        //    // Update Intermediate joints
        //    jointArticulationBodies[(int)joint.index_inter].transform.rotation = interIndexOrientation;
        //    jointArticulationBodies[(int)joint.middle_inter].transform.rotation = interMiddleOrientation;
        //    jointArticulationBodies[(int)joint.ring_inter].transform.rotation = interRingOrientation;
        //    jointArticulationBodies[(int)joint.pinky_inter].transform.rotation = interPinkieOrientation;

        //    jointArticulationBodies[(int)joint.base_].transform.rotation = interPinkieOrientation;

        //    //SEND data to ROS

        //    if (timeElapsed > publishMessageFrequency && this.isSendingMessages == true)
        //    {
        //        //Index
        //        indexProximalPosition = jointArticulationBodies[(int)joint.index_proximal].transform.position;
        //        indexIntermedPosition = jointArticulationBodies[(int)joint.index_inter].transform.position;
        //        Debug.Log("Position proximal index: " + indexProximalPosition);
        //        sendToRos((int)joint.index_proximal, RosTopicsNames.indexProximal, indexProximalPosition);
        //        sendToRos((int)joint.index_inter, RosTopicsNames.indexIntermediate, indexIntermedPosition);
        //        
                  // Middle
        //        sendToRos((int)joint.middle_proximal, RosTopicsNames.middleProximal);
        //        sendToRos((int)joint.middle_inter, RosTopicsNames.middleIntermediate);
        //        
                  // Ring
        //        sendToRos((int)joint.ring_proximal, RosTopicsNames.ringProximal);
        //        sendToRos((int)joint.ring_inter, RosTopicsNames.ringIntermediate);
        //        
                  // Pinkie
        //        sendToRos((int)joint.pinky_proximal, RosTopicsNames.pinkieProximal);
        //        sendToRos((int)joint.pinky_inter, RosTopicsNames.pinkieIntermediate);
        //        this.timeElapsed = 0;
        //    }
    }
}