using System.Threading.Tasks;
using bookpage.webui.Identity;
using bookpage.webui.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bookpage.webui.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<User> _userManager;//UserManager ıdentity içinde varolan bir yapı. <User> clasımız.Usermanager kullanıcı oluşturma login parla sıfırlama gibi temel işlemleri barındırır.
        private SignInManager<User> _singInManager;//cookie işleri

        public AccountController(UserManager<User> userManager,SignInManager<User> singInManager)
        {
            _userManager=userManager;
            _singInManager=singInManager;
        }
        public IActionResult Login(string ReturnUrl=null)
        {//mesela appte bir sayfaya girmek istedim login ister gireresem beni o en son bastıım sayfaya göndersin. bunun için yukarı string return url = null dedim
            return View(new LoginModel{
                ReturnUrl=ReturnUrl
            });//bunula bastım login.cshtml'de gizledim input olarak aşağıdada post edicem.Yani urldeki sayfayı açınca return nul yazan ve yanında olan şetler var onlaır alır burada tutar postlada gönderir
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }//eğer hata yoksa

            var user= await _userManager.FindByNameAsync(model.UserName);//vt'den verdiğim useri alalım bakalım varmı
            if(user==null)
            {
                ModelState.AddModelError("","Girdiğiniz kullanıcı adı kayıtlı değil, lütfen tekrar deneyiniz");
                return View();
            }
            var result=await _singInManager.PasswordSignInAsync(user,model.Password,true,false);//kullanıcının tarayıcısına coockie bırakıcaz. 1.parametre useri verdim, şifre bekliyo 2. şifreyi verdim. coockienin tarayıcı kapanınca silinip silinmemeisyle alakalı 3.false dedim tarayıcı kapanınca bilgi silinir,şimdi true yaptım 60 dksonra silinir. kullanıcı başarısız giriş yaparsa 4. false dersek hesap kilitlenme işlemi kapalı
            if(result.Succeeded)
            {
                return Redirect(model.ReturnUrl??"~/");//Burada post ettim ?? null kontrolü yapar eğer nula eşit anasayfaya git
            }
            ModelState.AddModelError("","Girdiğiniz kullanıcı adı veya parola yanlış");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user=new User()
            {
                FirstName=model.FirstName,
                LastName=model.LastName,
                UserName=model.UserName,
                Email=model.EMail,
            };//modeldekileri usere attım passwordi kashlememiz şifrelemememiz gerek açık olmaması gerek usermanager aracılığı ile halledeceğiz
            
            var result=await _userManager.CreateAsync(user,model.Password);//creatasync bir user bekliyodu verdik birde şifre onuda modelden gönderdik
            if(result.Succeeded)
            {
                //gitmeden önce token olması lazım
                return RedirectToAction("Login","Account");
            }
            ModelState.AddModelError("","Bilinmeyen bir hata oluştu, lütfen tekrar deneyiniz");//modalstateadderror mantığı model state içine gönderiyosun "" boş olduğu için en üstte verdiğin hata mesajı yazar
            return View(model);
        }





        
    }
}