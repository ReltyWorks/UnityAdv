using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PokemonGo.Runtime.Workflows
{
    public abstract class Workflow : MonoBehaviour
    {
        protected virtual void Start()
        {
            StartCoroutine(C_Workflow());
        }

        protected abstract IEnumerator C_Workflow();
    }
}
