using UnityEngine;

namespace Dominos.Util.Extensions
{
    public static class SetLayerExtension
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layerNo)
        {
            gameObject.layer = layerNo;

            foreach(Transform child in gameObject.transform)
            {
                SetLayerRecursively(child.gameObject, layerNo);
            }
        }
    }
}
