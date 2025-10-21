using UnityEngine;

public class exitTriggers : MonoBehaviour
{
    public bool playersThrough = false;
    private int _playerCount = 0;
    
    // private bool _countDown = false;
    // private float _countDownTimer = 0;
    // [SerializeField] private float countDownTime = 0.5f;
    
    
    private void Update()
    {
        playersThrough = _playerCount >= 2;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount++;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount--;
        }
    }
}
