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
        float moveX = actions.ContinuousActions[1];
        float speed = 2f;

        //CALCULATE REWARD WITH DISTANCE TO TARGET
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        AddReward(-0.01f * distanceToTarget);
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); //ball
        sensor.AddObservation(target.localPosition); //goal
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0.4f, Random.Range(-8f, 6f)); //ball
        target.localPosition = new Vector3(0.25f, 0.5f, 7f); //goal
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal"))
        {
            SetReward(10f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("wall"))
        {
            SetReward(-5f);
            EndEpisode();
        }
    }
}
