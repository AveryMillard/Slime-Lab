using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    [SerializeField, ReadOnly]
    float bossFightLength = 90f;

    [SerializeField]
    float spellCooldown = 2f;

    [SerializeField]
    float spellCastTime = 1f;

    public float frontalRadius = 15f;
    public float coneRadius = 20f;
    public float beamWidth = 2f;
    public float pushForce = 20f;

    // clamp view to 360 degrees max
    [Range(0, 360)]
    public float viewAngle = 70f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution = 1;

    public int edgeResolveIterations = 4;

    public float edgeDistanceThreshold = 0.5f;

    public MeshFilter frontalMeshFilter;
    public MeshFilter[] coneMeshFilters;
    public Material beamMaterial;
    public GameObject[] beamNodeObjects;
    public GameObject teleportNode;
    public GameObject startNode;
    public GameObject miniboss;

    private Mesh frontalMesh;
    private Mesh[] coneMeshes;
    private GameObject[] beams;

    private float timeSinceStart;
    private int activeBeam;

    private Vector3 playerPositionSnapshot;
    private Vector3 initialEulerAngles;

    private bool done = false;
    private bool paused = false;
    private bool showFrontal;
    private bool showCones;
    private bool showBeams;
    private bool alternateConeAngle;
    private GameObject bridge;
    private Timer timer;
    private float intermissionCooldownMultiplier;
    private float distanceToFloor;

    public Queue<Action> spells;
    public bool active;

    public AudioSource playerHitSound;

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    enum SpellID
    {
        FRONTAL,
        CONES,
        BEAMS
    }

    void Start()
    {
        active = false;
        SetupMeshes();
        SetupBeams();

        miniboss.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        initialEulerAngles = transform.eulerAngles;
        timeSinceStart = 0.0f;
        activeBeam = -1;

        distanceToFloor = 3f;
        intermissionCooldownMultiplier = 2f;

        timer = GameObject.Find("Timer").GetComponent<Timer>();

        bridge = GameObject.FindGameObjectWithTag("BossBridge");
        bridge.SetActive(false);

        // Queue up all spells
        spells = new Queue<Action>();

        SetupSpellQueue();
    }

    void Update()
    {
        Vector3 playerDir = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        playerDir.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(playerDir, Vector3.up);
        if (!paused)
            timeSinceStart += Time.deltaTime;

        if (done || timeSinceStart < 5.0f)
            return;

        if (spells.Count < 1 && !done)
        {
            done = true;
            EndFight();
            return;
        }

        if (!showFrontal && !showCones && !showBeams)
        {
            frontalMesh.Clear();
            ClearConeMeshes();
            ClearBeams();
        }
    }

    void LateUpdate()
    {
        if (showFrontal)
        {
            DrawFrontal();
        }
        if (showCones)
        {
            DrawCones();
        }
        if (showBeams)
        {
            DrawBeams();
        }
    }

    void EndFight()
    {

    }

    void QueuePhaseOneSpells()
    {
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(Pause);
        spells.Enqueue(Unpause);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(Pause);
        spells.Enqueue(Unpause);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);
    }
    void Pause()
    {
        spellCastTime *= 2f;
        spellCooldown *= 2f;
    }

    void Unpause()
    {
        spellCastTime /= 2f;
        spellCooldown /= 2f;
    }

    void QueuePhaseTwoSpells()
    {
        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(CastFrontal);
        spells.Enqueue(CastFrontal);

        spells.Enqueue(CastCones);
        spells.Enqueue(CastCones);

        spells.Enqueue(Pause);
    }

    /*  
     *
     * Spells and their Functions
     * 
     */

    void CastFrontal()
    {
        StartCoroutine("CastSpell", SpellID.FRONTAL);
    }

    void CastCones()
    {
        StartCoroutine("CastSpell", SpellID.CONES);
    }

    void CastBeams()
    {
        StartCoroutine("CastSpell", SpellID.BEAMS);
    }

    // START INTERMISSION AND END INTERMISSION AFFECT THE SPELL TIMER AND COOLDOWN
    void StartIntermission()
    {
        Debug.Log("Intermission Started");
        timer.PauseTimer();
        paused = true;
        bridge.SetActive(true);

        spellCastTime *= intermissionCooldownMultiplier;
        spellCooldown *= intermissionCooldownMultiplier;
    }

    public void EndIntermission()
    {
        Transform firstNode = miniboss.GetComponent<Enemy>().patrolPoints[0];
        miniboss.transform.position = firstNode.position;

        miniboss.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        miniboss.GetComponent<Enemy>().ChasePlayer();

        paused = false;
        timer.UnpauseTimer();
        bridge.SetActive(false);

        spellCastTime /= intermissionCooldownMultiplier;
        spellCooldown /= intermissionCooldownMultiplier;

        transform.position = teleportNode.transform.position;
    }

    // Called externally in EncounterStarter
    IEnumerator SpellQueue()
    {
        float timer = 0.0f;
        while (active)
        {
            yield return null;

            if (done || spells.Count < 1)
            {
                break;
            }
            if (timeSinceStart >= 5.0f && timer >= spellCooldown)
            {
                if (!paused)
                {
                    Action spellCast = spells.Dequeue();
                    spellCast();
                    timer = 0.0f;
                }

                else
                {
                    Action spellCast = spells.Peek();
                    spellCast();
                    timer = 0.0f;
                }
            }
            timer += Time.deltaTime;
        }
    }

    IEnumerator CastSpell(SpellID id)
    {
        float timer = 0.0f;
        switch (id)
        {
            case SpellID.FRONTAL:
                showFrontal = true;
                break;
            case SpellID.CONES:
                showCones = true;
                break;
            case SpellID.BEAMS:
                showBeams = true;
                break;
        }
        playerPositionSnapshot = GameObject.FindGameObjectWithTag("Player").transform.position;
        while (true)
        {
            yield return null;
            if (timer >= spellCastTime)
            {
                switch (id)
                {
                    case SpellID.FRONTAL:
                        showFrontal = false;
                        timer = 0.0f;
                        CheckForPlayerHit(frontalRadius, id);
                        break;
                    case SpellID.CONES:
                        showCones = false;
                        CheckForPlayerHit(coneRadius, id);
                        alternateConeAngle = !alternateConeAngle;
                        timer = 0.0f;
                        break;
                    case SpellID.BEAMS:
                        showBeams = false;
                        CheckForPlayerHit(beamWidth / 2.0f, id);
                        timer = 0.0f;
                        break;
                }
                playerHitSound.Play();
                break;
            }
            timer += Time.deltaTime;
        }
    }

    /*  
     *
     * Helper Functions
     * 
     */

    public void ResetEncounter()
    {
        StopAllCoroutines();
        SetupSpellQueue();
        frontalMesh.Clear();
        ClearConeMeshes();
        ClearBeams();
        transform.position = startNode.transform.position;
        bridge.SetActive(false);

        timeSinceStart = 0.0f;
        activeBeam = -1;
    }

    void SetupSpellQueue()
    {
        spells.Clear();
        QueuePhaseOneSpells();

        spells.Enqueue(StartIntermission);
        spells.Enqueue(CastBeams);

        QueuePhaseTwoSpells();
    }

    void SetupBeams()
    {
        List<GameObject> _beams = new List<GameObject>();
        foreach (GameObject node in beamNodeObjects)
        {
            var beamObj = new GameObject();
            beamObj.name = string.Format("beam");
            beamObj.transform.SetParent(transform);

            var beamRenderer = beamObj.AddComponent<LineRenderer>();

            beamRenderer.startWidth = beamRenderer.endWidth = 0f;
            beamRenderer.material = beamMaterial;
            
            beamRenderer.SetPosition(0, node.transform.position);
            beamRenderer.SetPosition(1, beamObj.transform.right * 24f);

            _beams.Add(beamObj);
        }

        beams = _beams.ToArray();
    }

    void SetupMeshes()
    {
        frontalMesh = new Mesh();
        List<Mesh> _coneMeshes = new List<Mesh>();
        for (int i = 0; i < 3; i++)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = string.Format("Cone View Mesh {0}", i);
            coneMeshFilters[i].mesh = newMesh;
            _coneMeshes.Add(newMesh);
        }

        coneMeshes = _coneMeshes.ToArray();

        frontalMesh.name = "Frontal View Mesh";
        frontalMeshFilter.mesh = frontalMesh;
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float radius)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, radius);

            bool edgeDistanceExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }

        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    ViewCastInfo ViewCast(float globalAngle, float radius)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, radius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }

        else
        {
            return new ViewCastInfo(false, transform.position + direction * radius, radius, globalAngle);
        }
    }

    void CheckForPlayerHit(float radius, SpellID id)
    {
        if (id == SpellID.BEAMS)
        {
            for (int i = 0; i < beamNodeObjects.Length; i++)
            {
                if (i != activeBeam)
                    continue;

                GameObject node = beamNodeObjects[i];
                Transform nodeTransform = node.transform;
                Collider[] targetsInCapsule = Physics.OverlapCapsule(nodeTransform.position, nodeTransform.right * 24f, radius, targetMask);

                for (int j = 0; j < targetsInCapsule.Length; j++)
                {
                    Transform target = targetsInCapsule[j].transform;
                    if (target.gameObject.tag != "Player")
                        continue;

                   PlayerManager.Instance.TakeLives(1);
                }
            }
        }
        else
        {
            Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);
            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                //Collider collider = targetsInRadius[i];
                Transform target = targetsInRadius[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (target.gameObject.tag != "Player")
                    continue;

                if (id == SpellID.CONES)
                {
                    float x = target.transform.position.x - transform.position.x;
                    float z = target.transform.position.z - transform.position.z;
                    float angleToPlayer = Mathf.Atan2(x, z) * 180 / Mathf.PI;

                    bool isInTopCone;
                    bool isInRightCone;
                    bool isInLeftCone;

                    if (!alternateConeAngle)
                    {
                        isInTopCone = angleToPlayer > -120f && angleToPlayer < -60f;
                        isInLeftCone = angleToPlayer > 120f && angleToPlayer < 180f;
                        isInRightCone = angleToPlayer > 0f && angleToPlayer < 60f;
                    }
                    else
                    {
                        isInTopCone = angleToPlayer > 60f && angleToPlayer < 120f;
                        isInLeftCone = angleToPlayer > -60f && angleToPlayer < 0f;
                        isInRightCone = angleToPlayer > -180f && angleToPlayer < -120f;
                    }

                    if (isInTopCone || isInLeftCone || isInRightCone)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.position);

                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                        {
                            IEnumerator pushCoroutine = PushPlayer(target.gameObject, directionToTarget);
                            StartCoroutine(pushCoroutine);
                        }
                    }
                }
                else if (id == SpellID.FRONTAL)
                {
                    Vector3 vectorToPlayer = (playerPositionSnapshot - transform.position).normalized;
                    if (Vector3.Angle(vectorToPlayer, directionToTarget) < viewAngle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.position);

                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                        {
                            IEnumerator pushCoroutine = PushPlayer(target.gameObject, vectorToPlayer);
                            StartCoroutine(pushCoroutine);
                        }
                    }
                }
            }
        }
    }

    IEnumerator PushPlayer(GameObject player, Vector3 pushVector)
    {
        float time = 0f;
        while(true)
        {
            yield return null;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            Vector3 playerPosition = player.transform.position;

            playerRb.AddForce(pushVector * pushForce, ForceMode.Impulse);
            if (time >= 0.1f)
            {
                break;
            }
            time += Time.deltaTime;
        }
    }

    /*  
     *
     * Draw Functions
     * 
     */

    void DrawFrontal()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        float x = playerPositionSnapshot.x - transform.position.x;
        float z = playerPositionSnapshot.z - transform.position.z;
        float angleToPlayer = Mathf.Atan2(x, z) * 180 / Mathf.PI;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = angleToPlayer - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, frontalRadius);

            if (i > 0)
            {
                bool edgeDistanceExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit && (oldViewCast.hit && newViewCast.hit && edgeDistanceExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, frontalRadius);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            Vector3 groundedPoint = newViewCast.point;
            groundedPoint.z -= distanceToFloor;
            viewPoints.Add(groundedPoint);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        Vector3 grounded = Vector3.zero;
        grounded.z -= distanceToFloor;
        vertices[0] = grounded;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        frontalMesh.Clear();
        frontalMesh.vertices = vertices;
        frontalMesh.triangles = triangles;
        frontalMesh.RecalculateNormals();
    }

    // MAGIC NUMBERS
    void DrawCones()
    {
        // 60f -> 6 pizza slices
        int stepCount = Mathf.RoundToInt(60f * meshResolution);
        float stepAngleSize = 60f / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        List<Vector3> viewPointsB = new List<Vector3>();
        List<Vector3> viewPointsC = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        ViewCastInfo oldViewCastB = new ViewCastInfo();
        ViewCastInfo oldViewCastC = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle;
            if (alternateConeAngle)
            {
                angle = initialEulerAngles.y - 180f / 2 + stepAngleSize * i;
            }
            else
            {
                angle = initialEulerAngles.y - 60f / 2 + stepAngleSize * i;
            }
            ViewCastInfo newViewCast = ViewCast(angle, coneRadius);
            ViewCastInfo newViewCastB = ViewCast(angle - 120f, coneRadius);
            ViewCastInfo newViewCastC = ViewCast(angle + 120f, coneRadius);

            if (i > 0)
            {
                bool edgeDistanceExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                bool edgeDistanceExceededB = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                bool edgeDistanceExceededC = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit && (oldViewCast.hit && newViewCast.hit && edgeDistanceExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, coneRadius);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
                if (oldViewCastB.hit != newViewCastB.hit && (oldViewCastB.hit && newViewCastB.hit && edgeDistanceExceededB))
                {
                    EdgeInfo edge = FindEdge(oldViewCastB, newViewCastB, coneRadius);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPointsB.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPointsB.Add(edge.pointB);
                    }
                }
                if (oldViewCastC.hit != newViewCastC.hit && (oldViewCastC.hit && newViewCastC.hit && edgeDistanceExceededC))
                {
                    EdgeInfo edge = FindEdge(oldViewCastC, newViewCastC, coneRadius);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPointsC.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPointsC.Add(edge.pointB);
                    }
                }
            }

            Vector3 groundedPoint = newViewCast.point;
            Vector3 groundedPointB = newViewCastB.point;
            Vector3 groundedPointC = newViewCastC.point;

            groundedPoint.z -= distanceToFloor;
            groundedPointB.z -= distanceToFloor;
            groundedPointC.z -= distanceToFloor;

            viewPoints.Add(groundedPoint);
            viewPointsB.Add(groundedPointB);
            viewPointsC.Add(groundedPointC);
            oldViewCast = newViewCast;
            oldViewCastB = newViewCastB;
            oldViewCastC = newViewCastC;
        }

        int vertexCount = viewPoints.Count + 1;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] verticesB = new Vector3[vertexCount];
        Vector3[] verticesC = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        int[] trianglesB = new int[(vertexCount - 2) * 3];
        int[] trianglesC = new int[(vertexCount - 2) * 3];

        Vector3 grounded = Vector3.zero;
        grounded.z -= distanceToFloor;

        vertices[0] = grounded;
        verticesB[0] = grounded;
        verticesC[0] = grounded;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            verticesB[i + 1] = transform.InverseTransformPoint(viewPointsB[i]);
            verticesC[i + 1] = transform.InverseTransformPoint(viewPointsC[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;

                trianglesB[i * 3] = 0;
                trianglesB[i * 3 + 1] = i + 1;
                trianglesB[i * 3 + 2] = i + 2;

                trianglesC[i * 3] = 0;
                trianglesC[i * 3 + 1] = i + 1;
                trianglesC[i * 3 + 2] = i + 2;
            }
        }

        ClearConeMeshes();
        coneMeshes[0].vertices = vertices;
        coneMeshes[0].triangles = triangles;
        coneMeshes[0].RecalculateNormals();

        coneMeshes[1].vertices = verticesB;
        coneMeshes[1].triangles = trianglesB;
        coneMeshes[1].RecalculateNormals();

        coneMeshes[2].vertices = verticesC;
        coneMeshes[2].triangles = trianglesC;
        coneMeshes[2].RecalculateNormals();
    }

    void DrawBeams()
    {
        if (activeBeam == -1)
            activeBeam = UnityEngine.Random.Range(0, beamNodeObjects.Length);

        for (int i = 0; i < beamNodeObjects.Length; i++)
        {
            if (i != activeBeam)
                continue;

            Transform nodeTransform = beamNodeObjects[i].transform;

            var lineRenderer = beams[i].GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, new Vector3(nodeTransform.position.x, nodeTransform.position.y, nodeTransform.position.z));
            lineRenderer.SetPosition(1, nodeTransform.right * 24f);

            lineRenderer.GetComponent<LineRenderer>().startWidth = beams[i].GetComponent<LineRenderer>().endWidth = beamWidth;
        }
    }

    void ClearBeams()
    {
        foreach (GameObject beam in beams)
        {
            beam.GetComponent<LineRenderer>().startWidth = beam.GetComponent<LineRenderer>().endWidth = 0f;
        }
        activeBeam = -1;
    }

    void ClearConeMeshes()
    {
        foreach(Mesh mesh in coneMeshes)
        {
            mesh.Clear();
        }
    }
}
