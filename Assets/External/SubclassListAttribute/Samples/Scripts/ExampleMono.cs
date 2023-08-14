using UnityEngine;

namespace SubclassList_Examples
{
    public class ExampleMono : MonoBehaviour
    {
        [SubclassList.SubclassList(typeof(A)), SerializeField] private ABC_Container abc_list;
    }
}