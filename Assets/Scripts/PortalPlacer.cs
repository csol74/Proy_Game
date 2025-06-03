using UnityEngine;
using System.Collections.Generic;

public class PortalPlacer : MonoBehaviour
{
    public GameObject portalPrefab;
    public Camera cam;
    public LayerMask floorMask;
    public float placementRange = 200f;
    public float deleteRadius = 1f;

    private List<GameObject> activePortals = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Colocar portal
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, placementRange, floorMask))
            {
                GameObject newPortal = Instantiate(portalPrefab, hit.point, Quaternion.LookRotation(Vector3.up));

                // Limitar a 2 portales
                if (activePortals.Count >= 2)
                {
                    Destroy(activePortals[0]);
                    activePortals.RemoveAt(0);
                }

                activePortals.Add(newPortal);

                // Conectar portales
                if (activePortals.Count == 2)
                {
                    Portal a = activePortals[0].GetComponent<Portal>();
                    Portal b = activePortals[1].GetComponent<Portal>();
                    a.linkedPortal = b;
                    b.linkedPortal = a;
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) // Eliminar portal cercano
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = cam.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit, placementRange, floorMask))
            {
                GameObject closest = null;
                float closestDist = deleteRadius;

                foreach (GameObject portal in activePortals)
                {
                    if (portal == null) continue;

                    float dist = Vector3.Distance(hit.point, portal.transform.position);
                    if (dist < closestDist)
                    {
                        closest = portal;
                        closestDist = dist;
                    }
                }

                if (closest != null)
                {
                    activePortals.Remove(closest);
                    Destroy(closest);

                    // Romper el enlace si había pareja
                    foreach (GameObject p in activePortals)
                    {
                        if (p != null)
                        {
                            Portal portal = p.GetComponent<Portal>();
                            portal.linkedPortal = null;
                        }
                    }
                }
            }
        }
    }
}