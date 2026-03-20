using UnityEngine;

public class CreditsBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private GameObject _textContainer;
    private float _timer = 0f;

    private void OnEnable()
    {
        _timer = 0f;
        _textContainer.transform.position = _baseTransform.position;
    }

    private void Update()
    {
        if (_timer < _duration)
        {
            _textContainer.transform.position += _textContainer.transform.up * _speed * Time.deltaTime;
            _timer += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
