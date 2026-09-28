using UnityEngine;

namespace View
{
    public class FloorController : ObjectDestroyer
    {
        public override void GetImpact(float damage)
        {
            // El piso no daña ciudades: un impacto en el suelo es inofensivo (M1a).
        }
    }
}