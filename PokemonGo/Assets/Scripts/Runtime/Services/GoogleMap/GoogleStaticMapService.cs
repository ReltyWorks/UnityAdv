using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace PokemonGo.Runtime.Services.GoogleMap
{
    public class GoogleStaticMapService : MonoBehaviour
    {
        private const string BASE_URL = "https://maps.googleapis.com/maps/api/staticmap?";
        private const string API_KEY = "AIzaSyAbTe7ZpSs9ukFMUT0TjzIQ1L91R-JDoqI";


        public void LoadMap(double latitude, double longitude, int zoom, Vector2Int size, Action<Texture2D> onComplete)
        {
            StartCoroutine(C_LoadMap(latitude, longitude, zoom, size, onComplete));
        }

        public IEnumerator C_LoadMap(double latitude, double longitude, int zoom, Vector2Int size, Action<Texture2D> onComplete)
        {
            string url =
                BASE_URL +
                "center=" + latitude + "," + longitude +
                "&zoom=" + zoom +
                "&size=" + size.x + "x" + size.y +
                "&key=" + API_KEY;

            Debug.Log($"[{nameof(GoogleStaticMapService)}] Request map texture ... {url}");

            using (var requestTexture = UnityWebRequestTexture.GetTexture(url))
            {
                yield return requestTexture.SendWebRequest();

                if (requestTexture.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[{nameof(GoogleStaticMapService)}] Failed to request map texture ... {requestTexture.result}");
                    onComplete?.Invoke(null);
                    yield break;
                }

                Texture2D texture = DownloadHandlerTexture.GetContent(requestTexture);
                onComplete?.Invoke(texture);
            }
        }
    }
}