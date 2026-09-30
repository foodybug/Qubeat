using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 레벨 디자인 테스트용 자동 플레이어 컴포넌트.
/// Player 오브젝트에 AddComponent하여 사용. Player.cs 수정 없음.
///
/// Ghost를 제외한 모든 Enemy를 레벨 비교로 일괄 처리한다.
///   - Ghost                            →  무조건 위험 대상 (레벨 무관 회피)
///   - player.realLevel >= enemy.Level  →  캐치 대상 (접근)
///   - player.realLevel <  enemy.Level  →  위험 대상 (회피)
///
/// 우선순위:
///   1. DangerRange 안에 위험 적 존재  →  회피 (밀어내는 방향 벡터 합산)
///   2. CatchRange  안에 캐치 가능 적  →  가장 가까운 대상에 접근
///   3. 아무것도 없음                  →  스테이지 내 랜덤 배회
/// </summary>
public class DebugPlayer : MonoBehaviour
{
    #region - inspector params -
    [Header("== Debug Player AI ==")]
    [Tooltip("AI 활성 여부. false 시 완전 비활성.")]
    [SerializeField] bool m_Active = true;

    [Header("- Level Override -")]
    [Tooltip("AI가 판단 기준으로 사용할 플레이어 레벨.\n0 = player.realLevel 그대로 사용\n1 이상 = 지정 레벨로 오버라이드 (실제 Player 레벨과 무관)")]
    [SerializeField] int m_DebugLevel = 0;

    [Header("- Detect Range -")]
    [Tooltip("캐치 가능 적을 탐색할 반경")]
    [SerializeField] float m_CatchRange  = 15f;
    [Tooltip("위험 적을 감지할 반경 (이 안에 들어오면 회피 최우선)")]
    [SerializeField] float m_DangerRange = 8f;

    [Header("- Movement -")]
    [Tooltip("회피 시 도망칠 목표 거리")]
    [SerializeField] float m_AvoidDistance = 12f;
    [Tooltip("AI 판단 갱신 주기 (초)")]
    [SerializeField] float m_RefreshRate = 0.3f;

    [Header("- Debug -")]
    [Tooltip("Scene 뷰에서 탐색 범위 Gizmo 표시")]
    [SerializeField] bool m_ShowGizmo = true;
    [Tooltip("현재 AI 상태 (읽기 전용)")]
    [SerializeField] string m_StatusLog = "Idle";
    #endregion

    #region - internal -
    enum eAIState { Idle, Catch, Avoid }
    eAIState m_AIState = eAIState.Idle;

    Player   m_Player;
    Coroutine m_AICoroutine;
    #endregion

    #region - lifecycle -
    void Start()
    {
        m_Player = GetComponent<Player>();
        if (m_Player == null)
        {
            Debug.LogError("[DebugPlayer] Player 컴포넌트를 찾을 수 없습니다. 같은 GameObject에 추가해 주세요.");
            enabled = false;
            return;
        }

        if (m_Active)
            StartAI();
    }

    void OnEnable()
    {
        if (m_Player != null && m_Active)
            StartAI();
    }

    void OnDisable()
    {
        StopAI();
    }

    void OnValidate()
    {
        // Inspector에서 m_Active 토글 시 런타임 중 즉시 반영
        if (!Application.isPlaying) return;

        if (m_Active)
            StartAI();
        else
            StopAI();
    }
    #endregion

    #region - AI control -
    void StartAI()
    {
        if (m_AICoroutine != null)
            StopCoroutine(m_AICoroutine);
        m_AICoroutine = StartCoroutine(AI_Loop());
    }

    void StopAI()
    {
        if (m_AICoroutine != null)
        {
            StopCoroutine(m_AICoroutine);
            m_AICoroutine = null;
        }
        m_StatusLog = "Stopped";
    }

    IEnumerator AI_Loop()
    {
        // EntityManager / Player가 준비될 때까지 대기
        while (YEntityManager.Instance == null || YEntityManager.Instance.PlayerEntity == null)
            yield return null;

        while (m_Active && m_Player != null && m_Player.Living)
        {
            Decide();
            yield return new WaitForSeconds(m_RefreshRate);
        }

        m_StatusLog = "Idle (player dead or disabled)";
    }

    /// <summary>
    /// 매 RefreshRate마다 호출.
    /// Ghost를 제외한 모든 Enemy를 스캔해 캐치/회피 목적지를 계산하고
    /// Msg_Input_Move 로 Player에게 주입한다.
    /// </summary>
    void Decide()
    {
        if (YEntityManager.Instance == null) return;
        if (YStageManager.Instance   == null) return;

        // m_DebugLevel이 1 이상이면 오버라이드, 0이면 실제 레벨 사용
        int   playerLevel = (m_DebugLevel >= 1) ? m_DebugLevel : m_Player.realLevel;
        Vector3 myPos     = transform.position;

        // ---- 적 분류 ----
        YBaseEntity nearestCatch   = null;
        float       nearestCatchSq = float.MaxValue;

        Vector3 avoidForce = Vector3.zero;
        bool    hasDanger  = false;

        foreach (KeyValuePair<int, YBaseEntity> pair in YEntityManager.Instance.dicEntity)
        {
            YBaseEntity entity = pair.Value;

            // 자기 자신 / null / 죽은 엔티티 제외
            if (entity == null)       continue;
            if (entity == m_Player)   continue;
            if (entity is Player)     continue;
            if (!entity.Living)       continue;

            float sqDist = (entity.transform.position - myPos).sqrMagnitude;

            // Ghost는 레벨 무관 무조건 위험 대상
            bool isCatchable = !(entity is Ghost) && playerLevel >= entity.Level;

            if (isCatchable)
            {
                // CatchRange 안의 가장 가까운 캐치 대상
                if (sqDist < m_CatchRange * m_CatchRange && sqDist < nearestCatchSq)
                {
                    nearestCatchSq = sqDist;
                    nearestCatch   = entity;
                }
            }
            else
            {
                // DangerRange 안의 위험 적 → 밀어내는 방향 누적
                if (sqDist < m_DangerRange * m_DangerRange)
                {
                    hasDanger = true;
                    Vector3 pushDir = myPos - entity.transform.position;
                    // 가까울수록 강하게 (거리 역비례 가중치)
                    float weight = 1f / (pushDir.magnitude + 0.01f);
                    avoidForce  += pushDir.normalized * weight;
                }
            }
        }

        // ---- 목표 결정 ----
        Vector3 dest;

        if (hasDanger)
        {
            // 1순위: 회피
            m_AIState = eAIState.Avoid;
            Vector3 escapeDir = (avoidForce.sqrMagnitude > 0f)
                ? avoidForce.normalized
                : -transform.forward;   // fallback: 현재 방향 반대

            dest = ClampToStage(myPos + escapeDir * m_AvoidDistance);
            m_StatusLog = "Avoiding";
        }
        else if (nearestCatch != null)
        {
            // 2순위: 캐치
            m_AIState   = eAIState.Catch;
            dest        = nearestCatch.transform.position;
            m_StatusLog = string.Format("Catching lv.{0} ({1})", nearestCatch.Level, nearestCatch.GetType().Name);
        }
        else
        {
            // 3순위: 랜덤 배회
            m_AIState   = eAIState.Idle;
            dest        = YStageManager.Instance.GetRandomPlane2DPosInStage();
            m_StatusLog = "Wandering";
        }

        // Player 이동 명령 주입
        m_Player.HandleMessage(new Msg_Input_Move(dest));
    }

    /// <summary>스테이지 경계 안쪽으로 좌표를 클램핑한다.</summary>
    Vector3 ClampToStage(Vector3 pos)
    {
        if (YStageManager.Instance == null) return pos;

        Vector2 size   = YStageManager.Instance.StageSize;
        const float margin = 1f;

        pos.x = Mathf.Clamp(pos.x, -size.x + margin, size.x - margin);
        pos.z = Mathf.Clamp(pos.z, -size.y + margin, size.y - margin);
        return pos;
    }
    #endregion

    #region - gizmo -
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!m_ShowGizmo) return;

        Vector3 pos = transform.position;

        // 캐치 범위 — 초록 와이어
        Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
        Gizmos.DrawWireSphere(pos, m_CatchRange);

        // 위험 범위 — 빨강 와이어
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(pos, m_DangerRange);

        // 현재 AI 상태 마커
        switch (m_AIState)
        {
            case eAIState.Catch:  Gizmos.color = Color.green;  break;
            case eAIState.Avoid:  Gizmos.color = Color.red;    break;
            default:              Gizmos.color = Color.gray;   break;
        }
        Gizmos.DrawSphere(pos + Vector3.up * 0.6f, 0.25f);
    }
#endif
    #endregion
}
