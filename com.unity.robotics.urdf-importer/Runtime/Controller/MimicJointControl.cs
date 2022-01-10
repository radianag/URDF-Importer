using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter;

public class MimicJointControl : MonoBehaviour
{
    private UrdfJoint mimicedJoint;
    private float multiplier;
    private float offset = 0.0f; // Rad or m
    private float mimicedJointStartJointPosition = 0.0f; // Rad or m
    private ArticulationBody joint;
    private UrdfJoint urdfJoint;

    public void SetMimic(
            UrdfJoint mimicedJoint,
            float multiplier,
            float offset = 0.0f,
            float mimicedJointStartJointPosition = 0.0f)
    {
        this.mimicedJoint = mimicedJoint;
        this.multiplier = multiplier;
        if(!Double.IsNaN(offset))
        {
            this.offset = offset;
        }
        this.mimicedJointStartJointPosition = mimicedJointStartJointPosition;
    }

    private void updateJointDriveTarget(float target)
    {
        ArticulationDrive currentDrive = joint.xDrive;
        currentDrive.target = target;
        joint.xDrive = currentDrive;
    }

    private int xAxis = 0;
    public void TeleportToJointPosition(float position)
    {
        var jp = joint.jointPosition;
        jp[xAxis] = position;
        joint.jointPosition = jp;

        // This is Done in FixedUpdate as well because user should have set
        // the mimicedJoint reset also,
        // but is ok to make sure here.
        float localTarget = position;
        if (urdfJoint.IsRevoluteOrContinuous)
        {
            localTarget = localTarget * Mathf.Rad2Deg;
        }
        updateJointDriveTarget(localTarget);
    }

    public void TeleportSetToStartJointPosition()
    {
        var startTarget = multiplier * mimicedJointStartJointPosition + offset; // Rad or m
        TeleportToJointPosition(startTarget);
    }

    public void TeleportSetToMatchJointPosition()
    {
        float localTarget = mimicedJoint.unityJoint.jointPosition[xAxis];
        localTarget = multiplier * localTarget + offset; // Rad or m
        TeleportToJointPosition(localTarget);
    }

    void Start()
    {
        joint = this.GetComponent<ArticulationBody>();
        urdfJoint = this.GetComponent<UrdfJoint>();
        TeleportSetToStartJointPosition();
    }

    void FixedUpdate()
    {
        float localOffset = offset;
        if (urdfJoint.IsRevoluteOrContinuous)
        {
            localOffset = localOffset * Mathf.Rad2Deg;
        }
        updateJointDriveTarget(multiplier * mimicedJoint.unityJoint.xDrive.target + localOffset);
    }
}
