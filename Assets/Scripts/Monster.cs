using System.Collections;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
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

    private State _state;

    public void FixedUpdate()
    {
        if (_state is State.Distracted) return;

        switch (_state)
        {
            case State.Chasing:
                var seesPlayer = true; // !BlockedByWall(_playerBeingChased);
                var target = seesPlayer ? _playerBeingChased : _currentWaypoint.transform;
                MoveTowards(target, _chasingSpeed);
                break;
            case State.Walking:
                MoveTowards(_currentWaypoint.transform, _walkingSpeed);
                goto case State.Waiting;
            case State.Waiting:
                var playerGo = CheckPlayerInSight();
                if (playerGo == null) break;
                StopAllCoroutines();
                StartCoroutine(StartChasingPlayer(playerGo));
                break;
        }

        CheckWaypoint();

        var distractableGo = CheckSightCone(_distractionMask);
        if (distractableGo == null) return;
        var distractable = distractableGo.GetComponentInParent<Distractable>();
        if (distractable == null) return;
        StopAllCoroutines();
        StartCoroutine(GetDistracted(distractable));
    }

    private void CheckWaypoint()
    {
        var distVector = transform.position - _currentWaypoint.transform.position;
        distVector.y = 0;
        var distFromWaypoint = Vector3.SqrMagnitude(distVector);
        if (distFromWaypoint > 0.75f * 0.75f) return;
        Debug.Log("Hit waypoint!");
        _prevWaypoint = _currentWaypoint;

        if (_currentWaypoint.Neighbors.Length == 1)
        {
            _currentWaypoint = _currentWaypoint.Neighbors[0];
            return;
        }

        var remainingWaypoints = _currentWaypoint.Neighbors.Where(w => w != _prevWaypoint).ToArray();
        if (remainingWaypoints.Length == 1)
        {
            _currentWaypoint = remainingWaypoints[0];
            return;
        }

        if (!_playerBeingChased)
        {
            var r = Random.Range(0, remainingWaypoints.Length);
            _currentWaypoint = remainingWaypoints[r];
        }

        var closestWaypoint = remainingWaypoints.OrderBy(w => Vector3.SqrMagnitude(w.transform.position - _playerBeingChased.position)).First();
        _currentWaypoint = closestWaypoint;
    }

    private IEnumerator StartChasingPlayer(GameObject playerGo)
    {
        // TODO: Other laugh sound
        Debug.Log("Spotted player!");
        _state = State.Distracted;
        _playerBeingChased = playerGo.transform;
        yield return new WaitForSeconds(1.5f);
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
        var hits = Physics.OverlapSphere(transform.position, 10f, layerMask);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (BlockedByWall(hit.transform)) continue;

            var hitDirection = hit.transform.position - transform.position;
            if (WithinSightAngle(hitDirection))
            {
                return hit.gameObject;
            }
        }

        return null;
    }

    private bool WithinSightAngle(Vector3 direction)
    {
        direction.y = 0f; // Ignore the y component

        // Calculate the forward direction of the object on the xz plane
        var forwardDirection = transform.forward;
        forwardDirection.y = 0f; // Ignore the y component

        // Calculate the dot product
        var dotProduct = Vector3.Dot(forwardDirection.normalized, direction.normalized);
        return dotProduct > 0.819f;
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
        Debug.Log("Got distracted!");
        _state = State.Distracted;
        distractable.SetState(false);
        _agent.stoppingDistance = 2;
        _agent.speed = _walkingSpeed;
        _agent.destination = distractable.transform.position;
        while (_agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1.5f);
        _state = State.Walking;
        _currentWaypoint = distractable.ClosestWaypoint;
    }

    private void MoveTowards(Transform target, float speed)
    {
        var targetPos = target.position;
        _agent.speed = speed;
        _agent.stoppingDistance = 0;
        if (_agent.destination != targetPos) _agent.destination = targetPos;
    }
}