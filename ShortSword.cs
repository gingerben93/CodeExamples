using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortSword : Weapon
{
    bool attacking = false;
    private Vector3 mousePos;

    SpriteRenderer Image;
    BoxCollider2D ObjectCollider;

    void Start()
    {
        //set damage
        transform.GetComponent<DamageOnCollision>().damage = PlayerStats.PlayerStatsSingle.strength;
        transform.GetComponent<DamageOnCollision>().onCollide = onCollide;

        Image = gameObject.GetComponentInChildren<SpriteRenderer>();
        ObjectCollider = gameObject.GetComponentInChildren<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (attacking)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 8f);
        }
    }

    public override void Attack()
    {
        if (!attacking)
        {
            StartCoroutine(SwordAttack());
        }
    }

    IEnumerator SwordAttack()
    {
        //calculate angle for pointing at mouse
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dir = mousePos - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //offset angle where it starts so it swings aross player
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 60);

        attacking = true;
        Image.enabled = true;
        ObjectCollider.enabled = true;

        //change later to waiting for angle reached not time limit
        yield return new WaitForSeconds(.25f);

        Image.enabled = false;
        ObjectCollider.enabled = false;
        attacking = false;
    }

    //sword does nothing special on collide right now, but later it will; sounds, effects, etc.
    void onCollide()
    {
        //
    }

    void OnDestroy()
    {
        PlayerController.PlayerControllerSingle.playerAttack -= Attack;
    }
}
