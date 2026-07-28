using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }
        
        [SerializeField] private string serverUrl = "http://localhost:5000";
        [SerializeField] private int timeoutSeconds = 10;
        
        public string ServerUrl => serverUrl;

        //서버쪽 메시지들이 CamelCase로 되어 있어서 그걸 기반으로 Resolving하라는 뜻. 
        //Json만들때 Null값은 무시하고 만들기.
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() //씬에 ApiClient가 없다면 자동으로 생성(씬이 로드되기전에)
        {
            if(Instance == null)
                new GameObject("ApiClient").AddComponent<ApiClient>();
        }

        private void Awake()
        {
            if(Instance != null) {
                Destroy(gameObject);
                return;
            }

            //싱글톤 로직
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        #region 매서드 콜 Http

        public async Task<ApiResult<TRes>> SendAsync<TRes>(string method, string path, object body, CancellationToken ct = default)
        {
            string url = $"{ServerUrl}/{path.TrimStart('/')}";
            using UnityWebRequest req = new UnityWebRequest(url, method);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.timeout = timeoutSeconds;

            if (body != null)
            {
                string json = JsonConvert.SerializeObject(body, JsonSettings); //설정한 규칙에 맞게 Json직렬화 시작.
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)); 
                //Json을 byteArray로 변경하여 전송
                req.SetRequestHeader("Content-Type", "application/json"); //요청헤더를 json으로
            }
            req.SetRequestHeader("Accept", "application/json");
            
            try
            {
                await using (ct.Register(() => req.Abort()))
                    await req.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ApiClient] {method} {path} 예외 : {e.Message}");
                throw;
            }
            
            long statusCode = req.responseCode;
            string text = req.downloadHandler?.text;

            if (req.result == UnityWebRequest.Result.Success)
            {
                TRes data = default;
                if (!string.IsNullOrEmpty(text))
                    data = JsonConvert.DeserializeObject<TRes>(text, JsonSettings);
                
                return ApiResult<TRes>.Success(data, statusCode);
            }
            
            ApiError error = ParseError(text, statusCode, req.error);
            Debug.LogWarning($"[ApiClient] {method} {path} 실패 : {statusCode} / {error.Message}");
            return ApiResult<TRes>.Failure(error, statusCode);
        }

        private static ApiError ParseError(string body, long status, string transportError)
        {
            ApiError error = new ApiError {Status = (int) status, Raw = body};

            if (string.IsNullOrEmpty(body))
            {
                error.Message = string.IsNullOrEmpty(transportError) ? "요청 실패" : transportError;
                return error;
            }

            try
            {
                ProblemDetailsDto pd = JsonConvert.DeserializeObject<ProblemDetailsDto>(body, JsonSettings);
                if (pd != null)
                {
                    error.Message = !string.IsNullOrEmpty(pd.Message) ? pd.Message
                        : !string.IsNullOrEmpty(pd.Title) ? pd.Title
                        : "요청에 실패했습니다.";
                    error.Errors = pd.Errors;
                    return error;
                }
            }
            catch
            {
                // Json응답이 아니라사 Deserialize에서 예외 발생시, Message에 body를 통으로 넣는다.
            }

            error.Message = body; 
            return error;
        }

        #endregion
        
        public Task<ApiResult<TRes>> GetAsync<TRes>(string path, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbGET, path, null, ct);
        public Task<ApiResult<TRes>> PostAsync<TRes>(string path, object body, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbPOST, path, body, ct);
        public Task<ApiResult<TRes>> PutAsync<TRes>(string path, object body, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbPUT, path, body, ct);
        public Task<ApiResult<TRes>> DeleteAsync<TRes>(string path, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbDELETE, path, null, ct);
        
    }
}