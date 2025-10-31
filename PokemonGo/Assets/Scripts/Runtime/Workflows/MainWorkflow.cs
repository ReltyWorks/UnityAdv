using PokemonGo.Runtime.Services;
using System.Collections;
using UnityEngine;
using VContainer;

namespace PokemonGo.Runtime.Workflows
{
    public class MainWorkflow : Workflow
    {
        [Inject] InternetConnectionService _internetConnectionService;


        protected override IEnumerator C_Workflow()
        {
            Debug.Log($"[{nameof(MainWorkflow)}] internet connection : {_internetConnectionService.isConnected}");
            yield break;
        }
    }
}
