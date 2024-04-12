using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : PlayerMover
{
    //石に寄生する機能関連
    public const int MAX_STONES = 6;
    int currentStoneIndex = 0;
    [SerializeField] GameObject[] stoneObjects = new GameObject[MAX_STONES];
    List<LineRenderer> trajectoryLines = new List<LineRenderer>();
    List<GameObject> stoneCreatures = new List<GameObject>();
    public float throwForce = 20.0f;

    //草に寄生する機能関連
    public const int MAX_GRASSES = 2;
    [SerializeField] GameObject[] grassObjects = new GameObject[MAX_GRASSES];
    [SerializeField] GameObject vineObject;
    [SerializeField] GameObject grassParentObject;
    List<GameObject> grassCreatures = new List<GameObject>();
    [SerializeField] const float MAX_GRASS_LENGTH = 12f;
    bool isVineExtending = false;
    bool isVineAtacking = false;
    Vector3 grassLocalPosition = Vector3.zero;
    float currentVineAtackAngle = 0f;
    [SerializeField] private float rotateSpeed = 360f;
    private Vector3 vineAtackStartPosition = Vector3.forward;
    const float ShrinkTime = 0.2f;
    float shrinkTimer = -1f;
    [SerializeField] float ShrinkPower = 20f;
    [SerializeField] GameObject grassAtackEffectPrefab;

    //キノコ本体関連
    [SerializeField] const float CALL_RADIUS = 5f;
    [SerializeField] const float PARASITE_RADIUS = 2f;
    [SerializeField] const float ACCERALATION = 30f;
    [SerializeField] const float JUMP_POWER = 6.25f;
    [SerializeField] const float REJUMP_TIME = 0.5f;
    float reJumpTimer = 0f;
    [SerializeField] GameObject mainBody = default!;
    [SerializeField] const float KINOKO_MASS = 1.0f;
    [SerializeField] const float STONE_MASS = 1.25f;
    [SerializeField] const float GRASS_MASS = 0.75f;
    [SerializeField] float respawnHeight = 0f;
    Rigidbody rb;
    bool isOnGround = false;
    bool isAiming;
    [SerializeField] GameObject ParasiteRangeEffect;
    [SerializeField] GameObject CallRangeEffect;
    [SerializeField] GameObject Smoke;

    //カメラ入力関連
    [SerializeField] float throwAngleSpeedX = 2.0f;
    [SerializeField] float throwAngleSpeedY = 2.0f;
    [SerializeField] InputActionReference XYAxisInput;
    [SerializeField] GameObject PlayerHead;
    [SerializeField] GameObject PlayerFoot;

    //表示するキノコ
    [SerializeField] GameObject[] ArukiKinokoObject = default!;
    [SerializeField] GameObject KiseiKinokoObject = default!;
    [SerializeField] GameObject NokoriKinokoObject = default!;
    [SerializeField] GameObject OmakeKinokoObject = default!;

    //リスポーン関連
    Vector3 respawnPos;
    List<Monster> monsters = new List<Monster>();

    //音関連
    [SerializeField] StepSE kinokoStepSE;
    [SerializeField] StepSE grassStepSE;
    [SerializeField] StepSE stoneStepSE;
    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource audioSource;
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
        audioSource = GetComponents<AudioSource>()[0];

        respawnPos = transform.position;
        rb = GetComponent<Rigidbody>();
        ClearAll();
        grassLocalPosition = grassObjects[1].transform.localPosition;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 10000000);
        foreach (Collider collider in colliders)
        {
            Monster monster = collider.GetComponent<Monster>();
            if (monster != null)
            {
                monsters.Add(monster);
            }
        }
        foreach (var stone in stoneObjects)
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
        if (isAiming)
        {
            stoneObjects[stoneCreatures.Count - 1].GetComponent<LineRenderer>().enabled = true;
        }

        if (stoneCreatures.Count > 0)
        {
            rb.mass = STONE_MASS;
        }
        else if (grassCreatures.Count > 0)
        {
            rb.mass = GRASS_MASS;
        }
        else rb.mass = KINOKO_MASS;

        if (shrinkTimer > 0f && !isVineAtacking)
        {
            shrinkTimer -= Time.deltaTime;

            if (!isOnGround)
                rb.AddForce((grassObjects[1].transform.position + Vector3.up - transform.position) * Time.deltaTime / ShrinkTime * ShrinkPower, ForceMode.VelocityChange);

            if (shrinkTimer <= 0f)
            {
                isVineExtending = false;
                if (!isOnGround)
                {
                    transform.position = grassObjects[1].transform.position + Vector3.up;
                    rb.velocity = Vector3.zero;
                }
                grassObjects[1].transform.rotation = this.transform.rotation;
                grassObjects[1].transform.parent = grassParentObject.transform;
                grassObjects[1].transform.localPosition = grassLocalPosition;
                rb.useGravity = true;
                PlayerFoot.transform.parent = this.transform;
                PlayerFoot.transform.localPosition = Vector3.zero;
            }
            return;
        }

        if (stoneCreatures.Count > 0)
        {
            foreach (var kino in ArukiKinokoObject)
                kino.SetActive(false);
            KiseiKinokoObject.SetActive(true);
        }
        else if (grassCreatures.Count > 0)
        {
            foreach (var kino in ArukiKinokoObject)
                kino.SetActive(false);
        }
        else
        {
            foreach (var kino in ArukiKinokoObject)
                kino.SetActive(true);
            KiseiKinokoObject.SetActive(false);
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
            PlayerHead.transform.position = vineAtackStartPosition;
            var rootPos = grassObjects[1].transform.position + Vector3.up;
            //　ユニットの角度を変更
            currentVineAtackAngle += rotateSpeed * Time.deltaTime;
            //360を超えるなら回転終了
            if (currentVineAtackAngle > 360f)
            {
                currentVineAtackAngle = 0f;
                isVineAtacking = false;
                transform.position = vineAtackStartPosition;
            }
            else
            {
                float eased = (currentVineAtackAngle / 360) * (currentVineAtackAngle / 360) * (3 - 2 * (currentVineAtackAngle / 360));
                //　ユニットの位置 = ターゲットの位置 ＋ ターゲットから見たユニットの角度 ×　ターゲットからの距離
                transform.position = rootPos
                    + Quaternion.Euler(0f, eased * 359f, 0f) * (vineAtackStartPosition - rootPos);

                transform.rotation = Quaternion.Euler(0f, currentVineAtackAngle, 0f);
            }
        }
        else if (stoneCreatures.Count > 0)
        {
            PlayerHead.transform.position =
                new Vector3(stoneObjects[stoneCreatures.Count - 1].transform.position.x,
                stoneObjects[stoneCreatures.Count - 1].transform.position.y + 2,
                stoneObjects[stoneCreatures.Count - 1].transform.position.z);
        }
        else PlayerHead.transform.position = new Vector3(mainBody.transform.position.x, mainBody.transform.position.y + 2, mainBody.transform.position.z);

        //草のばし中、移動範囲を超えないように座標を制限
        if (isVineExtending && !isVineAtacking)
        {
            var rootPos = grassObjects[1].transform.position + Vector3.up;
            if ((rootPos - transform.position).sqrMagnitude
                > MAX_GRASS_LENGTH * MAX_GRASS_LENGTH)
            {
                transform.position = (rootPos + (transform.position - rootPos).normalized
                    * MAX_GRASS_LENGTH);
            }
        }
    }
    private void MoveMethod()
    {
        //通常時、カメラの見ている方向にプレイヤーを回転
        if (!isAiming)
        {
            // カメラの方向から、X-Z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            Vector3 moveForward = (cameraForward * _moveInputValue.y + Camera.main.transform.right * _moveInputValue.x);
            if (moveForward != Vector3.zero)
            {
                if (stoneCreatures.Count > 0 && isOnGround)
                {
                    stoneStepSE.isMoving = true;
                }
                else if (grassCreatures.Count > 0 && isVineExtending == false && isOnGround)
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
            rb.AddForce(moveForward * ACCERALATION, ForceMode.Force);

            if (isVineExtending)
            {
                if (transform.position.y > grassObjects[1].transform.position.y + 1.5f)
                    rb.AddForce(Vector3.down * ACCERALATION / 2, ForceMode.Acceleration);
                if (transform.position.y < grassObjects[1].transform.position.y + 0.5f)
                    rb.AddForce(Vector3.up * ACCERALATION / 2, ForceMode.Acceleration);
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
                Vector3.ProjectOnPlane((collider.transform.position - grassCreatures[1].transform.position), Vector3.up).normalized
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
        if (grassCreatures.Count > 0 && !isVineAtacking)
        {
            ClearAll();
            return;
        }
        //石を持っている場合　狙い中なら狙い解除、そうでなければ石を解除
        if (stoneCreatures.Count > 0)
        {
            if (isAiming)
            {
                isAiming = false;
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
        if (grassCreatures.Count >= 1 && isVineExtending && !isVineAtacking)
        {
            GrassAtack();
            return;
        }
        //そうでなく2本持っている場合　何も行わない
        if (grassCreatures.Count == 2)
        {
            PlaySE(GrassCallSE);
            return;
        }
        //ペアを作れていない場合、呼ぶ
        if (grassCreatures.Count < 2 && grassCreatures.Count > 0)
        {
            CollectGrasses();
            return;
        }
        //石を持っている場合　石をあつめる
        if (stoneCreatures.Count > 0)
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
        if (stoneCreatures.Count > 0)
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
        if (stoneCreatures.Count > 0)
        {
            SwapHostStoneDown();
            return;
        }
        //どちらも持っていない場合　何も行わない
        return;
    }
    protected override void OnSkillStart(InputAction.CallbackContext context)
    {
        if (grassCreatures.Count <= 1 && stoneCreatures.Count <= 1)
        {
            Jump();
            return;
        }
        if (stoneCreatures.Count > 0)
        {
            isAiming = true;
            return;
        }

        if (grassCreatures.Count > 1 && !isVineExtending && !isVineAtacking && isOnGround)
        {
            PlayerFoot.transform.parent = null;
            isVineExtending = true;
            rb.useGravity = false;
            grassObjects[1].transform.position = this.transform.position + Vector3.down * 0.8f;
            grassObjects[1].transform.parent = transform.parent;
            transform.position = this.transform.position + Vector3.up * 0.2f;
            return;
        }
    }
    protected override void OnSkillCancel(InputAction.CallbackContext context)
    {
        if (stoneCreatures.Count > 0 && isAiming)
        {
            ThrowStone();
            isAiming = false;
            return;
        }

        if (grassCreatures.Count > 0 && isVineExtending)
        {
            shrinkTimer = ShrinkTime;
            return;
        }
    }
    void Parasite()
    {
        PlaySE(ParasiteSE);
        float radius = PARASITE_RADIUS;
        GameObject haniObject = Instantiate(ParasiteRangeEffect, transform.position, Quaternion.identity);
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
                Instantiate(NokoriKinokoObject, transform.position, transform.rotation, null)
                    .GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.VelocityChange);
                transform.position = monster.transform.position;
                Instantiate(Smoke);
                monster.FinishCollect();
                if (monster.IsStone())
                    PlaySE(StoneCallSE);
                else
                    PlaySE(GrassCallSE);
                return;
            }
        }
        //何も見つからなかった場合　おまけキノコを生成
        Instantiate(OmakeKinokoObject, transform.position + Vector3.up, transform.rotation, null)
            .GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.1f, 1.0f), 0.1f, Random.Range(0.1f, 1.0f)));
        Debug.Log("Parasite Failed");
    }
    void SwapHostStoneUp()
    {
        if (stoneCreatures.Count != 0)
        {
            currentStoneIndex = (currentStoneIndex + 1) % (stoneCreatures.Count);
            mainBody.transform.localPosition = new Vector3(0, currentStoneIndex, 0);
        }
    }
    void SwapHostStoneDown()
    {
        if (stoneCreatures.Count != 0)
        {
            PlaySE(StoneRideSE);
            if (currentStoneIndex == 0)
                currentStoneIndex = stoneCreatures.Count - 1;
            else
                currentStoneIndex = (currentStoneIndex - 1) % (stoneCreatures.Count);
            mainBody.transform.localPosition = new Vector3(0, currentStoneIndex, 0);
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
            rb.AddForce(Vector3.up * JUMP_POWER, ForceMode.Impulse);
            reJumpTimer = REJUMP_TIME;
        }
    }
    void ThrowStone()
    {
        PlaySE(StoneThrowSE);
        //寄生していないか、石を重ねていないとき　何も起こらない
        if (stoneCreatures.Count == 1 || stoneCreatures.Count == 0) return;

        // 投げる方向を計算
        Vector3 throwDirection = Quaternion.AngleAxis(-45, transform.right) * transform.forward;

        //自分が頂点の場合　自分を投げる
        if (currentStoneIndex == stoneCreatures.Count - 1)
        {
            int plpos = stoneCreatures.Count;
            OnlyHostStone();
            transform.position = stoneObjects[plpos - 1].transform.position + transform.up;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);
            return;
        }

        //どちらでもない場合、頂点の石を投げる
        stoneObjects[stoneCreatures.Count - 1].SetActive(false);

        int lastIndex = stoneCreatures.Count - 1;
        // プレファブを元にゲームオブジェクトを取得
        GameObject throwstone = stoneCreatures[lastIndex];
        Monster monster = throwstone.GetComponent<Monster>();
        monster.Drop();
        throwstone.transform.position = stoneObjects[lastIndex].transform.position + transform.up;
        throwstone.transform.rotation = this.transform.rotation;
        monster.SetVelocity(rb.velocity);
        monster.Jump(throwDirection);
        stoneCreatures.RemoveAt(lastIndex);
    }
    void GrassAtack()
    {
        PlaySE(GrassAtackSE);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        vineAtackStartPosition = transform.position;
        isVineAtacking = true;
        Invoke("grassAtackEffect", 0.45f);
    }
    void grassAtackEffect()
    {
        var rootPos = grassObjects[1].transform.position + Vector3.up;
        float effectSize = (transform.position - rootPos).magnitude / 2;
        Instantiate(grassAtackEffectPrefab, rootPos, transform.rotation, null).transform.localScale = new Vector3(effectSize, 1f, effectSize);
    }
    void CollectStones()
    {
        PlaySE(StoneCallSE);
        //寄生していないときはParasiteを行う
        if (stoneCreatures.Count == 0)
        {
            Parasite();
            return;
        }
        float radius = CALL_RADIUS - 1f;

        GameObject callObject = Instantiate(CallRangeEffect, transform.position, Quaternion.identity);
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
        currentStoneIndex = 0;
        mainBody.transform.localPosition = Vector3.zero;
        for (int i = 1; i < stoneObjects.Length; i++)
        {
            stoneObjects[i].SetActive(false);
        }
        while (stoneCreatures.Count > 1)
        {
            int lastIndex = stoneCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwstone = stoneCreatures[lastIndex];
            Monster monster = throwstone.GetComponent<Monster>();
            monster.Drop();
            throwstone.transform.position = stoneObjects[lastIndex - 1].transform.position;
            throwstone.transform.rotation = this.transform.rotation;
            monster.kaijo();
            stoneCreatures.RemoveAt(lastIndex);
        }
    }
    private void ClearAll()
    {
        PlayerHead.transform.position = mainBody.transform.position;
        currentStoneIndex = 0;
        isAiming = false;
        isVineExtending = false;
        grassObjects[1].transform.position = transform.position + Vector3.up;
        grassObjects[1].transform.rotation = transform.rotation;
        grassObjects[1].transform.parent = grassParentObject.transform;
        rb.useGravity = true;
        isVineAtacking = false;
        currentVineAtackAngle = 0f;
        PlayerFoot.transform.position = transform.position;
        PlayerFoot.transform.parent = this.transform;

        mainBody.transform.localPosition = Vector3.zero;
        int playerpon = Mathf.Max(stoneCreatures.Count, grassCreatures.Count);
        for (int i = 0; i < stoneObjects.Length; i++)
        {
            stoneObjects[i].SetActive(false);
        }
        while (stoneCreatures.Count >= 1)
        {
            int lastIndex = stoneCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwstone = stoneCreatures[lastIndex];
            Monster monster = throwstone.GetComponent<Monster>();
            monster.Drop();
            throwstone.transform.position = stoneObjects[stoneCreatures.Count - 1].transform.position;
            throwstone.transform.rotation = this.transform.rotation;
            stoneCreatures.RemoveAt(lastIndex);
            monster.kaijo();
        }
        stoneCreatures.Clear();

        for (int i = 0; i < grassObjects.Length; i++)
        {
            grassObjects[i].SetActive(false);
        }
        while (grassCreatures.Count >= 1)
        {
            int lastIndex = grassCreatures.Count - 1;
            // プレファブを元にゲームオブジェクトを取得
            GameObject throwgrass = grassCreatures[lastIndex];
            Monster monster = throwgrass.GetComponent<Monster>();
            monster.Drop();
            throwgrass.transform.position = grassObjects[grassCreatures.Count - 1].transform.position;
            throwgrass.transform.rotation = this.transform.rotation;
            grassCreatures.RemoveAt(lastIndex);
            monster.kaijo();
        }
        grassCreatures.Clear();
        vineObject.SetActive(false);
        transform.position += Vector3.up * playerpon;
    }
    public int addStone(GameObject gameObject, int num)
    {
        //草に寄生している場合　不成立
        if (grassCreatures.Count != 0) return num;

        if (stoneCreatures.Count < MAX_STONES)
        {
            PlaySE(StoneRideSE);
            gameObject.GetComponent<Monster>().releaseAll();
            stoneCreatures.Add(gameObject);
            Instantiate(Smoke, stoneObjects[stoneCreatures.Count - 1].transform);
            stoneObjects[stoneCreatures.Count - 1].SetActive(true);
            return num - 1;
        }
        return num;
    }
    public int addGrass(GameObject gameObject, int num)
    {
        //草に寄生している場合　不成立
        if (stoneCreatures.Count != 0) return num;

        if (grassCreatures.Count < 2)
        {
            PlaySE(GrassRideSE);
            gameObject.GetComponent<Monster>().releaseAll();
            grassCreatures.Add(gameObject);
            Instantiate(Smoke, grassObjects[grassCreatures.Count - 1].transform);
            grassObjects[grassCreatures.Count - 1].SetActive(true);
            if (grassCreatures.Count == 2)
                vineObject.SetActive(true);
            return num - 1;
        }
        return num;
    }
    void CollectGrasses()
    {
        //寄生していないときはParasiteを行う
        if (grassCreatures.Count == 0)
        {
            Parasite();
            return;
        }
        float radius = CALL_RADIUS;
        PlaySE(GrassCallSE);

        GameObject callObject = Instantiate(CallRangeEffect, transform.position, Quaternion.identity);
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
        return stoneCreatures.Count;
    }
    public int getGrassesCount()
    {
        return grassCreatures.Count;
    }
    public bool isInStoneSkill()
    {
        return isAiming;
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
        if (stoneCreatures.Count > 0)
            return true;
        if (grassCreatures.Count > 0 && isVineExtending == true)
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

        if ((isVineAtacking || (stoneCreatures.Count > 0 && !isOnGround)) && collision.gameObject.CompareTag("Breakable"))
        {
            collision.gameObject.GetComponent<BreakableObject>().killMe();
        }
    }

    public void PlaySE(List<AudioClip> SE)
    {
        audioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(SE[Random.Range(0, SE.Count)]);
    }
}