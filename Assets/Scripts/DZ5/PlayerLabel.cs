using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ5
{
    public class PlayerLabel : MonoBehaviour
    {
        public void DrawLabel(Camera camera)
        {
            if (camera == null) return;

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.blue;
            var objects = ClientScene.objects;
            for(int i = 0; i < objects.Count; i++)
            {
                var obj = objects.ElementAt(i).Value;
                var position = camera.WorldToScreenPoint(obj.transform.position);

                var collider = obj.GetComponent<Collider>();
                if(collider != null && Visible(camera, collider) && obj.transform != transform)
                {
                    GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10, obj.name.Length * 10.5f)), obj.name, style);
                }
            }
        }

        public bool Visible(Camera camera, Collider collider)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
        }
    }
}