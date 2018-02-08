using System;
using UnityEngine;

namespace DivideByZero.Gyazo
{
    public class GyazoUploadManager : MonoBehaviour
    {
        [SerializeField] private string _accessToken;
        [SerializeField] private bool _isDebugLogOutput;

        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_accessToken)) throw new Exception("AccessTokenをセットしてください");
                return _accessToken;
            }
        }

        public bool IsDebugLogOutput
        {
            get { return _isDebugLogOutput; }
        }

        private static GyazoUploadManager sinstance;
        public static GyazoUploadManager Instance
        {
            get
            {
                if (sinstance == null)
                {
                    sinstance = FindObjectOfType<GyazoUploadManager>();
                    if (sinstance == null)
                    {
                        var obj = new GameObject(typeof(GyazoUploadManager).Name);
                        sinstance = obj.AddComponent<GyazoUploadManager>();
                    }
                }
                return sinstance;
            }
        }

        void Awake()
        {
            if (this == Instance)
            {
                DontDestroyOnLoad(Instance);
                return;
            }
            Destroy(gameObject);
        }

        public static GyazoUploader CreateGyazoUploader()
        {
            return new GyazoUploader();
        }
    }
}