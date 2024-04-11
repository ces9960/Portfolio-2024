using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class VehicleAgent : Agent
{
    Vector3 velocity;
    const float accelerationMagnitude = 0.002f;
    int targetsReached = 0;
    const int targetCount = 5;
    public TargetSpawn targetObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnEpisodeBegin()
    {
        targetObj.moveTarget();
        targetsReached = 0;
        transform.position = new Vector3(0, 0.5f, 0);
        velocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetObj.transform.position);
        sensor.AddObservation(transform.position);

        sensor.AddObservation(velocity.x);
        sensor.AddObservation(velocity.z);
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        velocity += accelerationMagnitude * controlSignal;
        transform.position += velocity;

        float targetDistance = Vector3.Distance(targetObj.transform.position, transform.position);

        if (targetDistance < 2.0f)
        {
            targetsReached++;
            targetObj.moveTarget();
            SetReward(1.0f);
        }
        if(targetsReached >= targetCount)
        {
            SetReward(10.0f);
            EndEpisode();
        }
        if(transform.position.x > 25 || transform.position.z > 25 || transform.position.x < -25 || transform.position.z < -25)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
