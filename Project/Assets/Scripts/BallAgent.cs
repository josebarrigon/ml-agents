using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    [SerializeField] private Transform target1; //goal1
    [SerializeField] private Transform target2; //goal2
    [SerializeField] private Material color1; 
    [SerializeField] private Material color2;
    private Transform pickedTarget;



    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        float moveX = actions.ContinuousActions[1];
        float speed = 2f;

        //CALCULATE REWARD WITH DISTANCE TO TARGET
        float distanceToTarget = Vector3.Distance(transform.localPosition, pickedTarget.localPosition);
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
        sensor.AddObservation(pickedTarget.localPosition); //goal
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0.4f, Random.Range(-8f, 6f)); //ball
        target1.localPosition = new Vector3(0.25f, 0.5f, 7f); //goal
        target2.localPosition = new Vector3(0.25f, 0.5f, -7.4f); //goal

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            pickedTarget = target2;
            transform.GetComponent<Renderer>().material = color1;

        }
        else
        {
            pickedTarget = target1;
            transform.GetComponent<Renderer>().material = color2;
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "goal")
        {
            if (collider.gameObject == pickedTarget.gameObject)
            {
                AddReward(10f);
                EndEpisode();
            }
            else
            {
                AddReward(-30f);
                EndEpisode();
            }
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
