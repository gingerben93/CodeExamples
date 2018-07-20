using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
    private Vector3 startLocation, endLocation;
    private Rigidbody2D rigidbodyComponent;

    //this would work in 3d space as well
    void Start()
    {
        //damages bassed on player intel stat
        transform.GetComponent<DamageOnCollision>().damage = PlayerStats.PlayerStatsSingle.intelligence;
        //set on collide behavior
        transform.GetComponent<DamageOnCollision>().onCollide = OnCollide;
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        CalculateRotation();
    }

    void OnCollide()
    {
        Destroy(gameObject);
    }

    void CalculateRotation()
    {
        //direction based on speed; faces object forward; rigidbody needs to be getting speeds
        //trig functions not efficient; find different way?
        float angle = Mathf.Atan2(rigidbodyComponent.velocity.y, -rigidbodyComponent.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    //start method; use when firball instantiated
    public void SetStartData(Vector3 Start, Vector3 End)
    {
        startLocation = Start;
        endLocation = End;

        //get fireball rb
        rigidbodyComponent = GetComponent<Rigidbody2D>();
        //set start locations of object
        transform.position = Start;
        //get firball velocity
        CalculateSpeed();
        CalculateRotation();
    }

    //gets speed bases on start and end location
    void CalculateSpeed()
    {
        float g, midpointHeight, time, displacementY;
        Vector3 velocityY, velocityXZ, displacementXZ;

        //current gravity in game
        g = -9.81f;

        //only different if end.y - start.y < 3
        midpointHeight = endLocation.y - startLocation.y;
        displacementY = endLocation.y - startLocation.y;

        //min midpoint hight so fireball doesn't move too fast
        if (0 < midpointHeight && midpointHeight < 3)
        {
            midpointHeight = 3;
        }

        //max height must be > 0; different behavior could make this null?
        if (displacementY <= 0)
        {
            if (displacementY == 0)
            {
                midpointHeight = 3f;
                displacementY = 3f;
            }
            else
            {
                midpointHeight = 1;
            }
        }

        //find speed forward based on where start and end locations are
        displacementXZ = new Vector3(endLocation.x - startLocation.x, 0, endLocation.z - startLocation.z);
        time = Mathf.Sqrt(-2 * midpointHeight / g) + Mathf.Sqrt(2 * (displacementY - midpointHeight) / g);
        velocityY = Vector3.up * Mathf.Sqrt(-2 * g * midpointHeight);
        velocityXZ = displacementXZ / time;

        //set forward velocity in rb
        rigidbodyComponent.velocity = (velocityXZ + velocityY * -Mathf.Sign(g));
    }
}
