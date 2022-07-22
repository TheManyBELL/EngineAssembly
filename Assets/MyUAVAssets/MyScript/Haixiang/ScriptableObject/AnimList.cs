using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Animation Object List", menuName = "New Animation Object List")]
public class AnimList : ScriptableObject
{
    public List<GameObject> list;
}
