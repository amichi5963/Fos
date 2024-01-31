using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum MonsterState
{
    Patrolling,
    Collecting,
    Fighting,
}
[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    [SerializeField] MonsterState monsterState = MonsterState.Patrolling;

    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float collectSpeed = 3.5f;
    [SerializeField] float fightSpeed = 3.5f;

    //戦闘まわり
    List<GameObject> FusedCreatures = new List<GameObject>();
    [SerializeField] List<Monster> Kobuns = new List<Monster>();
    Monster Oyabun = null;
    [SerializeField] float FightEndDistance = 10f;
    [SerializeField] float FightDistance = 4f;
    [SerializeField] float AtackInterval = 1f;
    private float atackTimer = 0.0f;
    [SerializeField] float MoveInterval = 2f;
    private float moveTimer = 0.0f;
    private bool isAtacking = false;
    [SerializeField] float atackRadius = 2f;
    [SerializeField] float atackPower = 20f;
    GameObject target = null;

    //草のばしまわり
    Vector3 rootPos = Vector3.zero;
    float grassHeight = 0.2f;
    [SerializeField] GameObject Vine;
    bool isVineExtending = false;
    bool isVineAtacking = false;
    Vector3 vineAtackStartPos = Vector3.forward;
    GameObject GrassParent;
    //　現在の角度
    private float vineAtackAngle = 0f;
    //　回転するスピード
    [SerializeField]
    private float rotateSpeed = 180f;
    [SerializeField] GameObject GrassAtackEffect;

    //石重ねまわり
    public const int FUSES_MAX = 6;
    [SerializeField] GameObject[] Fuses = new GameObject[FUSES_MAX];
    [SerializeField] bool isStone = true;

    //投げる強さ
    public float throwForce = 10.0f;

    //プレイヤーにどの程度近づいたら回収したことにするか
    [SerializeField]
    private float _collectDistance = 0.3f;

    //この時間たったら動くのをやめる
    [SerializeField]
    private float maxCollectTimer = 10.0f;
    private float timer = 0.0f;


    //視野範囲
    [SerializeField]
    private float searchAngle = 130f;
    [SerializeField]
    private float searchLength = 10f;

    //NavMesh関連
    private NavMeshAgent agent;
    public Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    private Vector3 startPos;
    Vector3 randomWalkPos;
    [SerializeField] float randomWalkRange = 4f;
    private bool canFall = true;
    float navMeshRadius = 0.5f;

    private GameObject _playerObject = null;
    PlayerController _playerController;

    Rigidbody rb;
    Collider col;

    [SerializeField] float respawnHeight = 0f;
    [SerializeField] float respawnTime = 3f;
    [SerializeField] float respawnDistance = 100000f;

    [SerializeField] GameObject Kemuri;
    [SerializeField] GameObject Bikkuri;

    [SerializeField] GameObject model;

    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource source;
    [SerializeField] List<AudioClip> RideSE = new List<AudioClip>();

    [SerializeField] List<AudioClip> ThrowSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> AtackSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> FoundSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> ReplySE = new List<AudioClip>();
    [SerializeField] List<AudioClip> GroundSE = new List<AudioClip>();

    float respawnTimer = -1f;
    public bool IsStone()
    {
        return isStone;
    }
    private void Awake()
    {
        source = GetComponents<AudioSource>()[1];
    }

    private void Start()
    {
        if (!isStone)
        {
            GrassParent = Fuses[0].transform.parent.gameObject;
        }
        _playerObject = GameObject.Find("Player");
        _playerController = _playerObject.GetComponent<PlayerController>();
        if (_playerObject == null)
        {
            Debug.LogError("プレイヤーが見つかりませんでした");
        }
        else
        {
            target = _playerObject;
        }

        startPos = transform.position;
        randomWalkPos = startPos;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        if (Kobuns.Count > 0)
        {
            foreach (var kobun in Kobuns)
            {
                kobun.Oyabun = this;
            }
        }
    }
    private void FixedUpdate()
    {

        if (isAtacking)
            Atack();
    }

    void Atack()
    {
        float radius = atackRadius;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (monsterState == MonsterState.Fighting && collider.CompareTag("Player"))
            {
                _playerObject.GetComponent<Rigidbody>().AddForce(
                    Vector3.ProjectOnPlane((_playerObject.transform.position - transform.position), Vector3.up).normalized
                    * atackPower, ForceMode.Impulse);
                Instantiate(Kemuri, transform.position, transform.rotation, null);
                isAtacking = false;
            }
            if (collider.CompareTag("Breakable"))
            {
                Instantiate(Kemuri, transform.position, transform.rotation, null);
                collider.gameObject.GetComponent<BreakableObject>().killMe();
            }
            if ((isStone && collider.CompareTag("Grass"))
                || (!isStone && collider.CompareTag("Stone")))
            {
                Monster monster = collider.gameObject.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.FinishFight();
                    monster.kaijo();
                    monster.GetComponent<Rigidbody>().AddForce(
                    Vector3.ProjectOnPlane(collider.transform.position - transform.position, Vector3.up).normalized
                    * atackPower + Vector3.up, ForceMode.Impulse);
                    Instantiate(Kemuri, transform.position, transform.rotation, null);
                    isAtacking = false;
                }
            }
        }
    }

    public void Respawn()
    {
        Instantiate(Kemuri, transform.position, transform.rotation, null);
        Drop();
        if (Kobuns.Count > 0)
        {
            releaseAll();
            foreach (var kobun in Kobuns)
            {
                if (kobun.transform.parent == null)
                    kobun.Respawn();
            }
        }
        //FinishFight();
        col.isTrigger = false;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = true;
        agent.enabled = false;
        transform.position = startPos;
        randomWalkPos = startPos;
        Instantiate(Kemuri, transform.position, transform.rotation);
    }
    Vector3 pre3 = Vector3.one;
    Vector3 now3 = Vector3.zero;
    private void Update()
    {
        pre3 = now3;
        now3 = transform.position;

        //このタイミングでリスポーン処理
        if (respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                Respawn();
            }
        }

        //草戦闘状態の確認
        isVineExtending = (!isStone && FusedCreatures.Count >= 1);

        //落下時にリスポーン
        if (transform.position.y < respawnHeight && respawnTimer <= 0)
        {
            respawnTimer = respawnTime;
            return;
        }

        //戻れなさそうならリスポーン
        if ((startPos - transform.position).sqrMagnitude > respawnDistance * respawnDistance && respawnTimer <= 0)
        {
            respawnTimer = respawnTime;
            return;
        }

        //沼に入ったら落下
        if (SwampCheck())
        {
            agent.enabled = false;
            rb.isKinematic = false;
            //col.isTrigger = true;
            if (respawnTimer <= 0)
                respawnTimer = respawnTime;
            return;
        }

        //草戦闘時でなければ接地判定を行う
        if (!agent.enabled && !isVineExtending)
        {
            //接地判定を行う
            RaycastHit hit;
            Physics.SphereCast(transform.position, 0.4f, Vector3.down, out hit, 1f, -1, QueryTriggerInteraction.Ignore);

            if (hit.collider != null && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Grass") && !hit.collider.CompareTag("Stone"))
            {
                isAtacking = false;
                //Navmesh外に着地した場合はリスポーン予約してリターン
                NavMeshHit navMeshHit = new NavMeshHit();
                if (!NavMesh.SamplePosition(transform.position, out navMeshHit, navMeshRadius, 1))
                {
                    if (respawnTimer <= 0)
                    {
                        respawnTimer = respawnTime;
                    }
                    return;
                }

                // 投げ状態からNavMeshエージェントに制御を切り替える
                PlaySE(GroundSE);
                agent.enabled = true;
                rb.isKinematic = true;
                randomWalkPos = transform.position;
                //一旦目的地をリセット
                agent.SetDestination(transform.position);
            }
            return;
        }

        //ここまで問題なかったらリスポーン予約解除
        respawnTimer = -1f;

        //草戦闘時でなければNavMeshエージェントに制御を切り替える
        if (!isVineExtending)
        {
            rb.isKinematic = true;
            agent.enabled = true;
        }

        //キャラの向きを進行方向に向かせます
        if (Vector3.ProjectOnPlane(now3 - pre3, Vector3.up) != Vector3.zero)
            model.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(now3 - pre3, Vector3.up));

        //同種なら特定の距離まで近づいたら回収完了
        if (!CollectingCheck())
        {
            var diff = _playerObject.transform.position - transform.position;
            if (diff.sqrMagnitude < _collectDistance * _collectDistance)
            {
                FinishCollect();
            }
        }

        switch (monsterState)
        {
            //平時の行動
            case MonsterState.Patrolling:
                agent.speed = patrolSpeed;
                Patrol();
                return;

            //回収時の行動
            case MonsterState.Collecting:
                agent.speed = collectSpeed;
                Collecting();
                return;

            //戦闘時の行動
            case MonsterState.Fighting:
                agent.speed = fightSpeed;
                Fight();
                return;
        }
    }

    bool SwampCheck()
    {
        //石が沼に入ったかどうか
        if (isStone)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, navMeshRadius, 1 << 3))
            {
                return true;
            }
        }
        return false;
    }
    //回収時の行動
    private void Collecting()
    {
        timer += Time.deltaTime;

        //回収の最大時間を超えていないかチェック　超えていたらAI移動に戻る
        if (timer > maxCollectTimer)
        {
            timer = 0.0f;
            monsterState = MonsterState.Patrolling;
            return;
        }
        //プレイヤーが同種に寄生していない場合　AI移動に戻る
        if (CollectingCheck())
        {
            monsterState = MonsterState.Patrolling;
            return;
        }
        //乗れる距離に近づいてたら何もしない(プレイヤーを押し続けてしまうため)
        Vector3 diff = _playerObject.transform.position - transform.position;
        if (diff.sqrMagnitude < _collectDistance * _collectDistance)
        {
            if (!agent.pathPending)
                agent.SetDestination(transform.position);
            return;
        }

        //オフメッシュリンクを使って計算
        agent.autoTraverseOffMeshLink = true;
        RaycastHit hitInfo;
        Physics.Raycast(_playerObject.transform.position, Vector3.down, out hitInfo, 5f, -1, QueryTriggerInteraction.Ignore);
        //プレイヤーに向かって進ませる
        if (hitInfo.collider != null && !agent.pathPending && canFall)
            agent.SetDestination(hitInfo.point);
        //OffmeshLinkの上にいるとき、本当にオフメッシュリンクを使っていいのか検証
        if (agent.isOnOffMeshLink == true && agent.currentOffMeshLinkData.endPos.y < hitInfo.point.y - 0.5f)
        {
            //落ちる先が沼ならむしろ降りてリスポーン
            NavMeshHit swampHit;
            if (isStone && NavMesh.SamplePosition(agent.currentOffMeshLinkData.endPos, out swampHit, 0.5f, 1 << 3))
            {
                return;
            }
            agent.autoTraverseOffMeshLink = false;
            canFall = false;
            //プレイヤーに向かって進ませる
            if (hitInfo.collider != null && !agent.pathPending)
                agent.SetDestination(hitInfo.point);
        }
        else
        {
            canFall = true;
        }
        return;
    }
    //戦闘時の行動
    private void Fight()
    {
        agent.autoTraverseOffMeshLink = true;
        //子分の挙動
        if (Oyabun != null)
        {
            //親分に向かって進ませる
            if (!agent.pathPending && Oyabun.gameObject.activeSelf)
                agent.SetDestination(Oyabun.transform.position);

            //特定の距離まで近づいたら回収完了
            var diff = Oyabun.transform.position - transform.position;
            if (diff.magnitude < _collectDistance && monsterState == MonsterState.Fighting)
            {
                if (Oyabun.addCreature(this.gameObject, 1) <= 0)
                {
                    Oyabun.setRootPos(Oyabun.transform.position);
                    gameObject.SetActive(false);
                }
            }
        }
        //親分の挙動
        else
        {
            //プレイヤーの自分と同種族寄生時、もしくは距離がFigheEndDistance以上離れたとき、戦闘状態を解除
            if ((target == _playerObject && isStone && _playerController.getStonesCount() > 0)
                || (target == _playerObject && !isStone && _playerController.getGrassesCount() > 0)
                || target.activeSelf == false
                || (target.transform.position - transform.position).sqrMagnitude
                > FightEndDistance * FightEndDistance
                )
            {
                FinishFight();
                return;
            }

            //石キャラの行動パターン
            if (isStone)
            {
                model.transform.rotation = Quaternion.LookRotation(target.transform.position - this.transform.position);
                moveTimer += Time.deltaTime;
                if (moveTimer > MoveInterval && FusedCreatures.Count != 0)
                {
                    moveTimer = 0f;
                    //ターゲットからFightDiscanceの距離のランダムな点を目指す
                    float dx = transform.position.x - target.transform.position.x;
                    float dy = transform.position.y - target.transform.position.y;
                    float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg
                        + Random.Range(-90f, 90f);
                    float x = FightDistance * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float z = FightDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
                    agent.SetDestination(target.transform.position + new Vector3(x, 0f, z));
                }

                //仲間を重ねていないとき　何も起こらない
                if (FusedCreatures.Count == 0) return;

                atackTimer += Time.deltaTime;

                if (atackTimer >= AtackInterval)
                {
                    atackTimer = 0f;
                    transform.rotation =
                    (Quaternion.LookRotation(target.transform.position - transform.position));
                    //石は投げ攻撃を行う

                    //石を載せていてAtackInterval時間が経過した際、頂点の石を投げる
                    Fuses[FusedCreatures.Count - 1].SetActive(false);

                    int lastIndex = FusedCreatures.Count - 1;
                    // プレファブを元にゲームオブジェクトを取得
                    GameObject throwstone = FusedCreatures[lastIndex];
                    Monster monster = throwstone.GetComponent<Monster>();
                    monster.Drop();
                    throwstone.transform.position = Fuses[lastIndex].transform.position + transform.up;
                    throwstone.transform.rotation = this.transform.rotation;
                    monster.Jump((transform.forward + Vector3.up).normalized);
                    FusedCreatures.RemoveAt(lastIndex);
                }
            }

            //草キャラの行動パターン
            else
            {
                if (FusedCreatures.Count >= 1)
                {
                    Fuses[0].transform.position = rootPos + Vector3.down + new Vector3(0f, grassHeight, 0f);
                    Fuses[0].transform.parent = transform.parent;
                    agent.enabled = false;
                    rb.isKinematic = false;
                    rb.useGravity = false;

                    if (isVineAtacking)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        col.isTrigger = true;
                        //　ユニットの角度を変更
                        vineAtackAngle += rotateSpeed * Time.deltaTime;
                        //360を超えるなら回転終了
                        if (vineAtackAngle > 360f)
                        {
                            col.isTrigger = false;
                            isAtacking = false;
                            vineAtackAngle = 0f;
                            isVineAtacking = false;
                            atackTimer = 0f;
                            transform.position = vineAtackStartPos;
                        }
                        float eased = (vineAtackAngle / 360) * (vineAtackAngle / 360) * (3 - 2 * (vineAtackAngle / 360));
                        //　ユニットの位置 = ターゲットの位置 ＋ ターゲットから見たユニットの角度 ×　ターゲットからの距離
                        transform.position = rootPos
                            + Quaternion.Euler(0f, eased * 360f, 0f) * (vineAtackStartPos - rootPos);

                        transform.rotation = Quaternion.Euler(0f, vineAtackAngle, 0f);

                        return;
                    }

                    atackTimer += Time.deltaTime;
                    if (atackTimer >= AtackInterval)
                    {
                        atackTimer = 0f;
                        vineAtackStartPos = transform.position;
                        isVineAtacking = true;
                        isAtacking = true;
                        PlaySE(AtackSE);
                        Invoke("grassAtackEffect", 0.45f);
                        //草は攻撃を行う
                    }

                    //ターゲットから根っこを挟んで反対の点を目指す
                    Vector3 togo = rootPos - (target.transform.position - rootPos);

                    rb.AddForce((togo - transform.position).normalized * fightSpeed, ForceMode.Acceleration);

                    if (Vine != null)
                    {
                        Vine.SetActive(true);
                    }
                }
            }
        }
    }
    void grassAtackEffect()
    {
        float effectSize = (transform.position - rootPos).magnitude / 2;
        Instantiate(GrassAtackEffect, rootPos, transform.rotation, null).transform.localScale = new Vector3(effectSize, 1f, effectSize);
    }
    public void FinishFight()
    {
        if (!isStone)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1f, -1, QueryTriggerInteraction.Ignore);
            if (hit.collider == null)
            {
                transform.position = rootPos;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            Fuses[0].transform.position = transform.position + Vector3.up;
            Fuses[0].transform.rotation = transform.rotation;
            Fuses[0].transform.parent = GrassParent.transform;
        }
        foreach (var kobun in Kobuns)
        {
            kobun.monsterState = MonsterState.Patrolling;
            kobun.isAtacking = false;
        }
        if (Vine != null)
        {
            Vine.SetActive(false);
        }
        isAtacking = false;

        monsterState = MonsterState.Patrolling;
        //ナビメッシュ上に居るなら目的地をリセット
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position);
        releaseAll();
    }
    public void releaseAll()
    {
        monsterState = MonsterState.Patrolling;
        isAtacking = false;
        isVineAtacking = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        col.isTrigger = false;
        atackTimer = 0f;
        foreach (var kobun in Kobuns)
        {
            kobun.randomWalkPos = kobun.transform.position;
            kobun.monsterState = MonsterState.Patrolling;
        }
        for (int i = 0; i < Fuses.Length; i++)
        {
            Fuses[i].SetActive(false);
        }
        while (FusedCreatures.Count >= 1)
        {
            int lastIndex = FusedCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject releasedMonster = FusedCreatures[lastIndex];
            Monster monster = releasedMonster.GetComponent<Monster>();
            monster.Drop();
            if (isStone)
            {
                releasedMonster.transform.position = Fuses[FusedCreatures.Count - 1].transform.position;
                releasedMonster.transform.rotation = this.transform.rotation;
            }
            else
            {
                releasedMonster.transform.position = rootPos;
                releasedMonster.transform.rotation = this.transform.rotation;
            }
            FusedCreatures.RemoveAt(lastIndex);
            monster.kaijo();
        }
        FusedCreatures.Clear();
    }
    int addCreature(GameObject gameObject, int num)
    {
        if (FusedCreatures.Count < FUSES_MAX)
        {
            PlaySE(RideSE);
            FusedCreatures.Add(gameObject);
            Instantiate(Kemuri, Fuses[FusedCreatures.Count - 1].transform);
            Fuses[FusedCreatures.Count - 1].SetActive(true);
            return num - 1;
        }
        return num;
    }
    //平時のAI移動
    private void Patrol()
    {
        agent.autoTraverseOffMeshLink = false;
        //戦闘を開始できるのは親分だけ
        if (Kobuns.Count > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchLength);
            foreach (Collider collider in colliders)
            {
                //　対象の方向
                var foundDirection = collider.transform.position - transform.position;
                //　敵の前方からの対象の方向
                var angle = Vector3.Angle(transform.forward, foundDirection);

                //　サーチする角度内で対象と自分の種族が異なるなら発見
                if (angle <= searchAngle)
                {
                    if (collider.CompareTag("Player") && (
                    (_playerController.getStonesCount() == 0 && isStone) ||
                    (_playerController.getGrassesCount() == 0 && !isStone)
                    ) && _playerController.isApplicationPause() == false)
                    {
                        Found(collider.gameObject);
                        return;
                    }
                    else if ((collider.CompareTag("Stone") && !isStone) || (collider.CompareTag("Grass") && isStone))
                    {
                        Found(collider.gameObject);
                        return;
                    }
                }
            }
        }

        //　目的地に到着したら次の目的地を設定
        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNextPatrolPoint();
        }
    }
    private void Found(GameObject foundObject)
    {
        if (foundObject == null)
        {
            return;
        }
        target = foundObject;
        PlaySE(FoundSE);
        Instantiate(Bikkuri, transform);
        monsterState = MonsterState.Fighting;
        foreach (var kobun in Kobuns)
        {
            kobun.monsterState = MonsterState.Fighting;
        }
        return;
    }
    //次の地点へ移動
    private void SetNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
        else agent.SetDestination(randomWalkPos + new Vector3(Random.Range(0, randomWalkRange), 0, Random.Range(0, randomWalkRange)));
    }
    /// <summary>
    /// オブジェクトを出現させる
    /// </summary>
    public void Drop()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 回収を開始する
    /// </summary>
    public void Collect()
    {
        if (monsterState == MonsterState.Collecting)
        {
            return;
        }
        PlaySE(ReplySE);
        timer = 0.0f;
        monsterState = MonsterState.Collecting;
        isAtacking = false;
        agent.enabled = true;
        rb.isKinematic = true;

        //念のため見える状態にしておく
        Drop();
    }

    public void kaijo()
    {
        agent.enabled = false;
        rb.isKinematic = false;
    }

    //自身を投擲する
    public void Jump(Vector3 throwDirection)
    {
        PlaySE(ThrowSE);
        isAtacking = true;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);
    }
    public void FinishCollect()
    {
        if (isStone)
        {
            if (_playerController.addStone(this.gameObject, 1) <= 0)
            {
                gameObject.SetActive(false);
            }
            //monsterState = MonsterState.Patrolling;
        }
        else
        {
            if (_playerController.addGrass(this.gameObject, 1) <= 0)
            {
                Vine.SetActive(false);
                gameObject.SetActive(false);
            }
            //monsterState = MonsterState.Patrolling;
        }
    }
    public bool CollectingCheck()
    {
        if (isStone)
            return (_playerController.getStonesCount() == 0);
        else
            return (_playerController.getGrassesCount() == 0);
    }
    public void setRootPos(Vector3 _root)
    {
        rootPos = _root;
    }
    public bool getAtacking()
    {
        return isAtacking;
    }
    public void SetVelocity(Vector3 vector3)
    {
        rb.AddForce(vector3, ForceMode.VelocityChange);
    }
    public void PlaySE(List<AudioClip> SE)
    {
        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(SE[Random.Range(0, SE.Count)]);
    }
}