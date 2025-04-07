using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IdleColors.mineral
{
    public class MineralController : MonoBehaviour
    {
        public  GameObject mineralBP;
        private bool       _invocationTriggered;

        public List<GameObject> minerals;

        void Start()
        {
            minerals = new List<GameObject>();
        }

        private void InstantiateMineral()
        {
            var mineralSpawnParent = transform.transform;
            GameObject mineral = Instantiate(
                mineralBP,
                mineralSpawnParent,
                true);
            mineral.transform.position = mineralSpawnParent.position;
            // mineral.name = "" + mineral.GetInstanceID();
            minerals.Add(mineral);
        }

        public List<GameObject> GetMinerals()
        {
            List<GameObject> returnList = new List<GameObject>();

            foreach (GameObject mineral in minerals.Reverse<GameObject>())
            {
                if (mineral == null)
                {
                    continue;
                }

                if (mineral.GetComponent<MineralHandler>().dead)
                {
                    Destroy(mineral);
                    minerals.Remove(mineral);
                    continue;
                }

                if (mineral.GetComponent<MineralHandler>().targeted ||
                    mineral.transform.position.y > 2)
                {
                    continue;
                }

                returnList.Add(mineral);
            }

            if (returnList.Count >= 10)
            {
                CancelInvoke();
                _invocationTriggered = false;
            }

            if (!_invocationTriggered &&
                returnList.Count < 3)
            {
                InvokeRepeating(nameof(InstantiateMineral), Random.Range(1f, 3f), 3f);
                InvokeRepeating(nameof(GetMinerals), 1f, 30f);
                _invocationTriggered = true;
            }

            return returnList;
        }
    }
}