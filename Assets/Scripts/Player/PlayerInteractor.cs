using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("输入系统")]
    public InputAction interactAction; // 在Inspector里绑定Interact Action

    [Header("交互检测")]
    public float interactRange = 1.3f;  // 检测范围
    public LayerMask interactLayer;     // 可交互对象所在层
    public Vector2 interactOffset = new Vector2(0f, 1f); // 检测中心的偏移量

    private IInteractable currentTarget;

    private void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += OnInteract; //按键触发回调
        LoadInteractKey(); //加载保存的按键绑定
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    private void Update()
    {
        DetectInteractable();
    }

    // 计算检测圆位置，自动跟随玩家朝向
    private Vector2 GetDetectPosition()
    {
        float dir = Mathf.Sign(transform.localScale.x);    //1=右, -1=左
        return (Vector2)transform.position + new Vector2(interactOffset.x * dir, interactOffset.y);
    }

    // 检测附近可交互对象
    void DetectInteractable()
    {
        Vector2 detectPos = GetDetectPosition();

        Collider2D hit = Physics2D.OverlapCircle(detectPos, interactRange, interactLayer);

        if (hit != null)
        {
            currentTarget = hit.GetComponent<IInteractable>();
            // TODO: 显示提示UI，例如“按键互动”
        }
        else
        {
            currentTarget = null;
            // TODO: 隐藏提示UI
        }
    }

    // Input System 回调触发交互
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    // 保留了修改按键的接口
    public void RebindInteract()
    {
        interactAction.Disable();
        interactAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                interactAction.Enable();
                operation.Dispose();
                // 保存玩家选择
                PlayerPrefs.SetString("InteractKey", interactAction.bindings[0].effectivePath);
            })
            .Start();
    }

    // 加载玩家保存的按键
    void LoadInteractKey()
    {
        if (PlayerPrefs.HasKey("InteractKey"))
        {
            string path = PlayerPrefs.GetString("InteractKey");
            interactAction.ApplyBindingOverride(0, path);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 detectPos = (Vector2)transform.position + interactOffset;
        Gizmos.DrawWireSphere(detectPos, interactRange);   //可交互范围
    }
}