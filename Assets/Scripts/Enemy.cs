using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum State {IdleState,ChaseState,AttackState}
    State currentState;

    public ParticleSystem deathEffect;
    NavMeshAgent pathfinder;
    private Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color ogColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>(); base.Start();

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            hasTarget = true;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        }
    }
    protected override void Start()
    {
        base.Start();
        
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            currentState = State.ChaseState;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
    }
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if(damage>=health)
        {
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)),deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        { 
              if (nextAttackTime < Time.time)
              {
                  float sqrDistance = (target.position - transform.position).sqrMagnitude;
                  if (sqrDistance < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                  {
                      nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                  }
              }
        }
    }
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.IdleState;
    }
    public void SetCharacteristics(float speed,int health, int hitsToKill,Color skinColor)
    {
        pathfinder.speed = speed;
        startingHealth = health;
        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKill);
        }
        skinMaterial = GetComponent<Renderer>().sharedMaterial;
        skinMaterial.color = skinColor;
        ogColor = skinMaterial.color;
    }

    IEnumerator Attack()
    {
        currentState = State.AttackState;
        pathfinder.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
        float attackSpeed = 3f;
        float percent=0f;
        skinMaterial.color = Color.gray;
        bool hasAppliedDamage = false;
        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4f;
            transform.position = Vector3.Lerp(originalPosition, attackPosition , interpolation);
            yield return null;
        }
        skinMaterial.color = ogColor;
        pathfinder.enabled = true;
        currentState = State.ChaseState;
    }
    IEnumerator UpdatePath()
    {
        float refreshRate=.25f;
         while (hasTarget)
         {
            if (currentState == State.ChaseState)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius+targetCollisionRadius+attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
                yield return new WaitForSeconds(refreshRate);  
         }

    }
}
