using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    private NavMeshAgent _agent;
    private Animator _animator;
    [SerializeField] public Transform[] _patrolPoints;
    private int _currentPP;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [SerializeField] public float waitTime;
    [SerializeField] public LayerMask floorLayerMask;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _currentPP = 0;
        StartPatrolling();
    }

    void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance)
        {
            _currentPP++;
            if(_currentPP >= _patrolPoints.Length) _currentPP = 0;
            StartPatrolling();
        }

        if (_agent.remainingDistance > 5f)
        {
            _agent.speed = runSpeed;
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _agent.speed = walkSpeed;
            _animator.SetBool("isRunning", false);
        }

        HandleMoveInput();
        _animator.SetFloat("movementSpeed", Mathf.Abs(Mathf.Sqrt((_agent.velocity.x* _agent.velocity.x) + (_agent.velocity.z * _agent.velocity.z))));
    }

    void HandleMoveInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Clicked!");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorLayerMask))
            {
                _agent.SetDestination(hit.point);
            }
        }
    }

    async void StartPatrolling()
    {
        await Task.Delay((int)(waitTime * 1000));
        _agent.SetDestination(_patrolPoints[_currentPP].position);
    }
}
