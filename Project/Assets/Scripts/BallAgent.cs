using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    [SerializeField] private Transform target; //goal

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        float speed = 2f;

        transform.localPosition += new Vector3(0, 0, moveZ) * Time.deltaTime * speed;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f, 0.4f, Random.Range(-8f, 6f)); //ball
        target.localPosition = new Vector3(0.25f, 0.5f, 7f); //goal
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "goal")
        {
            AddReward(10f);
            EndEpisode();
        }
    }

}
