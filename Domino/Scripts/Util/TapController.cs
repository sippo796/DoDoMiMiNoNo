using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dominos.AppUtil
{
    public static class TapController
    {
        public static bool GetTapClosestObject(out RaycastHit hit)
        {
            hit = default;
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity).OrderBy(o => o.distance).ToList();

            if(hits.Count > 0)
            {
                hit = hits[0];
                return true;
            }

            return false;
        }
    }
}
