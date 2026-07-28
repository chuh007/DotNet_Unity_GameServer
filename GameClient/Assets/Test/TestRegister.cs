using System.Threading.Tasks;
using Networking;
using Networking.Dtos;
using UnityEngine;

namespace Test
{
    public class WebRequestTest : MonoBehaviour
    {
        [SerializeField] private string username;
        [SerializeField] private string nickname;
        [SerializeField] private string password;

        [ContextMenu("Test Web Request-register")]
        private async Task TestRegister()
        {
            Debug.Log("Test Register");
            ApiResult<UserResponse> result = await AuthApi.RegisterAsync(username, nickname, password);
            
            Debug.Log($"{result.Data.Id} 로 가입되었습니다. : {result.Data.Nickname}" );
        }
    }
}