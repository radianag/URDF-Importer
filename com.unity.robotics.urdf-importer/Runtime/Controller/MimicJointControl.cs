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

    public void set(UrdfJoint mimicedJoint, double multiplier, double offset = 0.0)
    {
        this.mimicedJoint = mimicedJoint;
        this.multiplier = (float)multiplier;

        if(!Double.IsNaN(offset))
        {
            this.offset = (float)offset;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        joint = this.GetComponent<ArticulationBody>();
        urdfJoint = this.GetComponent<UrdfJoint>();
        ArticulationDrive currentDrive = joint.xDrive;
        currentDrive.stiffness = 10000; // P
        currentDrive.damping = 1000; // D
        currentDrive.forceLimit = 200;
        joint.xDrive = currentDrive;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ArticulationDrive currentDrive = joint.xDrive;
        currentDrive.target = multiplier * mimicedJoint.unityJoint.xDrive.target + offset;
        joint.xDrive = currentDrive;
    }
}
