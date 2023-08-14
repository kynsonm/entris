using UnityEngine;

namespace SubclassList_Examples
{
    public class Example_SO : ScriptableObject
    {
        [SubclassList.SubclassList(typeof(A)), SerializeField] private ABC_Container abc_list;
    }
}