using System.Collections;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MonsterScript : MonoBehaviour
{
    private enum State
    {
        Waiting,
        Walking,
        Chasing,
        Distracted,
    }

    [SerializeField] private Waypoint _prevWaypoint, _currentWaypoint;
    [SerializeField] private Transform _playerBeingChased;

    [SerializeField] private LayerMask _distractionMask;
    [SerializeField] private LayerMask _wallMask;

    [SerializeField] private float _walkingSpeed, _chasingSpeed;
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private Transform[] _players;

    [SerializeField] private AudioSource _deathSound;
    [SerializeField] private MonoBehaviour _disableOnDeath;
    [SerializeField] private Waypoint _movableWaypoint;

    private Vector3 _lastPosition;
    private State _state = State.Waiting;

    private float timeStill = 0f;

    public void FixedUpdate()
    {
        if (_state is State.Distracted) return;

        switch (_state)
        {
            case State.Chasing:
                var seesPlayer = true; // !BlockedByWall(_playerBeingChased);
                var target = seesPlayer ? _playerBeingChased : _currentWaypoint.transform;
                MoveTowards(target, _chasingSpeed);

                var dist = Vector3.SqrMagnitude(_playerBeingChased.transform.position - transform.position);
                if (dist <= 5)
                {
                    StartCoroutine(Die());
                    return;
                }

                break;
            case State.Walking:
                MoveTowards(_currentWaypoint.transform, _walkingSpeed);
                CheckWaypoint();
                goto case State.Waiting;
            case State.Waiting:
                var playerGo = CheckPlayerInSight();
                if (playerGo == null) break;
                StopAllCoroutines();
                StartCoroutine(StartChasingPlayer(playerGo));
                break;
        }

        var distractableGo = CheckSightCone(_distractionMask);
        if (distractableGo == null) return;
        var distractable = distractableGo.GetComponentInParent<Distractable>();
        if (distractable == null) return;
        StopAllCoroutines();
        StartCoroutine(GetDistracted(distractable));
    }

    private void CheckWaypoint()
    {
        if (Vector3.SqrMagnitude(_lastPosition - transform.position) < 0.1f * Time.deltaTime)
        {
            timeStill += Time.deltaTime;
        }
        else
        {
            timeStill = 0;
        }

        _lastPosition = transform.position;

        if (timeStill < 2.5f)
        {
            return;
        }

        timeStill = 0;
        Debug.Log("Switched waypoint");

        if (Random.Range(0, 3) == 0)
        {
            _movableWaypoint.transform.position = _playerBeingChased.transform.position;
            _currentWaypoint = _movableWaypoint;
            return;
        }

        var allWaypoints = FindObjectsOfType<Waypoint>();
        var r = Random.Range(0, allWaypoints.Length);
        _currentWaypoint = allWaypoints[r];
    }

    private IEnumerator StartChasingPlayer(GameObject playerGo)
    {
        // TODO: Other laugh sound
        ShowTextOnScreen.ShowText("HA HA HA HA HA HA");
        _state = State.Distracted;
        _playerBeingChased = playerGo.transform;
        yield return new WaitForSeconds(3f);
        _state = State.Chasing;
    }

    private GameObject CheckPlayerInSight()
    {
        foreach (var player in _players)
        {
            if (BlockedByWall(player)) continue;
            if (!WithinSightAngle(player.transform.position - transform.position)) continue;
            return player.gameObject;
        }

        return null;
    }

    private GameObject CheckSightCone(LayerMask layerMask)
    {
        var hits = Physics.OverlapSphere(transform.position, 8f, layerMask);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (BlockedByWall(hit.transform)) continue;

            var hitDirection = hit.transform.position - transform.position;
            if (WithinSightAngle(hitDirection, 110))
            {
                return hit.gameObject;
            }
        }

        return null;
    }

    private bool WithinSightAngle(Vector3 direction, float deg = 65)
    {
        direction.y = 0f; // Ignore the y component

        // Calculate the forward direction of the object on the xz plane
        var forwardDirection = transform.forward;
        forwardDirection.y = 0f; // Ignore the y component

        // Calculate the dot product
        var dotProduct = Vector3.Dot(forwardDirection.normalized, direction.normalized);
        return dotProduct > Mathf.Cos(Mathf.Deg2Rad * deg);
    }

    private bool BlockedByWall(Transform target)
    {
        var posA = transform.position;
        var posB = target.position;
        posA.y = 0.5f;
        posB.y = 0.5f;
        return Physics.Linecast(posA, posB, _wallMask);
    }

    private IEnumerator GetDistracted(Distractable distractable)
    {
        // TODO: Start laughing
        _state = State.Distracted;
        distractable.SetState(false);
        yield return new WaitForFixedUpdate();
        _agent.stoppingDistance = 1;
        _agent.speed = _walkingSpeed;
        _agent.destination = distractable.transform.position;
        while (_agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return new WaitForFixedUpdate();
        }

        ShowTextOnScreen.ShowText("HA HA HA HA");

        yield return new WaitForSeconds(3f);
        _state = State.Walking;
        _currentWaypoint = distractable.ClosestWaypoint != null ? distractable.ClosestWaypoint : FindClosestWaypoint();
    }

    private Waypoint FindClosestWaypoint()
    {
        var points = FindObjectsOfType<Waypoint>();
        Waypoint closest = null;
        var distance = float.MaxValue;
        var pos = transform.position;
        foreach (var point in points)
        {
            var dist = Vector3.SqrMagnitude(point.transform.position - pos);
            if (dist < distance)
            {
                distance = dist;
                closest = point;
            }
        }

        return closest;
    }

    private void MoveTowards(Transform target, float speed)
    {
        var targetPos = target.position;
        _agent.speed = speed;
        _agent.stoppingDistance = 0;
        if (_agent.destination != targetPos) _agent.destination = targetPos;
    }

    private IEnumerator Die()
    {
        _state = State.Distracted;
        ShowTextOnScreen.ShowText("*player dying sounds*");
        _deathSound.Play();
        _disableOnDeath.enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
}