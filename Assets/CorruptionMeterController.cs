using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionMeterController : MonoBehaviour
{
        public CorruptionMeter corruptionMeter;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                corruptionMeter.Increase(10);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                corruptionMeter.Decrease(10);
            }
        }

}
