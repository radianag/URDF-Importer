using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter;

public class MimicJointControl : MonoBehaviour
{
    private UrdfJoint mimicedJoint;
    private float multiplier;
    private float offset = 0.0f;
    private ArticulationBody joint;
    private UrdfJoint urdfJoint;


    public void SetMimic(UrdfJoint mimicedJoint, double multiplier, double offset = 0.0)
    {
        this.mimicedJoint = mimicedJoint;
        this.multiplier = (float)multiplier;

        if(!Double.IsNaN(offset))
        {
            this.offset = (float)offset;
        }
    }

    void Start()
    {
        joint = this.GetComponent<ArticulationBody>();
        urdfJoint = this.GetComponent<UrdfJoint>();
    }

    void FixedUpdate()
    {
        ArticulationDrive currentDrive = joint.xDrive;
        currentDrive.target = multiplier * mimicedJoint.unityJoint.xDrive.target + offset;
        joint.xDrive = currentDrive;
    }
}
