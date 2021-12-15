using UnityEngine;
using JointState = RosMessageTypes.Sensor.JointState;
using PoseStamped = RosMessageTypes.Geometry.PoseStamped;

public class HandController_finale : MonoBehaviour
{
    private ROSConnection ros;
    public GameObject hand;
    private ArticulationBody[] jointArticulationBodies;
    private int nJoints = 13;
    private Quaternion baseOrientation = new Quaternion();
    private Vector3 initialPose;
    private bool firstPose = true;

    void Start()
    {
        ros = ROSConnection.instance;
        GetHandReference();
        initJointStateCallback();
        initbaseCallback();
    }

    /*void Update()
    {
        //-------------------------- 3 giunti sferici ---------------------------------------------------
        //jointArticulationBodies[10].xDrive = circuit_support_1_x;
        //jointArticulationBodies[11].yDrive = circuit_support_2_y;
        //jointArticulationBodies[12].zDrive = circuit_support_3_z;
        //// Controllando i giunti nell'Update la rotazione della mano non funziona bene
        //// Mettendo 3 giunti revolute bisogna modificare i parametri di Anchor Rotation altrimenti gli assi sono tutti coincidenti 

        //// Using rotate instead of setting transform.rotation , gives a little °non smooth° movement 
        //// When modifying the object's transform using transform.rotation or transform.Rotate, causes problems in the collision 
        //// the collisions do not work as expected
        
        //Vector3 pos_attuale, pos_finale;
        //Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
        //hand.transform.rotation = Quaternion.Euler(baseOrientation_euler);
        
        //pos_finale = baseOrientation_euler;
        //pos_attuale.x = hand.transform.rotation.eulerAngles.x;
        //pos_attuale.y = hand.transform.rotation.eulerAngles.y;
        //pos_attuale.z = hand.transform.rotation.eulerAngles.z;
        //delta = pos_finale - pos_attuale;
        //hand.transform.Rotate(delta);
    }*/

    void GetHandReference()
    {
        jointArticulationBodies = new ArticulationBody[nJoints];
        string pinkie_proximal = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/pinkie/pinkie_proximal";
        jointArticulationBodies[0] = hand.transform.Find(pinkie_proximal).GetComponent<ArticulationBody>();

        string pinkie_middle = pinkie_proximal + "/pinkie_middle";
        jointArticulationBodies[1] = hand.transform.Find(pinkie_middle).GetComponent<ArticulationBody>();

        string ring_proximal = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/ring/ring_proximal";
        jointArticulationBodies[2] = hand.transform.Find(ring_proximal).GetComponent<ArticulationBody>();

        string ring_middle = ring_proximal + "/ring_middle";
        jointArticulationBodies[3] = hand.transform.Find(ring_middle).GetComponent<ArticulationBody>();

        string middle_proximal = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/middle/middle_proximal";
        jointArticulationBodies[4] = hand.transform.Find(middle_proximal).GetComponent<ArticulationBody>();

        string middle_middle = middle_proximal + "/middle_middle";
        jointArticulationBodies[5] = hand.transform.Find(middle_middle).GetComponent<ArticulationBody>();

        string index_proximal = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/palm2/index/index_proximal";
        jointArticulationBodies[6] = hand.transform.Find(index_proximal).GetComponent<ArticulationBody>();

        string index_middle = index_proximal + "/index_middle";
        jointArticulationBodies[7] = hand.transform.Find(index_middle).GetComponent<ArticulationBody>();

        string thumb = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3/palm1/thumb";
        jointArticulationBodies[8] = hand.transform.Find(thumb).GetComponent<ArticulationBody>();

        string thumb_proximal = thumb + "/thumb_proximal";
        jointArticulationBodies[9] = hand.transform.Find(thumb_proximal).GetComponent<ArticulationBody>();

        string circuit_support_1 = "ar10/wrist_plate/circuit_support_1";
        jointArticulationBodies[10] = hand.transform.Find(circuit_support_1).GetComponent<ArticulationBody>();

        string circuit_support_2 = "ar10/wrist_plate/circuit_support_1/circuit_support_2";
        jointArticulationBodies[11] = hand.transform.Find(circuit_support_2).GetComponent<ArticulationBody>();

        string circuit_support_3 = "ar10/wrist_plate/circuit_support_1/circuit_support_2/circuit_support_3";
        jointArticulationBodies[12] = hand.transform.Find(circuit_support_3).GetComponent<ArticulationBody>();
    }

    void init()
    {
        //Inizializzazione a 0 per l'angolo delle dita
        for (int i = 0; i < 13; i++)
        {
            var index1 = jointArticulationBodies[i].xDrive;
            index1.target = 0;
            jointArticulationBodies[i].xDrive = index1;
        }
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
            var pinkie_p = jointArticulationBodies[0].xDrive;
            pinkie_p.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[0].xDrive = pinkie_p;
        }
        else if (joint_states.name[0] == "servo7")
        {
            var pinkie_m = jointArticulationBodies[1].xDrive;
            pinkie_m.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[1].xDrive = pinkie_m;
        }
        else if (joint_states.name[0] == "servo4")
        {
            var ring_p = jointArticulationBodies[2].xDrive;
            ring_p.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[2].xDrive = ring_p;
        }
        else if (joint_states.name[0] == "servo5")
        {
            var ring_m = jointArticulationBodies[3].xDrive;
            ring_m.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[3].xDrive = ring_m;
        }
        else if (joint_states.name[0] == "servo2")
        {
            var middle_p = jointArticulationBodies[4].xDrive;
            middle_p.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[4].xDrive = middle_p;
        }
        else if (joint_states.name[0] == "servo3")
        {
            var middle_m = jointArticulationBodies[5].xDrive;
            middle_m.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[5].xDrive = middle_m;
        }
        else if (joint_states.name[0] == "servo0")
        {
            var index_p = jointArticulationBodies[6].xDrive;
            index_p.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[6].xDrive = index_p;
        }
        else if (joint_states.name[0] == "servo1")
        {
            var index_m = jointArticulationBodies[7].xDrive;
            index_m.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[7].xDrive = index_m;
        }
        else if (joint_states.name[0] == "servo8")
        {
            var thumb_p = jointArticulationBodies[8].xDrive;
            thumb_p.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[8].xDrive = thumb_p;
        }
        else if (joint_states.name[0] == "servo9")
        {
            var thumb_m = jointArticulationBodies[9].xDrive;
            thumb_m.target = (float)(Mathf.Rad2Deg * joint_states.position[0]);
            jointArticulationBodies[9].xDrive = thumb_m;
        }
    }

    private void initbaseCallback()
    {
        ros.Subscribe<PoseStamped>("backPose", base_revolute_Callback);
        //ros.Subscribe<PoseStamped>("backPose", base_spherical_Callback);
    }

    private void base_spherical_Callback(PoseStamped poseMessage)
    {
        baseOrientation.x = (float)poseMessage.pose.orientation.y;
        baseOrientation.y = -(float)poseMessage.pose.orientation.z;
        baseOrientation.z = -(float)poseMessage.pose.orientation.x;
        baseOrientation.w = (float)poseMessage.pose.orientation.w;

        //-------------------------------------- 3 giunti sferici ---------------------------------------------
        Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
        Vector3 pos_finale = baseOrientation_euler;

        if (firstPose)
        {
            initialPose = baseOrientation.eulerAngles;
            firstPose = false;
        }       

        var circuit_support_1_x = jointArticulationBodies[10].xDrive;
        var circuit_support_2_y = jointArticulationBodies[11].yDrive;
        var circuit_support_3_z = jointArticulationBodies[12].zDrive;

        var posY = (90 + pos_finale.x - initialPose.x) % 360;
        var posX = (190 + (pos_finale.y - initialPose.y)) % 360;
        var posZ = (90 + (pos_finale.z - initialPose.z)) % 360;

        //Ogni asse singolarmente funziona (funzionano bene X e Z, Y un po' meno) ma non funzionano tutti e 3 assieme 
        //--------------------------------------yaw--------------------------------------------- OK
        if (posX > 240)
        {
            circuit_support_1_x.target = 240;
            Debug.Log("Posizione > 240   " + posX + " " + pos_finale.y + " " + initialPose.y);
        }
        else if (posX < 140)
        {
            circuit_support_1_x.target = 140;
            Debug.Log("Posizione < 140   " + posX + " " + pos_finale.y + " " + initialPose.y);
        }
        else
        {
            circuit_support_1_x.target = posX;
            Debug.Log(posX + " " + pos_finale.y + " " + initialPose.y);
        }
        //--------------------------------------------------------------------------------------- OK

        //// da sistemare. Gli angoli non arrivano corretti dall'IMU. 
        if (posY > 160)
        {
            circuit_support_2_y.target = 160;
            Debug.Log("Posizione > 160   " + posY + " " + pos_finale.x + " " + initialPose.x);
        }
        else if (posY < 20)
        {
            circuit_support_2_y.target = 20;
            Debug.Log("Posizione < 20   " + posY + " " + pos_finale.x + " " + initialPose.x);
        }
        else
        {
            circuit_support_2_y.target = posY;
            Debug.Log(posY + " " + pos_finale.x + " " + initialPose.x);
        }

        //--------------------------------------------------------------------------------------- OK
        if (posZ > 160)
        {
            circuit_support_3_z.target = 160;
            Debug.Log("Posizione > 160   " + posZ + " " + pos_finale.z + " " + initialPose.z);
        }
        else if (posZ < 20)
        {
            circuit_support_3_z.target = 20;
            Debug.Log("Posizione < 20   " + posZ + " " + pos_finale.z + " " + initialPose.z);
        }
        else
        {
            circuit_support_3_z.target = posZ;
            Debug.Log(posZ + " " + pos_finale.z + " " + initialPose.z);
        }
        //--------------------------------------------------------------------------------------- OK

        jointArticulationBodies[10].xDrive = circuit_support_1_x;
        jointArticulationBodies[11].yDrive = circuit_support_2_y;
        jointArticulationBodies[12].zDrive = circuit_support_3_z;
    }

    private void base_revolute_Callback(PoseStamped poseMessage)
    {
        baseOrientation.x = (float)poseMessage.pose.orientation.y;
        baseOrientation.y = -(float)poseMessage.pose.orientation.z;
        baseOrientation.z = -(float)poseMessage.pose.orientation.x;
        baseOrientation.w = (float)poseMessage.pose.orientation.w;
        
        Vector3 baseOrientation_euler = baseOrientation.eulerAngles;
        Vector3 pos_finale = baseOrientation_euler;

        if (firstPose)
        {
            initialPose = baseOrientation.eulerAngles;
            firstPose = false;
        }

        var circuit_support_1 = jointArticulationBodies[10].xDrive;
        var circuit_support_2 = jointArticulationBodies[11].xDrive;
        var circuit_support_3 = jointArticulationBodies[12].xDrive;

        var posY = (90 + pos_finale.x - initialPose.x) % 360;
        var posX = (pos_finale.y - initialPose.y) % 360;
        var posZ = -(90 + (pos_finale.z - initialPose.z)) % 360;

        //Ogni asse singolarmente funziona ma non funzionano tutti e 3 assieme 
        circuit_support_1.target = posX;
        circuit_support_2.target = posY;        
        circuit_support_3.target = posZ;

        jointArticulationBodies[10].xDrive = circuit_support_1;
        jointArticulationBodies[11].xDrive = circuit_support_2;
        jointArticulationBodies[12].xDrive = circuit_support_3;
    }
}