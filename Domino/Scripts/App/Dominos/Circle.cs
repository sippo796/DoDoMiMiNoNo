using UnityEngine;

namespace Dominos.App
{
    public class Circle : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private float Radius = .3f;

        [SerializeField]
        public int PointNum = 100;

        void Start()
        {
            DrawCircle();
        }

        public void DrawCircle()
        {
            var theta_scale = 0.1f;
            var size = (int)((2f * Mathf.PI) / theta_scale) + 1;

            lineRenderer.startColor = new Color(1, 0, 0);
            lineRenderer.endColor = new Color(1, 1, 1);
            lineRenderer.startWidth = 0.1F;
            lineRenderer.endWidth = 0.1F;
            lineRenderer.positionCount = size;

            int i = 0;
            for(float theta = 0; theta <= 2 * Mathf.PI; theta += 0.1f)
            {
                var x = Radius * Mathf.Sin(theta);
                var y = Radius * Mathf.Cos(theta);

                var pos = new Vector3(x, 0, y);
                lineRenderer.SetPosition(i, pos);
                i += 1;
            }
        }
    }
}
