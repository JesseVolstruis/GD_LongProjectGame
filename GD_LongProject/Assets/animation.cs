using UnityEngine;

public class animation : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int HasItem = Animator.StringToHash("HasItem");

    private bool hasItem = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [SerializeField] Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //WALK n STOP
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool(IsWalking, true);
        }
        if (!Input.GetKey(KeyCode.W))
        {
            animator.SetBool(IsWalking, false);
        }
        
        //Item
        if (Input.GetKeyDown(KeyCode.E))
        {
            hasItem = !hasItem;
            animator.SetBool(HasItem, hasItem);
        }
        
        //JUMP
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger(Jump);
            
        }
        
    }
}
