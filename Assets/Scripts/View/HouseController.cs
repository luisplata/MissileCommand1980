using Model;
using UnityEngine;

namespace View
{
    public class HouseController : ObjectDestroyer
    {
        [SerializeField] private int cityId;

        public int CityId => cityId;

        public override void GetImpact(float damage)
        {
            if (!RunSession.Current.CityAlive(cityId)) return;
            RunSession.Current.KillCity(cityId);
            gameObject.SetActive(false);
        }
    }
}