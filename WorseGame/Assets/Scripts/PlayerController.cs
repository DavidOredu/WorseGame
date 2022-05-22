using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform player;
    public Rigidbody2D rb;
    public SpringJoint2D rope;
    public PlayerData playerData;

  
    public LineRenderer line;
    public LayerMask whatToHook;
    public float grabSpeed;
    public float minimumDistance;

    [Range(0, 100)]
    public int percentageToDrop;

    public float heightOffset;
    private bool tooHigh;

    RaycastHit2D hookHit;
    Transform currentBox;
    private bool isHolding;

    public float orthoMinSize;
    public float orthoMaxSize;
    private List<GameObject> hitsDebug = new List<GameObject>();

    private Vector2 desiredPos;
    public Vector2 offsetPoint;
    private Vector2 endPoint;
    public float maxOffsetMultiplier;
    private float offsetMultiplier;
    public float maxVel;
    public Vector2 ropeVel;
    public float offsetVel;

    public MMFeedbacks extensionSound;
    public MMFeedbacks hitSound;
    public MMFeedbacks hitFeedback;
    // Start is called before the first frame update
    void Start()
    {
        rope.enabled = false;
        hookHit.point = transform.position;
        playerData = Resources.Load<PlayerData>("PlayerData");

        grabSpeed = playerData.ropeGrapSpeed;
        rope.dampingRatio = playerData.ropeDampingRatio;

        offsetVel = maxVel;
        offsetMultiplier = maxOffsetMultiplier;

        SetPlayerBig();
    }
    void SetPlayerBig()
    {
        var playerSc = player.GetComponent<Player>();
        if (playerSc.isBig)
        {
            transform.localScale = transform.localScale * 2;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        // set player animation
        SetPlayerBody();
        CheckPlayerMaxHeight();

        if (isHolding)
        {
            DrawLine();
            rope.enabled = true;
        }
        else
        {
            rope.enabled = false;
        }
    }
    private void CheckPlayerMaxHeight()
    {
        if ((transform.position.y > LevelGenerator.instance.playerMaxHeight + heightOffset) && !tooHigh) 
        {
            rb.velocity = new Vector2((percentageToDrop * rb.velocity.x) / 100, (percentageToDrop * rb.velocity.y) / 100);
            tooHigh = true;
        }
        else if((transform.position.y < LevelGenerator.instance.playerMaxHeight + 20f) && tooHigh)
        {
            tooHigh = false;
        }

    }
    private void HoldingStart()
    {
        isHolding = true;
        Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);

        Collider2D[] colliders;

        colliders = Physics2D.OverlapCircleAll(clickedPos, 60f, LayerMask.GetMask("Box"));
        if(colliders.Length == 0)
        {
            hookHit = Physics2D.Raycast(transform.position, Vector2.zero, 0f, LayerMask.GetMask("Box"));
            isHolding = false;
            return;
        }
        currentBox = GetNearestBox(colliders, clickedPos);
        hookHit = Physics2D.Raycast(transform.position, (currentBox.position - transform.position).normalized, Mathf.Infinity, LayerMask.GetMask("Box"));
        
        if(hookHit.collider == null)
        {
            isHolding = false;
            return;
        }
        rope.connectedAnchor = hookHit.point;
        rope.enabled = true;
        line.enabled = true;

        if (AudioManager.instance.gameSettings.sound)
        {
            extensionSound.PlayFeedbacks();
        }
        hitFeedback.PlayFeedbacks();

        ropeVel = rb.velocity;
        offsetVel = maxVel;
        offsetMultiplier = maxOffsetMultiplier;
    }
    public void PlayHitSound()
    {
        if (AudioManager.instance.gameSettings.sound)
        {
            hitSound.PlayFeedbacks();
        }
    }
    private void Holding()
    {
        if (!isHolding) { return; }

       

        if(playerData.hasLaserRope)
            LaserRope();

        var dist = (hookHit.transform.position - rope.transform.position).normalized;
        float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
     //   hookHit.transform.Rotate(new Vector3(hookHit.transform.rotation.x, hookHit.transform.rotation.y, angle));
        hookHit.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + 90f));
        rope.distance = Mathf.Lerp(rope.distance, minimumDistance, Time.deltaTime * grabSpeed);
        rb.velocity *= .995f;
    }
    private void HoldingEnd()
    {
        isHolding = false;
        rope.enabled = false;
        line.enabled = false;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        hitsDebug.Clear();

        
    }
    void CheckStopGrappling()
    {
        if (hookHit.collider != null)
        {
            if (!hookHit.collider.gameObject.activeSelf)
            {
                HoldingEnd();
            }
        } 
    }
    void LaserRope()
    {
        var startPoint = transform.position;
        var endPoint = hookHit.point;

        var lineDist = endPoint - (Vector2)startPoint;

        var hits = Physics2D.RaycastAll(startPoint, lineDist.normalized, lineDist.magnitude, LayerMask.GetMask("Box"));
        Debug.DrawRay(startPoint, lineDist, Color.green);


        for (int i = 0; i < hits.Length; i++)
        {
            if (!hitsDebug.Contains(hits[i].collider.gameObject))
            hitsDebug.Add(hits[i].collider.gameObject);
        }
       
        foreach (var hit in hits)
        {
            if(hit.collider == hookHit.collider) { continue; }
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Box"))
            {
                var box = hit.collider.GetComponent<Box>();
                box.Damage(gameObject, false);
            }
        }
    }
    private void LateUpdate()
    {
        
    }
    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HoldingStart();
        }
        if (Input.GetMouseButton(0))
        {
            Holding();
        }
        if (Input.GetMouseButtonUp(0))
        {
            HoldingEnd();
        }
        CheckStopGrappling();
        
    }
    void SetPlayerBody()
    {
        player.transform.localScale = new Vector3(Mathf.Clamp(Remap(rb.velocity.magnitude, 0, 100f, 1, 1.5f), 1, 1.5f),
            Mathf.Clamp(Remap(rb.velocity.magnitude, 0, 100f, 1, 0.5f), 0.5f, 1), 1);
        GameManager.instance.cmVcam.m_Lens.OrthographicSize = Mathf.Clamp(Remap(rb.velocity.magnitude, 0, 100f, orthoMinSize, orthoMaxSize), orthoMinSize, orthoMaxSize);
        Vector3 dir = rb.velocity.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2; 
    }
    void DrawLine()
    {
        //  if(hookHit.collider == null || rope == null)
        //  RemoveLine 
        desiredPos = hookHit.point + offsetPoint;
        endPoint = Vector2.SmoothDamp(endPoint, desiredPos, ref ropeVel, .03f);
        offsetMultiplier = Mathf.SmoothDamp(offsetMultiplier, 0, ref offsetVel, .12f);

        int positions = 100;
        line.positionCount = positions;
        Vector2 startPoint = transform.position;

        line.SetPosition(0, startPoint);
        line.SetPosition(positions - 1, endPoint);

        float freq = 15f, height = .5f;
        for (int i = 0; i < positions - 1; i++)
        {
            float percentage = i / (float)positions;
            float progress = percentage * offsetMultiplier;
            float offset = ((Mathf.Sin(progress * freq) - .5f) * height) * (progress * 2);

            Vector2 dir = (endPoint - startPoint).normalized;
            float s = Mathf.Sin(percentage * 100 * Mathf.Deg2Rad);

            float offset2 = Mathf.Cos((((offsetMultiplier) * 90)) * Mathf.Deg2Rad);
            Vector2 pos = (startPoint + (endPoint - startPoint) / positions * i) + (offset2 * offset * Vector2.Perpendicular(dir) + (offsetMultiplier) * s * Vector2.down);
            line.SetPosition(i, pos);
        }
    }
   Transform GetNearestBox(Collider2D[] colliders, Vector3 clickedPos)
    {
        int index = 0;

        float dist = Mathf.Infinity;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < colliders.Length; i++)
        {
            dist = Vector3.Distance(colliders[i].transform.position, clickedPos);
            if(dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return colliders[index].transform;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hookHit.point, .5f);
    }
}
