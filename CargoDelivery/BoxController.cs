﻿using System.Collections;
using Obi;
using UnityEngine;
using UnityEngine.Events;

public class BoxController : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _gameOverEvent;

    [SerializeField]
    private UnityEvent _gameWinEvent;

    [SerializeField]
    private GameObject _rope;
    [SerializeField] 
    private float _speed = 1;

    [SerializeField]
    private Transform _finish;

    private ObiParticleAttachment _ropeAttachment;

    private void Awake()
    {
        // Получаем ссылку на крепление ящика к веревке:
        var ropeAttachments = _rope.GetComponents<ObiParticleAttachment>();
        foreach (var ropeAttachment in ropeAttachments)
        {
            if (ropeAttachment.target.gameObject == gameObject)
            {
                _ropeAttachment = ropeAttachment;
                break;
            }
        }
    }

    public bool NeedToPut()
    {
        return Mathf.Round(transform.position.x) == Mathf.Round(_finish.position.x);
    }

    private void OnCollisionEnter()
    {
        // Отцепляем ящик от веревки
        _ropeAttachment.enabled = false;
        _gameOverEvent.Invoke();
    }

    // TODO: Вызовите данный метод, когда ящик на веревке окажется над финишной точкой 
    // Vector3 position - точка, на которую должен опуститься ящик
    public void DropDown()
    {
        var position = _finish.position;
        // Делаем соединение с веревкой статичным (чтоб она растягивалась при движении ящика)
        _ropeAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Static;
        
        // Выключаем Rigidbodies ящика
        GetComponent<ObiRigidbody>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true; 
        
        // Перемещаем ящик в точку ровно над финишем
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);

        // Медленно двигаем ящик вниз
        StartCoroutine(DeliverToTarget(position));
        _gameWinEvent.Invoke();
    }
    
    private IEnumerator DeliverToTarget(Vector3 targetPosition)
    {
        // Пока позиция ящика не совпадет с позицией финишной точки
        while(transform.position != targetPosition)
        {
            // Меняем позицию ящика в соответствии со скоростью
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 
                _speed * Time.deltaTime);
            
            // Пропускаем кадр
            yield return null;
        }
        
        // Когда ящик занял нужную позицию - отцепляем ящик от веревки
        _ropeAttachment.enabled = false;
    }
}