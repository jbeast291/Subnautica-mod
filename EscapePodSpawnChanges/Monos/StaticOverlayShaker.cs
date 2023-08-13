using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UWE;


namespace LifePodRemastered.Monos
{
    public class StaticOverlayShaker : MonoBehaviour
    {
        Image img;
        public void Start()
        {
            img = gameObject.GetComponent<Image>();
            img.color = new Color(255, 255, 255, 0.015f); // 0.03776348f
        }

        public void Update()
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1);
            transform.localPosition = new Vector3(Random.Range(0, 100), Random.Range(0, 100), 0);
        }
    }
}
