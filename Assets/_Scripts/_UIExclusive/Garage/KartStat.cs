using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KartStat : MonoBehaviour
{
    [SerializeField] private Canvas cvsSelect;
    [SerializeField] private Canvas cvsGarage;

    private float rotAng = 0;
    [SerializeField] private float rotSpeed = 1.0f;

    [SerializeField] private Image linePrefab;

    public List<RectTransform> Dots = new List<RectTransform>();

    [SerializeField] private List<RectTransform> pentas = new List<RectTransform>();

    [SerializeField] private List<RectTransform> lines = new List<RectTransform>();

    private void FixedUpdate()
    {
        if(rotAng >= 360.0f)
        {
            rotAng = 0f;
        }

        this.transform.rotation = Quaternion.Euler(0, rotAng, 0);

        //lines[0].rectTransform.rotation = Quaternion.Euler(0, 0, rotAng);
        //lines[0].rectTransform.localPosition = new Vector3(0, 0, 0);

        rotAng += Time.deltaTime * rotSpeed;

        //Debug.Log(Vector3.SignedAngle(pentas[2].localPosition - pentas[1].localPosition, Vector3.right, Vector3.right));
        //Debug.Log(Vector3.Angle(pentas[1].localPosition - pentas[0].localPosition, Vector3.right));

    }

    public void setupKart(KartAsset _kart)
    {
        GameObject spawnedKart = Instantiate(_kart.AssetPrefab, this.transform.position, Quaternion.identity);
        spawnedKart.GetComponent<KartController>().spawnMode = 1;
        spawnedKart.GetComponent<KartController>().onDisplayScale = 0.9f;
        spawnedKart.transform.SetParent(this.transform);
        spawnedKart.transform.localPosition = Vector3.zero;


        List<float> stats = new List<float>()
        {
            _kart._acceleration / 1000,
            _kart._maxSpeed / 250,
            _kart._drift / 50,
            _kart._control / 50,
            _kart._weight / 3000
        };

        stats.Add(stats[0]);
        pentas.Add(pentas[0]);

        for (int i = 0; i < lines.Count; i++)
        {
            Vector3 start = Vector3.Lerp(pentas[i].localPosition, Vector3.zero, 1.0f - stats[i]);
            
            Vector3 end = Vector3.Lerp(pentas[i + 1].localPosition, Vector3.zero, 1.0f - stats[i + 1]);

            Dots[i].localPosition = start;
            
            setupLines(lines[i], start, end);
        }

        Debug.Log("LINE!");
        cvsSelect.gameObject.SetActive(false);
        cvsGarage.gameObject.SetActive(true);
    }

    private void setupLines(RectTransform line, Vector3 pStart, Vector3 pEnd)
    {
        float angle = VectorAngle(pStart, pEnd);

        float distance = Vector3.Distance(pStart, pEnd);

        line.sizeDelta = new Vector2(distance, 5);

        line.anchoredPosition = pStart;
        
        line.transform.localRotation = Quaternion.AngleAxis(-angle, Vector3.forward);

    }

    private float VectorAngle(Vector3 from, Vector3 to)
    {
        float angle;

        float height = to.y - from.y;
        //angle = Vector3.SignedAngle(to - from, Vector3.right, Vector3.right);
        angle = Vector3.Angle(to - from, Vector3.right);


        return height > 0 ? -angle : angle;
    }
}
