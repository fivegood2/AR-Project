using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Spatial CAPTCHA 전체 흐름 관리.
/// Intro → Navigate → AtAhaPoint → Answering → Result
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public enum Phase { Intro, Navigate, AtAhaPoint, Answering, Result }

    [Header("Phase (Read-Only)")]
    [SerializeField] private Phase _phase = Phase.Intro;

    // ── Scene Objects ──────────────────────────────────────────────
    [Header("Key Objects")]
    public GameObject keyMesh;
    public GameObject keyFloorMarker;
    public GameObject keyView;

    [Header("Heart Objects")]
    public GameObject heartMesh;
    public GameObject heartFloorMarker;
    public GameObject heartView;

    [Header("Start Button")]
    public GazeButton startButton;

    [Header("UI Panels")]
    public GameObject introPanel;
    public GameObject navigatePanel;
    public GameObject atAhaPanel;
    public GameObject answerPanel;
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Answer Buttons")]
    public GazeButton btnKey;
    public GazeButton btnHeart;
    public GazeButton btnChair;
    public GazeButton btnDoll;

    [Header("Settings")]
    public float atAhaToAnswerDelay = 10f;
    public float failRetryDelay     = 3f;
    public float answerTimeLimit    = 30f;

    [Header("Timer Text")]
    public TMP_Text timerText;

    // ── State ──────────────────────────────────────────────────────
    private string          _selectedShape;
    private AhaPointTrigger _activeTrigger;
    private float           _answerTimer;
    private Vector3         _resultPanelPos;
    private Quaternion      _resultPanelRot = Quaternion.identity;

    // ── Lifecycle ──────────────────────────────────────────────────
    void Start()
    {
        // 모든 메시/마커 비활성화
        SetGO(keyMesh,          false);
        SetGO(keyFloorMarker,   false);
        SetGO(keyView,          false);
        SetGO(heartMesh,        false);
        SetGO(heartFloorMarker, false);
        SetGO(heartView,        false);

        ShowOnly(introPanel);
        _phase = Phase.Intro;

        // START 버튼 이벤트 연결 (런타임)
        if (startButton != null)
            startButton.OnGazeConfirmed.AddListener(OnStartPressed);
        else
        {
            var gbs = Resources.FindObjectsOfTypeAll<GazeButton>();
            foreach (var gb in gbs)
                if (gb.name == "GazeButton_Start") { gb.OnGazeConfirmed.AddListener(OnStartPressed); break; }
        }

        Debug.Log("[GameFlow] Phase: Intro");
    }

    void Update()
    {
        if (_phase != Phase.Answering) return;
        _answerTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(Mathf.Max(0, _answerTimer)).ToString();
        if (_answerTimer <= 0f) OnTimerExpired();
    }

    // ── Phase Transitions ──────────────────────────────────────────

    public void OnStartPressed()
    {
        if (_phase != Phase.Intro) return;
        StartNavigate();
    }

    void StartNavigate()
    {
        // Key / Heart 랜덤 선택
        _selectedShape = (Random.value < 0.5f) ? "Key" : "Heart";
        Debug.Log("[GameFlow] Selected: " + _selectedShape);

        if (_selectedShape == "Key")
        {
            SetGO(keyMesh, true); SetGO(keyFloorMarker, true); SetGO(keyView, true);
            _activeTrigger = keyFloorMarker?.GetComponent<AhaPointTrigger>();
        }
        else
        {
            SetGO(heartMesh, true); SetGO(heartFloorMarker, true); SetGO(heartView, true);
            _activeTrigger = heartFloorMarker?.GetComponent<AhaPointTrigger>();
        }

        if (_activeTrigger != null)
            _activeTrigger.onEntered.AddListener(OnAhaPointReached);
        else
            Debug.LogError("[GameFlow] AhaPointTrigger not found!");

        _phase = Phase.Navigate;
        ShowOnly(navigatePanel);
        Debug.Log("[GameFlow] Phase: Navigate");
    }

    public void OnAhaPointReached()
    {
        if (_phase != Phase.Navigate) return;
        _phase = Phase.AtAhaPoint;
        ShowOnly(atAhaPanel);
        Debug.Log("[GameFlow] Phase: AtAhaPoint — waiting " + atAhaToAnswerDelay + "s");
        StartCoroutine(ShowAnswerAfterDelay());
    }

    IEnumerator ShowAnswerAfterDelay()
    {
        yield return new WaitForSeconds(atAhaToAnswerDelay);
        PositionAnswerPanel();
        StartAnswering();
    }

void PositionAnswerPanel()
    {
        if (answerPanel == null) return;

        // 활성 메시 오른쪽에 AnswerPanel 배치
        GameObject activeMesh = (_selectedShape == "Key") ? keyMesh : heartMesh;
        GameObject activeView = (_selectedShape == "Key") ? keyView  : heartView;
        if (activeMesh == null || activeView == null) return;

        Vector3 meshPos = activeMesh.transform.position;
        Vector3 viewPos = activeView.transform.position;
        Vector3 fwd     = (meshPos - viewPos).normalized;
        if (fwd == Vector3.zero) fwd = Vector3.forward;

        Vector3 right    = Vector3.Cross(Vector3.up, fwd).normalized;
        Vector3 panelPos = meshPos + right * 0.9f;
        panelPos.y = viewPos.y;

        Vector3 look = viewPos - panelPos; look.y = 0;
        Quaternion panelRot = Quaternion.identity;
        // World-space Canvas의 '정면(읽히는 방향)'은 로컬 -Z 쪽이다.
        // 따라서 -Z가 시청자(viewPos) 쪽을 향하도록 -look 방향으로 LookRotation을 적용해야
        // 사용자가 정면(똑바로 보이는 면)을 보게 된다. (look 그대로 쓰면 뒷면이 보여 뒤집혀 보인다)
        if (look != Vector3.zero)
            panelRot = Quaternion.LookRotation(-look);

        answerPanel.transform.SetPositionAndRotation(panelPos, panelRot);

        // Success/Fail 패널도 같은 자리에 뜨도록 위치/회전을 저장해둔다
        _resultPanelPos = panelPos;
        _resultPanelRot = panelRot;

        Debug.Log($"[GameFlow] AnswerPanel → {panelPos}");
    }

    void StartAnswering()
    {
        _phase = Phase.Answering;
        _answerTimer = answerTimeLimit;

        SetupAnswerButton(btnKey,   "Key");
        SetupAnswerButton(btnHeart, "Heart");
        SetupAnswerButton(btnChair, "Chair");
        SetupAnswerButton(btnDoll,  "Doll");

        ShowOnly(answerPanel);
        Debug.Log("[GameFlow] Phase: Answering");
    }

    void SetupAnswerButton(GazeButton btn, string shapeName)
    {
        if (btn == null) return;
        btn.gameObject.SetActive(true);
        btn.ResetButton();
        btn.OnGazeConfirmed.RemoveAllListeners();
        string captured = shapeName;
        btn.OnGazeConfirmed.AddListener(() => OnAnswerSelected(captured));
    }

void OnAnswerSelected(string selected)
    {
        if (_phase != Phase.Answering) return;
        _phase = Phase.Result;
        Debug.Log($"[GameFlow] Selected={selected} Correct={_selectedShape}");

        if (selected == _selectedShape)
        {
            ShowResultPanel(successPanel);
            Debug.Log("[GameFlow] CORRECT!");
        }
        else
        {
            ShowResultPanel(failPanel);
            Debug.Log("[GameFlow] WRONG.");
            StartCoroutine(RestartAfterDelay(failRetryDelay));
        }
    }

void OnTimerExpired()
    {
        if (_phase != Phase.Answering) return;
        _phase = Phase.Result;
        ShowResultPanel(failPanel);
        StartCoroutine(RestartAfterDelay(failRetryDelay));
    }

    // 결과(Success/Fail) 패널을 AnswerPanel과 동일한 위치·방향에 배치한 뒤 표시한다
    void ShowResultPanel(GameObject panel)
    {
        if (panel != null)
            panel.transform.SetPositionAndRotation(_resultPanelPos, _resultPanelRot);
        ShowOnly(panel);
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Restart();
    }

    void Restart()
    {
        if (_activeTrigger != null)
        {
            _activeTrigger.onEntered.RemoveListener(OnAhaPointReached);
            _activeTrigger = null;
        }
        SetGO(keyMesh, false); SetGO(keyFloorMarker, false); SetGO(keyView, false);
        SetGO(heartMesh, false); SetGO(heartFloorMarker, false); SetGO(heartView, false);
        StartNavigate();
    }

    // ── Helpers ────────────────────────────────────────────────────
    void ShowOnly(GameObject target)
    {
        foreach (var p in new[]{ introPanel, navigatePanel, atAhaPanel,
                                  answerPanel, successPanel, failPanel })
            if (p != null) p.SetActive(p == target);
    }

    void SetGO(GameObject go, bool active) { if (go != null) go.SetActive(active); }
}
