using UnityEngine;

public class animation : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int HasItem = Animator.StringToHash("HasItem");

    private bool _hasItem = false;
    
   private Animator _animator;
   
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        //WALK n STOP
        if (Input.GetKey(KeyCode.W))
        {
            _animator.SetBool(IsWalking, true);
        }
        if (!Input.GetKey(KeyCode.W))
        {
            _animator.SetBool(IsWalking, false);
        }
        
        //Item
        if (Input.GetKeyDown(KeyCode.E))
        {
            _hasItem = !_hasItem;
            _animator.SetBool(HasItem, _hasItem);
        }
        
        //JUMP
        if (Input.GetKey(KeyCode.Space))
        {
            _animator.SetTrigger(Jump);
        }
        
    }
}
