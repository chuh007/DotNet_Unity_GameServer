using System.Threading.Tasks;
using CoreSystems.Util;
using Networking;
using Networking.Dtos;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoginUIController : AbstractUIScreenController
    {
        private Button _tabLogin;
        private Button _tabRegister;
        private Button _submitBtn;

        private VisualElement _rowNickname;
        private VisualElement _rowPasswordConfirm;

        private TextField _username;
        private TextField _nickname;
        private TextField _password;
        private TextField _passwordConfirm;
        private Label _message;

        private bool _isRegisterMode;
        private bool _isBusy;
        
        protected override void Bind()
        {
            _tabLogin = Btn("tab-login");
            _tabRegister = Btn("tab-register");
            _submitBtn = Btn("submit-btn");

            _rowNickname = Q<VisualElement>("row-nickname");
            _rowPasswordConfirm = Q<VisualElement>("row-password-confirm");
            
            _username = Q<TextField>("field-username");
            _nickname = Q<TextField>("field-nickname");
            _password = Q<TextField>("field-password");
            _passwordConfirm = Q<TextField>("field-password-confirm");
            _message = Lbl("message-label");

            _tabLogin.clicked += () => SetRegisterMode(false);
            _tabRegister.clicked += () => SetRegisterMode(true);
            _submitBtn.clicked += () => HandleSubmit().Forget();
            
            SetRegisterMode(false);
            //나중에 여기에 자동로그인 기능 추가.
        }
        
        private void SetRegisterMode(bool isRegisterMode)
        {
            _isRegisterMode = isRegisterMode;
            
            _tabLogin.EnableInClassList("tab-btn--active", !isRegisterMode);
            _tabRegister.EnableInClassList("tab-btn--active", isRegisterMode);
            
            _rowNickname.style.display = isRegisterMode ? DisplayStyle.Flex : DisplayStyle.None;
            _rowPasswordConfirm.style.display = isRegisterMode ? DisplayStyle.Flex : DisplayStyle.None;
            
            _submitBtn.text = isRegisterMode ? "회원가입" : "로그인";
            _submitBtn.EnableInClassList("btn--primary", !isRegisterMode);
            _submitBtn.EnableInClassList("submit-btn--register", isRegisterMode);
            
            ClearMessage(_message);
        }
        
        private async Task HandleSubmit()
        {
            if (_isBusy) return; //이미 전송중이면
            ClearMessage(_message);
    
            string username = _username.value?.Trim();
            string password = _password.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetMessage(_message, "아이디와 비밀번호를 입력하세요");
                return;
            }
    
            if(_isRegisterMode)
                await DoRegister(username, password);
            else 
                Debug.Log("로그인은 아직");
        }

        private async Task DoRegister(string username, string password)
        {
            string nickname = _nickname.value?.Trim();
            if (string.IsNullOrEmpty(nickname))
            {
                SetMessage(_message, "닉네임을 입력하세요");
                return;
            }

            if (password != _passwordConfirm.value)
            {
                SetMessage(_message, "비밀번호와 확인이 일치하지 않습니다");
                return;
            }
    
            SetBusy(true);
            ApiResult<UserResponse> result = await AuthApi.RegisterAsync(username, nickname, password);
            SetBusy(false);

            if (result.IsSuccess)
            {
                SetRegisterMode(false); //로그인모드로 전환
                _username.value = username;
                SetMessage(_message, "회원가입 완료! 로그인 해주세요.", success: true);
            }
            else
            {
                SetMessage(_message, result.Error.ToUserMessage());
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            _submitBtn.SetEnabled(!isBusy);
        }

    }
}
