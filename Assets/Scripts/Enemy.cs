using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct Target
    {
        public enum Mode
        {
            Position,
            Tag,
            Name,
            Component,
        }

        [SerializeField]
        private Mode targetBy;
        public Mode By => targetBy;
        private bool TagMode => targetBy == Mode.Tag;
        private bool NameMode => targetBy == Mode.Name;
        private bool ComponentMode => targetBy == Mode.Component;
        private bool PositionMode => targetBy == Mode.Position;
        [SerializeField, ShowIf(nameof(PositionMode)), AllowNesting]
        private Vector3 targetPosition;
        
        [SerializeField, Tag, ShowIf(nameof(TagMode)), AllowNesting]
        private string targetTag;
        public string Tag => targetTag;

        [SerializeField, ShowIf(nameof(NameMode)), AllowNesting]
        private string targetName;
        public string Name => targetName;

        [SerializeField, ShowIf(nameof(ComponentMode)), AllowNesting]
        private Object targetType;
        public Object Type => targetType;

        private Transform target;
        public Vector3 Position => PositionMode ? targetPosition : target.position;
        
        public void Set(Transform newTarget)
        {
            target = newTarget;
        }
    }

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private int difficulty;
    public int Difficulty => difficulty;

    [SerializeField]
    private Target target;

    [SerializeField]
    private float moveSpeed;


    private void Start()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        switch (target.By)
        {
            case Target.Mode.Tag:
                var taggedObjects = GameObject.FindGameObjectsWithTag(target.Tag);
                target.Set(GetRandomFromList(taggedObjects).transform);
                break;
            case Target.Mode.Name:
                target.Set(GameObject.Find(target.Name).transform);
                break;
            case Target.Mode.Component:
                var foundComponents = FindObjectsOfType(target.Type.GetType());
                target.Set(((Component)GetRandomFromList(foundComponents)).transform);
                break;
            default:
                target.Set(null);     
                break;
        }
    }

    public void DoUpdate(float deltaTime)
    {
        MoveTo(target.Position, moveSpeed * deltaTime);
    }

    private void MoveTo(Vector3 targetPosition, float distance)
    {
        Vector3 forward = targetPosition - transform.position;
        forward.y = 0;
        forward.Normalize();
        transform.forward = forward;

        controller.Move(distance * forward);
    }

    public static T GetRandomFromList<T>(IList<T> list)
    {
        int len = list.Count;
        int index = Random.Range(0, len);
        return list[index];
    }

}