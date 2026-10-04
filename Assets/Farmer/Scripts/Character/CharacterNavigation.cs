using UnityEngine;
using UnityEngine.AI;

public sealed class CharacterNavigation
{
    private readonly NavMeshAgent agent;

    public CharacterNavigation(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public bool TryMoveTo(Vector3 destination)
    {
        return agent != null && agent.enabled && agent.isOnNavMesh && agent.SetDestination(destination);
    }

    public bool HasArrived()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return true;

        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.01f);
    }
}
