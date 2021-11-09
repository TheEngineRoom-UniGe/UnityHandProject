using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum joint
{
    base_0, base_,
    pinky_meta, pinky_proximal, pinky_inter, pinky_distal,
    ring_meta, ring_proximal, ring_inter, ring_distal,
    middle_meta, middle_proximal, middle_inter, middle_distal,
    index_meta, index_proximal, index_inter, index_distal,
    thumb_meta, thumb_proximal, thumb_distal
}

enum phalange
{
    thumb_proximal, thumb_distal,
    index_proximal, index_inter,
    middle_proximal, middle_inter,
    ring_proximal, ring_inter,
    pinkie_proximal, pinkie_inter,
    base_
}

public static class Path
{   //Base path definitions
    public const string base_0 = "fob_sensor/root/base_0";
    public const string base_ = base_0 + "/base";

    //Thumb path definitions
    public const string thumb_meta = base_ + "/thumb_meta_0/thumb_meta";
    public const string thumb_proximal = thumb_meta + "/thumb_proximal";
    public const string thumb_distal = thumb_proximal + "/thumb_distal";

    //Index path definitions
    public const string index_meta = base_ + "/index_meta_0/index_meta";
    public const string index_proximal = index_meta + "/index_proximal";
    public const string index_inter = index_proximal + "/index_middle";
    public const string index_distal = index_inter + "/index_distal";

    //Middle path definitions
    public const string middle_meta = base_ + "/middle_meta_0/middle_meta";
    public const string middle_proximal = middle_meta + "/middle_proximal";
    public const string middle_inter = middle_proximal + "/middle_middle";
    public const string middle_distal = middle_inter + "/middle_distal";

    //Ring path definitions
    public const string ring_meta = base_ + "/ring_meta_0/ring_meta";
    public const string ring_proximal = ring_meta + "/ring_proximal";
    public const string ring_inter = ring_proximal + "/ring_middle";
    public const string ring_distal = ring_inter + "/ring_distal";

    //Pinkie path definitions
    public const string pinky_meta = base_ + "/pinky_meta_0/pinky_meta";
    public const string pinky_proximal = pinky_meta + "/pinky_proximal";
    public const string pinky_inter = pinky_proximal + "/pinky_middle";
    public const string pinky_distal = pinky_inter + "/pinky_distal";
}

public static class RosTopicsNames
{

    //Base joint
    public const string basejoint = "base_Pose";

    //Proximal joints
    public const string thumbProximal = "thumb_2Pose";
    public const string indexProximal = "index_2Pose";
    public const string middleProximal = "middle_2Pose";
    public const string ringProximal = "ring_2Pose";
    public const string pinkieProximal = "pinkie_2Pose";

    //Intermediate joints
    public const string indexIntermediate = "index_1Pose";
    public const string middleIntermediate = "middle_1Pose";
    public const string ringIntermediate = "ring_1Pose";
    public const string pinkieIntermediate = "pinkie_1Pose";

    //Distal joints
    public const string thumbDistal = "thumb_1Pose";
}
