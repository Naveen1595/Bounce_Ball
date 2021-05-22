using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private LineRenderer[] lineRenderers;
    [SerializeField] private Transform[] stripPosition;
    [SerializeField] private Transform center;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private GameObject ball;
    [SerializeField] private float ballPositionOffSet;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private float maxLength;
    [SerializeField] private float bottomBoundary;
    [SerializeField] private LineRenderer Trajectory;
    [SerializeField] private float force;
    bool isMouseDown;
    Rigidbody2D ballrb;
    Collider2D ballCollider;
    Vector3 ballforce;
    Vector3 slingMid;

    private void OnMouseDown()
    {
        SetTrajectoryActive(false);
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }

    private void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPosition[0].position); 
        lineRenderers[1].SetPosition(0, stripPosition[1].position);

        slingMid = new Vector3((stripPosition[0].position.x + stripPosition[1].position.x)/2, stripPosition[0].position.y + (stripPosition[1].position.y) / 2);
        CreateBall();
    }

   
    private void Update()
    {
        if(isMouseDown)
        {

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            
            setStrips(currentPosition);

            if(ballCollider)
            {
                ballCollider.enabled = true;
            }

            float pullDistance = Vector3.Distance(slingMid, ball.transform.position);
            showTrajectory(pullDistance);
        }
        else
        {
            ResetStrips();
        }
    }

    //Trajectory
    void SetTrajectoryActive(bool active)
    {
        Trajectory.enabled = active;
    }

    void showTrajectory(float distance)
    {
        SetTrajectoryActive(true);
        Vector3 diff = slingMid - ballrb.transform.position;
        int segmentCount = 25;
        Vector2[] segments = new Vector2[segmentCount];
        segments[0] = ballrb.transform.position;

        Vector2 segVelocity = new Vector2(diff.x, diff.y) * ballforce * distance;

        for (int i = 1; i < segmentCount; i++)
        {
            float timeCurve = (i * Time.fixedDeltaTime * 5);
            segments[i] = segments[0] + segVelocity * timeCurve + 0.5f * Physics2D.gravity * Mathf.Pow(timeCurve, 2);


        }
        Trajectory.positionCount = segmentCount;
        for (int j = 0; j < segmentCount; j++)
        {
            Trajectory.SetPosition(j, segments[j]);
        }
    }

    //Create Ball
    void CreateBall()
    {
        ballrb = Instantiate(ball).GetComponent<Rigidbody2D>();
        ballCollider = ballrb.GetComponent<Collider2D>();
        ballCollider.enabled = false;

        ballrb.isKinematic = true;

        ResetStrips();
    }
   
    
    void Shoot()
    {
        ballrb.isKinematic = false;
        ballforce = (currentPosition - center.position) * force * -1;
        ballrb.velocity = ballforce;

        ballrb = null;
        ballCollider = null;
        Invoke("CreateBall", 2);
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        setStrips(currentPosition);
    }

    void setStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if(ballrb)
        {
            Vector3 dir = position - center.position;
            ballrb.transform.position = position + dir.normalized * ballPositionOffSet;
            ballrb.transform.right = -dir.normalized;
        }
        
    }
}
