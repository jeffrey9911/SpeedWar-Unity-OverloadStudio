using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartStat : MonoBehaviour
{
    [SerializeField] private Canvas cvsSelect;
    [SerializeField] private Canvas cvsGarage;

    private float rotAng = 0;
    [SerializeField] private float rotSpeed = 1.0f;

    [SerializeField] private Image linePrefab;

    [SerializeField] private RectTransform pent_centre;
    [SerializeField] private List<RectTransform> pentas = new List<RectTransform>();

    [SerializeField] private List<Image> lines = new List<Image>();

    private void FixedUpdate()
    {
        if(rotAng >= 360.0f)
        {
            rotAng = 0f;
        }

        this.transform.rotation = Quaternion.Euler(0, rotAng, 0);

        rotAng += Time.deltaTime * rotSpeed;

    }

    public void setupKart(KartAsset _kart)
    {
        GameObject spawnedKart = Instantiate(_kart.AssetPrefab, this.transform.position, Quaternion.identity);
        spawnedKart.transform.SetParent(this.transform);
        spawnedKart.transform.localPosition = Vector3.zero;
        Destroy(spawnedKart.GetComponent<KartController>());
        Destroy(spawnedKart.GetComponent<Rigidbody>());

        List<float> stats = new List<float>();
        stats.Add(_kart._acceleration / 100);
        stats.Add(_kart._maxSpeed / 300);
        stats.Add(_kart._drift / 100);
        stats.Add(_kart._control / 100);
        stats.Add(_kart._weight / 3000);

        for (int i = 0; i < stats.Count; i++)
        {
            Vector2 start = Vector2.Lerp(pentas[i].anchoredPosition, pent_centre.anchoredPosition, stats[i]);
            Vector2 end = Vector2.Lerp(pentas[(i == (stats.Count - 1) ? 0 : i + 1)].anchoredPosition, pent_centre.anchoredPosition, stats[i]);

            setupLines(lines[i], start, end);
        }

        Debug.Log("LINE!");
        cvsSelect.gameObject.SetActive(false);
        cvsGarage.gameObject.SetActive(true);
    }

    private void setupLines(Image line, Vector2 pStart, Vector2 pEnd)
    {
        float distance = Vector2.Distance(pStart, pEnd);
        float angle = Vector2.SignedAngle(pStart - pEnd, Vector2Int.left);

        line.GetComponent<RectTransform>().anchoredPosition = (pStart + pEnd) / 2;
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, 5);
        line.transform.localRotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
}
