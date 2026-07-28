using System.Collections.Generic;

namespace Networking
{
    //서버 응답을 그대로 채워넣어주는 객체라 get; set; 필요
    public class ApiError
    {
        public string Message { get; set; }
        public int Status { get; set; }
        
        public Dictionary<string, string[]> Errors { get; set; } //key: field, value: error message
        public string Raw { get; set; } //디버그용 메시지 원본
        
        public string ToUserMessage()
        {
            if (Errors != null)
            {
                foreach (KeyValuePair<string, string[]> kv in Errors)
                {
                    if (kv.Value != null && kv.Value.Length > 0)
                        return kv.Value[0]; //첫번째 에러만 보여준다.
                }
            }
            
            return string.IsNullOrEmpty(Message) ? "오류가 발생했습니다." : Message;
        }
    }
}