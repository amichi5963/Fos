using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : PlayerMover
{
    //石寄生まわり
    public const int STONES_MAX = 6;
    int hostStone = 0;
    [SerializeField] GameObject[] Stones = new GameObject[STONES_MAX];
    List<LineRenderer> trajectoryLines = new List<LineRenderer>();
    List<GameObject> StoneCreatures = new List<GameObject>();
    public float throwForce = 10.0f;

    //草寄生まわり
    public const int GRASSES_MAX = 2;
    [SerializeField] GameObject[] Grasses = new GameObject[GRASSES_MAX];
    [SerializeField] GameObject Vine;
    [SerializeField] GameObject GrassesParent;
    List<GameObject> GrassCreatures = new List<GameObject>();
    [SerializeField] float GRASS_LENGTH_MAX = 6f;
    bool isVineExtending = false;
    bool isVineAtacking = false;
    Vector3 grassLocalPos = Vector3.zero;
    //　現在の角度
    float vineAtackAngle = 0f;
    //　回転するスピード
    [SerializeField] private float rotateSpeed = 180f;
    private Vector3 vineAtackStartPos = Vector3.forward;
    [SerializeField] GameObject GrassAtackEffect;

    //本体
    public float callRadius = 5f;
    public float parasiteRadius = 2f;
    public float acc = 5f;
    [SerializeField] float jumpPower = 5f;
    float reJumpTimer = 0f;
    float reJumpTime = 0.5f;
    [SerializeField] GameObject MainBody = default!;
    [SerializeField] float KinokoMass = 1.0f;
    [SerializeField] float StoneMass = 1.0f;
    [SerializeField] float GrassMass = 1.0f;
    Rigidbody rb;

    //カメラ入力も取得
    [SerializeField] float throwAngleSpeedX = 2.0f;
    [SerializeField] float throwAngleSpeedY = 2.0f;
    [SerializeField] InputActionReference XYAxis;

    [SerializeField] float respawnHeight = 0f;

    bool isOnGround = false;
    bool isTurning;
    [SerializeField] GameObject Hani;
    [SerializeField] GameObject CallRange;
    [SerializeField] GameObject Kemuri;

    //カメラが追尾する対象
    [SerializeField] GameObject LookAt;
    [SerializeField] GameObject ashimoto;

    //表示するキノコはどっちか
    [SerializeField] GameObject[] ArukiKinoko = default!;
    [SerializeField] GameObject KiseiKinoko = default!;
    [SerializeField] GameObject NokoriKinoko = default!;
    [SerializeField] GameObject OmakeKinoko = default!;

    //リスポーン
    Vector3 respawnPos;

    //戻る時間
    float shrinkTime = 0.2f;
    float shrinkTimer = -1f;
    [SerializeField] float ShrinkPower = 20f;

    List<Monster> monsters = new List<Monster>();

    //音関連

    [SerializeField] StepSE kinokoStepSE;
    [SerializeField] StepSE grassStepSE;
    [SerializeField] StepSE stoneStepSE;

    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource source;
    [SerializeField] List<AudioClip> ParasiteSE = new List<AudioClip>();

    [SerializeField] List<AudioClip> GrassRideSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> StoneRideSE = new List<AudioClip>();

    [SerializeField] List<AudioClip> StoneThrowSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> GrassAtackSE = new List<AudioClip>();

    [SerializeField] List<AudioClip> GrassCallSE = new List<AudioClip>();
    [SerializeField] List<AudioClip> StoneCallSE = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponents<AudioSource>()[0];

        respawnPos = transform.position;
        rb = GetComponent<Rigidbody>();
        ClearAll();
        grassLocalPos = Grasses[1].transform.localPosition;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 10000000);
        foreach (Collider collider in colliders)
        {
            Monster monster = collider.GetComponent<Monster>();
            if (monster != null)
            {
                monsters.Add(monster);
            }
        }
        foreach (var stone in Stones)
        {
            trajectoryLines.Add(stone.GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var line in trajectoryLines)
        {
            line.enabled = false;
        }
        if (isTurning)
        {
            Stones[StoneCreatures.Count - 1].GetComponent<LineRenderer>().enabled = true;
        }

        if (StoneCreatures.Count > 0)
        {
            rb.mass = StoneMass;
        }
        else if (GrassCreatures.Count > 0)
        {
            rb.mass = GrassMass;
        }
        else rb.mass = KinokoMass;

        if (shrinkTimer > 0f && !isVineAtacking)
        {
            shrinkTimer -= Time.deltaTime;

            if (!isOnGround)
                rb.AddForce((Grasses[1].transform.position + Vector3.up - transform.position) * Time.deltaTime / shrinkTime * ShrinkPower, ForceMode.VelocityChange);

            if (shrinkTimer <= 0f)
            {
                isVineExtending = false;
                if (!isOnGround)
                {
                    transform.position = Grasses[1].transform.position + Vector3.up;
                    rb.velocity = Vector3.zero;
                }
                Grasses[1].transform.rotation = this.transform.rotation;
                Grasses[1].transform.parent = GrassesParent.transform;
                Grasses[1].transform.localPosition = grassLocalPos;
                rb.useGravity = true;
                ashimoto.transform.parent = this.transform;
                ashimoto.transform.localPosition = Vector3.zero;
            }
            return;
        }

        if (StoneCreatures.Count > 0)
        {
            foreach (var kino in ArukiKinoko)
                kino.SetActive(false);
            KiseiKinoko.SetActive(true);
        }
        else if (GrassCreatures.Count > 0)
        {
            foreach (var kino in ArukiKinoko)
                kino.SetActive(false);
        }
        else
        {
            foreach (var kino in ArukiKinoko)
                kino.SetActive(true);
            KiseiKinoko.SetActive(false);
        }

        RaycastHit hit;
        Physics.SphereCast(transform.position, 0.4f, Vector3.down, out hit, 2f, -1, QueryTriggerInteraction.Ignore);
        if (hit.collider != null)
            isOnGround = true;
        else isOnGround = false;

        reJumpTimer -= Time.deltaTime;

        if (transform.position.y < respawnHeight)
        {
            //y座標が0より下ならリスポーン
            Respawn();
        }

        if (isVineAtacking)
        {
            LookAt.transform.position = vineAtackStartPos;
            var rootPos = Grasses[1].transform.position + Vector3.up;
            //　ユニットの角度を変更
            vineAtackAngle += rotateSpeed * Time.deltaTime;
            //360を超えるなら回転終了
            if (vineAtackAngle > 360f)
            {
                vineAtackAngle = 0f;
                isVineAtacking = false;
                transform.position = vineAtackStartPos;
            }
            else
            {
                float eased = (vineAtackAngle / 360) * (vineAtackAngle / 360) * (3 - 2 * (vineAtackAngle / 360));
                //　ユニットの位置 = ターゲットの位置 ＋ ターゲットから見たユニットの角度 ×　ターゲットからの距離
                transform.position = rootPos
                    + Quaternion.Euler(0f, eased * 359f, 0f) * (vineAtackStartPos - rootPos);

                transform.rotation = Quaternion.Euler(0f, vineAtackAngle, 0f);
            }
        }
        else if (StoneCreatures.Count > 0)
        {
            LookAt.transform.position =
                new Vector3(Stones[StoneCreatures.Count - 1].transform.position.x,
                Stones[StoneCreatures.Count - 1].transform.position.y + 2,
                Stones[StoneCreatures.Count - 1].transform.position.z);
        }
        else LookAt.transform.position = new Vector3(MainBody.transform.position.x, MainBody.transform.position.y + 2, MainBody.transform.position.z);

        //草のばし中、移動範囲を超えないように座標を制限
        if (isVineExtending && !isVineAtacking)
        {
            var rootPos = Grasses[1].transform.position + Vector3.up;
            if ((rootPos - transform.position).sqrMagnitude
                > GRASS_LENGTH_MAX * GRASS_LENGTH_MAX)
            {
                transform.position = (rootPos + (transform.position - rootPos).normalized
                    * GRASS_LENGTH_MAX);
            }
        }
    }
    private void MoveMethod()
    {
        //通常時、カメラの見ている方向にプレイヤーを回転
        if (!isTurning)
        {
            // カメラの方向から、X-Z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            Vector3 moveForward = (cameraForward * _moveInputValue.y + Camera.main.transform.right * _moveInputValue.x);
            if (moveForward != Vector3.zero)
            {
                if (StoneCreatures.Count > 0 && isOnGround)
                {
                    stoneStepSE.isMoving = true;
                }
                else if (GrassCreatures.Count > 0 && isVineExtending == false && isOnGround)
                {
                    grassStepSE.isMoving = true;
                }
                else if (isOnGround)
                {
                    kinokoStepSE.isMoving = true;
                }

                if (isVineExtending)
                {
                    stoneStepSE.isMoving = false;
                    kinokoStepSE.isMoving = false;
                    grassStepSE.isMoving = false;
                }
            }
            //移動方向に力を加える
            rb.AddForce(moveForward * acc, ForceMode.Force);

            if (isVineExtending)
            {
                if (transform.position.y > Grasses[1].transform.position.y + 1.5f)
                    rb.AddForce(Vector3.down * acc / 2, ForceMode.Acceleration);
                if (transform.position.y < Grasses[1].transform.position.y + 0.5f)
                    rb.AddForce(Vector3.up * acc / 2, ForceMode.Acceleration);
            }

            if (moveForward != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }
        //投げ時、キャラは動かず回転する
        else
        {
            float throwAngleX = transform.eulerAngles.x - _moveInputValue.y * throwAngleSpeedX;
            float throwAngleY = transform.eulerAngles.y + _moveInputValue.x * throwAngleSpeedY;

            if (throwAngleX > 45f && throwAngleX < 180f)
                throwAngleX = 45f;

            if (throwAngleX < 315f && throwAngleX > 180f)
                throwAngleX = 315f;

            transform.rotation = Quaternion.Euler(throwAngleX, throwAngleY, 0);
        }
    }
    private void FixedUpdate()
    {
        stoneStepSE.isMoving = false;
        kinokoStepSE.isMoving = false;
        grassStepSE.isMoving = false;
        if (!isInSwamp() && !isVineAtacking && shrinkTimer <= 0f)
            MoveMethod();
        if (isVineAtacking)
            VineAtack();
    }
    void VineAtack()
    {
        float radius = 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            // 範囲内のオブジェクトに対して関数を実行
            // Monsterスクリプトがアタッチされているかを確認
            Monster monster = collider.GetComponent<Monster>();
            if (monster != null)
            {
                //Monsterスクリプトがアタッチされている場合、ふっとばす
                monster.FinishFight();
                monster.kaijo();
                monster.GetComponent<Rigidbody>().AddForce(
                Vector3.ProjectOnPlane((collider.transform.position - GrassCreatures[1].transform.position), Vector3.up).normalized
                * throwForce, ForceMode.Impulse);
            }
            if (collider.gameObject.CompareTag("Breakable"))
            {
                collider.gameObject.GetComponent<BreakableObject>().killMe();
            }
            GoalUnit goalUnit = collider.GetComponent<GoalUnit>();
            if (goalUnit != null)
            {
                goalUnit.Ring();
            }
        }
    }
    protected override void OnRelease(InputAction.CallbackContext context)
    {
        //草を持っている場合　草を解除
        if (GrassCreatures.Count > 0 && !isVineAtacking)
        {
            ClearAll();
            return;
        }
        //石を持っている場合　狙い中なら狙い解除、そうでなければ石を解除
        if (StoneCreatures.Count > 0)
        {
            if (isTurning)
            {
                isTurning = false;
                return;
            }
            ClearAll();
            return;
        }
        //どちらも持っていない場合　何も行わない
        return;
    }
    protected override void OnCall(InputAction.CallbackContext context)
    {
        //草を持っている場合　草を回す
        if (GrassCreatures.Count >= 1 && isVineExtending && !isVineAtacking)
        {
            GrassAtack();
            return;
        }
        //そうでなく2本持っている場合　何も行わない
        if (GrassCreatures.Count == 2)
        {
            PlaySE(GrassCallSE);
            return;
        }
        //ペアを作れていない場合、呼ぶ
        if (GrassCreatures.Count < 2 && GrassCreatures.Count > 0)
        {
            CollectGrasses();
            return;
        }
        //石を持っている場合　石をあつめる
        if (StoneCreatures.Count > 0)
        {
            CollectStones();
            return;
        }
        //どちらも持っていない場合　寄生を行う
        Parasite();
        return;
    }
    protected override void OnStoneUp(InputAction.CallbackContext context)
    {
        //石を持っている場合　石を交換
        if (StoneCreatures.Count > 0)
        {
            SwapHostStoneUp();
            return;
        }
        //どちらも持っていない場合　何も行わない
        return;
    }
    protected override void OnStoneDown(InputAction.CallbackContext context)
    {
        //石を持っている場合　石を交換
        if (StoneCreatures.Count > 0)
        {
            SwapHostStoneDown();
            return;
        }
        //どちらも持っていない場合　何も行わない
        return;
    }
    protected override void OnSkillStart(InputAction.CallbackContext context)
    {
        if (GrassCreatures.Count <= 1 && StoneCreatures.Count <= 1)
        {
            Jump();
            return;
        }
        if (StoneCreatures.Count > 0)
        {
            isTurning = true;
            return;
        }

        if (GrassCreatures.Count > 1 && !isVineExtending && !isVineAtacking && isOnGround)
        {
            ashimoto.transform.parent = null;
            isVineExtending = true;
            rb.useGravity = false;
            Grasses[1].transform.position = this.transform.position + Vector3.down * 0.8f;
            Grasses[1].transform.parent = transform.parent;
            transform.position = this.transform.position + Vector3.up * 0.2f;
            return;
        }
    }
    protected override void OnSkillCancel(InputAction.CallbackContext context)
    {
        if (StoneCreatures.Count > 0 && isTurning)
        {
            ThrowStone();
            isTurning = false;
            return;
        }

        if (GrassCreatures.Count > 0 && isVineExtending)
        {
            shrinkTimer = shrinkTime;
            return;
        }
    }
    void Parasite()
    {
        PlaySE(ParasiteSE);
        float radius = parasiteRadius;
        GameObject haniObject = Instantiate(Hani, transform.position, Quaternion.identity);
        haniObject.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            // 範囲内のオブジェクトに対して関数を実行
            // Monsterスクリプトがアタッチされているかを確認
            Monster monster = collider.GetComponent<Monster>();
            if (monster != null)
            {
                //Monsterスクリプトがアタッチされている場合、FinishCollect()を実行
                Instantiate(NokoriKinoko, transform.position, transform.rotation, null)
                    .GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.VelocityChange);
                transform.position = monster.transform.position;
                Instantiate(Kemuri);
                monster.FinishCollect();
                if (monster.IsStone())
                    PlaySE(StoneCallSE);
                else
                    PlaySE(GrassCallSE);
                return;
            }
        }
        //何も見つからなかった場合　おまけキノコを生成
        Instantiate(OmakeKinoko, transform.position + Vector3.up, transform.rotation, null)
            .GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.1f, 1.0f), 0.1f, Random.Range(0.1f, 1.0f)));
    }
    void SwapHostStoneUp()
    {
        if (StoneCreatures.Count != 0)
        {
            hostStone = (hostStone + 1) % (StoneCreatures.Count);
            MainBody.transform.localPosition = new Vector3(0, hostStone, 0);
        }
    }
    void SwapHostStoneDown()
    {
        if (StoneCreatures.Count != 0)
        {
            PlaySE(StoneRideSE);
            if (hostStone == 0)
                hostStone = StoneCreatures.Count - 1;
            else
                hostStone = (hostStone - 1) % (StoneCreatures.Count);
            MainBody.transform.localPosition = new Vector3(0, hostStone, 0);
        }
    }
    void Jump()
    {
        if (isInSwamp())
        {
            return;
        }
        if (reJumpTimer < 0f)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            reJumpTimer = reJumpTime;
        }
    }
    void ThrowStone()
    {
        PlaySE(StoneThrowSE);
        //寄生していないか、石を重ねていないとき　何も起こらない
        if (StoneCreatures.Count == 1 || StoneCreatures.Count == 0) return;

        // 投げる方向を計算
        Vector3 throwDirection = Quaternion.AngleAxis(-45, transform.right) * transform.forward;

        //自分が頂点の場合　自分を投げる
        if (hostStone == StoneCreatures.Count - 1)
        {
            int plpos = StoneCreatures.Count;
            OnlyHostStone();
            transform.position = Stones[plpos - 1].transform.position + transform.up;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);
            return;
        }

        //どちらでもない場合、頂点の石を投げる
        Stones[StoneCreatures.Count - 1].SetActive(false);

        int lastIndex = StoneCreatures.Count - 1;
        // プレファブを元にゲームオブジェクトを取得
        GameObject throwstone = StoneCreatures[lastIndex];
        Monster monster = throwstone.GetComponent<Monster>();
        monster.Drop();
        throwstone.transform.position = Stones[lastIndex].transform.position + transform.up;
        throwstone.transform.rotation = this.transform.rotation;
        monster.SetVelocity(rb.velocity);
        monster.Jump(throwDirection);
        StoneCreatures.RemoveAt(lastIndex);
    }
    void GrassAtack()
    {
        PlaySE(GrassAtackSE);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        vineAtackStartPos = transform.position;
        isVineAtacking = true;
        Invoke("grassAtackEffect", 0.45f);
    }
    void grassAtackEffect()
    {
        var rootPos = Grasses[1].transform.position + Vector3.up;
        float effectSize = (transform.position - rootPos).magnitude / 2;
        Instantiate(GrassAtackEffect, rootPos, transform.rotation, null).transform.localScale = new Vector3(effectSize, 1f, effectSize);
    }
    void CollectStones()
    {
        PlaySE(StoneCallSE);
        //寄生していないときはParasiteを行う
        if (StoneCreatures.Count == 0)
        {
            Parasite();
            return;
        }
        float radius = callRadius - 1f;

        GameObject callObject = Instantiate(CallRange, transform.position, Quaternion.identity);
        callObject.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            // 範囲内のオブジェクトに対して関数を実行
            // StoneControllerスクリプトがアタッチされているかを確認
            Monster stoneController = collider.GetComponent<Monster>();
            if (stoneController != null && stoneController.IsStone())
            {
                // StoneControllerスクリプトがアタッチされている場合、Collect()を実行
                stoneController.Collect();
            }
        }
    }
    private void OnlyHostStone()
    {
        hostStone = 0;
        MainBody.transform.localPosition = Vector3.zero;
        for (int i = 1; i < Stones.Length; i++)
        {
            Stones[i].SetActive(false);
        }
        while (StoneCreatures.Count > 1)
        {
            int lastIndex = StoneCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwstone = StoneCreatures[lastIndex];
            Monster monster = throwstone.GetComponent<Monster>();
            monster.Drop();
            throwstone.transform.position = Stones[lastIndex - 1].transform.position;
            throwstone.transform.rotation = this.transform.rotation;
            monster.kaijo();
            StoneCreatures.RemoveAt(lastIndex);
        }
    }
    private void ClearAll()
    {
        LookAt.transform.position = MainBody.transform.position;
        hostStone = 0;
        isTurning = false;
        isVineExtending = false;
        Grasses[1].transform.position = transform.position + Vector3.up;
        Grasses[1].transform.rotation = transform.rotation;
        Grasses[1].transform.parent = GrassesParent.transform;
        rb.useGravity = true;
        isVineAtacking = false;
        vineAtackAngle = 0f;
        ashimoto.transform.position = transform.position;
        ashimoto.transform.parent = this.transform;

        MainBody.transform.localPosition = Vector3.zero;
        int playerpon = Mathf.Max(StoneCreatures.Count, GrassCreatures.Count);
        for (int i = 0; i < Stones.Length; i++)
        {
            Stones[i].SetActive(false);
        }
        while (StoneCreatures.Count >= 1)
        {
            int lastIndex = StoneCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwstone = StoneCreatures[lastIndex];
            Monster monster = throwstone.GetComponent<Monster>();
            monster.Drop();
            throwstone.transform.position = Stones[StoneCreatures.Count - 1].transform.position;
            throwstone.transform.rotation = this.transform.rotation;
            StoneCreatures.RemoveAt(lastIndex);
            monster.kaijo();
        }
        StoneCreatures.Clear();

        for (int i = 0; i < Grasses.Length; i++)
        {
            Grasses[i].SetActive(false);
        }
        while (GrassCreatures.Count >= 1)
        {
            int lastIndex = GrassCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwgrass = GrassCreatures[lastIndex];
            Monster monster = throwgrass.GetComponent<Monster>();
            monster.Drop();
            throwgrass.transform.position = Grasses[GrassCreatures.Count - 1].transform.position;
            throwgrass.transform.rotation = this.transform.rotation;
            GrassCreatures.RemoveAt(lastIndex);
            monster.kaijo();
        }
        GrassCreatures.Clear();
        Vine.SetActive(false);
        transform.position += Vector3.up * playerpon;
    }
    public int addStone(GameObject gameObject, int num)
    {
        //草に寄生している場合　不成立
        if (GrassCreatures.Count != 0) return num;

        if (StoneCreatures.Count < STONES_MAX)
        {
            PlaySE(StoneRideSE);
            gameObject.GetComponent<Monster>().releaseAll();
            StoneCreatures.Add(gameObject);
            Instantiate(Kemuri, Stones[StoneCreatures.Count - 1].transform);
            Stones[StoneCreatures.Count - 1].SetActive(true);
            return num - 1;
        }
        return num;
    }
    public int addGrass(GameObject gameObject, int num)
    {
        //草に寄生している場合　不成立
        if (StoneCreatures.Count != 0) return num;

        if (GrassCreatures.Count < 2)
        {
            PlaySE(GrassRideSE);
            gameObject.GetComponent<Monster>().releaseAll();
            GrassCreatures.Add(gameObject);
            Instantiate(Kemuri, Grasses[GrassCreatures.Count - 1].transform);
            Grasses[GrassCreatures.Count - 1].SetActive(true);
            if (GrassCreatures.Count == 2)
                Vine.SetActive(true);
            return num - 1;
        }
        return num;
    }
    void CollectGrasses()
    {
        //寄生していないときはParasiteを行う
        if (GrassCreatures.Count == 0)
        {
            Parasite();
            return;
        }
        float radius = callRadius;
        PlaySE(GrassCallSE);

        GameObject callObject = Instantiate(CallRange, transform.position, Quaternion.identity);
        callObject.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            // 範囲内のオブジェクトに対して関数を実行
            // Monsterスクリプトがアタッチされており、草であるかを確認
            Monster grassController = collider.GetComponent<Monster>();
            if (grassController != null && !grassController.IsStone())
            {
                // StoneControllerスクリプトがアタッチされている場合、Collect()を実行
                grassController.Collect();
            }
        }
    }
    public int getStonesCount()
    {
        return StoneCreatures.Count;
    }
    public int getGrassesCount()
    {
        return GrassCreatures.Count;
    }
    public bool isInStoneSkill()
    {
        return isTurning;
    }

    public bool isInGrassSkill()
    {
        return isVineExtending;
    }

    public void Respawn()
    {
        ClearAll();
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        transform.position = respawnPos;
        foreach (Monster monster in monsters)
        {
            monster.Respawn();
        }
    }
    bool isInSwamp()
    {
        //石が沼に入ったかどうか
        if (getStonesCount() > 0)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 0.5f, 1 << 3))
            {
                return true;
            }
        }
        return false;
    }
    public bool getRinging()
    {
        if (StoneCreatures.Count > 0)
            return true;
        if (GrassCreatures.Count > 0 && isVineExtending == true)
            return true;
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            respawnPos = other.transform.position;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        if ((isVineAtacking || (StoneCreatures.Count > 0 && !isOnGround)) && collision.gameObject.CompareTag("Breakable"))
        {
            collision.gameObject.GetComponent<BreakableObject>().killMe();
        }
    }

    public void PlaySE(List<AudioClip> SE)
    {
        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(SE[Random.Range(0, SE.Count)]);
    }
}